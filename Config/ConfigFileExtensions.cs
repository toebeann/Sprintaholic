using BepInEx.Configuration;
using BepInEx.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Tobey.Sprintaholic.Config;

internal static class ConfigFileExtensions
{
    internal static readonly SortedDictionary<Version, Action<ConfigFile>> migrations = new()
    {
        [new("1.2.0")] = (ConfigFile config) =>
        {
            // v1.2.0 introduces `Controls.Auto disable sprint` config entry, default true to mimic the way the sprint toggle works with gamepad controls
            // Users of Sprintaholic prior to this version will be used to having to disable it manually, so we should default it to false for them to avoid messing with their muscle memory
            config.Bind(Definitions.AutoDisableSprint, false);
        },
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

                Version configVersion = Version.Parse(line[line.LastIndexOf(" v")..]);

                var migrationsToApply = migrations
                    .SkipWhile(kvp => kvp.Key <= configVersion)
                    .OrderBy(kvp => kvp.Key)
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
            }
            catch (FileNotFoundException) // config file doesn't exist, no need to apply migrations
            { }
            catch // some other error occured, just ignore it
            { }
        }
    }
}