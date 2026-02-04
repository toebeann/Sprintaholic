using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Tobey.Sprintaholic;

internal static class ConfigFileExtensions
{
    internal static readonly SortedDictionary<Version, Action<ConfigFile>> migrations = new()
    {
        // v1.3.0 introduces `Controls.Sprint control mode` enum entry which replaces `Controls.Hold to sprint` and additionally adds a
        // setting "Always" to simplify users wanting to "run forever"

        // v1.3.0 additionally introduces `Controls.Sprint by default`, default false which when enabled, swaps your walk and sprint speed
        // If users have already manually swapped their walk and sprint speed, we should swap the speeds back and enable this setting
        [new("1.3.0")] = (ConfigFile config) =>
        {
            var walkSpeed = config.Bind(Definitions.WalkSpeed, -1f);
            var sprintSpeed = config.Bind(Definitions.SprintSpeed, -1f);

            // migrate `Controls.Hold to sprint` to `Controls.Sprint control mode`
            var holdToSprint = config.Bind("Controls", "Hold to sprint", false);
            config.Bind(Definitions.SprintControlMode, (holdToSprint.Value, walkSpeed.Value, sprintSpeed.Value) switch
            {
                // when user has set walk and sprint speed to be approximately equal, assume they want to "run forever"
                not (_, < 0, < 0) when Mathf.Approximately(walkSpeed.Value, sprintSpeed.Value) => Definitions.SprintMode.Always,

                // otherwise, simply migrate the setting from the old `Controls.Hold to sprint` setting
                (true, _, _) => Definitions.SprintMode.Hold,

                _ => Definitions.SprintMode.Toggle,
            });
            config.Remove(holdToSprint.Definition);

            if (sprintSpeed.Value < walkSpeed.Value && sprintSpeed.Value > 0)
            {   // migrate to `Controls.Sprint by default`
                float newSprintSpeed = walkSpeed.Value;
                walkSpeed.Value = sprintSpeed.Value;
                sprintSpeed.Value = newSprintSpeed;
                config.Bind(Definitions.SprintByDefault, true);
            }

            if (walkSpeed.Value < 0) config.Remove(walkSpeed.Definition);
            if (sprintSpeed.Value < 0) config.Remove(sprintSpeed.Definition);
        },

        // v1.2.0 introduces `Controls.Auto disable sprint, default true to mimic the way the sprint toggle works with gamepad controls
        // Users of Sprintaholic prior to this version will be used to having to disable it manually, so we should default it to false to avoid messing with their muscle memory
        [new("1.2.0")] = (ConfigFile config) => config.Bind(Definitions.AutoDisableSprint, false),
    };

    extension(ConfigFile config)
    {
        public ConfigEntry<T> Bind<T>(ConfigDefinition<T> definition) => config.Bind(
            configDefinition: definition.Definition,
            configDescription: definition.Description,
            defaultValue: definition.DefaultValue);

        public ConfigEntry<T> Bind<T, U>(ConfigDefinition<U> definition, T defaultValue) => config.Bind(
            configDefinition: definition.Definition,
            configDescription: definition.Description,
            defaultValue: defaultValue);

        public void ApplyMigrations(string version, ManualLogSource logger)
        {
            try
            {
                string searchString = "## Settings file was created by plugin ";

                string line = File.ReadLines(config.ConfigFilePath) // throws if can't read file
                    .First(line => line?.StartsWith(searchString) ?? false) // throws if no line satisfies criteria
                    [searchString.Length..];

                Version configVersion = Version.Parse(line[(line.LastIndexOf(" v") + 2)..]);

                var migrationsToApply = migrations
                    .SkipWhile(kvp => kvp.Key <= configVersion)
                    .ToList();

                if (configVersion >= Version.Parse(version) || !migrationsToApply.Any()) return; // no need to apply migrations

                // // prevent config getting saved for migrations to reduce file i/o
                bool saveOnConfigSet = config.SaveOnConfigSet;
                config.SaveOnConfigSet = false;

                foreach (var migration in migrationsToApply)
                {
                    logger.LogInfo($"Applying config migration v{configVersion} -> v{migration.Key}");
                    migration.Value(config);
                    configVersion = migration.Key;
                }

                config.SaveOnConfigSet = saveOnConfigSet; // reapply original config setting
                config.Save(); // ensure migrations are saved
            }
            catch (FileNotFoundException) // config file doesn't exist, no need to apply migrations
            { }
            catch (Exception e) // some other error occured, just print it
            {
                logger.LogError(e);
            }
        }
    }
}