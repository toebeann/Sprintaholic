using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tobey.Sprintaholic.SupermarketSimulator.IL2CPP;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    internal static new ConfigFile Config;

    internal static ConfigEntry<bool> HoldToSprint;
    internal static ConfigEntry<bool> AutoDisableSprint;
    internal static ConfigEntry<float> SpeedMultiplier;
    internal static ConfigEntry<float> WalkSpeed;
    internal static ConfigEntry<float> SprintSpeed;

    public override void Load()
    {
        Log = base.Log;
        Config = base.Config;

        ApplyMigrations(Config);

        HoldToSprint = Config.Bind(
            section: "Controls",
            key: "Hold to sprint",
            defaultValue: false,
            description: "When enabled, hold the sprint keybind to keep sprinting. When disabled, pressing the sprint keybind will cause the player to continue sprinting until they press the sprint keybind again.");

        AutoDisableSprint = Config.Bind(
            section: "Controls",
            key: "Auto disable sprint",
            defaultValue: true,
            description: "When enabled, sprint will automatically be toggled off when you stop moving. Ignored when Hold to sprint is enabled.");

        SpeedMultiplier = Config.Bind(
            section: "Movement",
            key: "Speed multiplier",
            defaultValue: 1f,
            description: "Walk and sprint speed will be multiplied by this number");

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    internal static readonly SortedDictionary<Version, Action<ConfigFile>> migrations = new()
    {
        [new("1.2.0")] = (ConfigFile config) =>
        {
            // v1.2.0 introduces `Controls.Auto disable sprint` config entry, default true to mimic the way the sprint toggle works with gamepad controls
            // Users of Sprintaholic prior to this version will be used to having to disable it manually, so we should default it to false for them to avoid messing with their muscle memory
            config.Bind(
                section: "Controls",
                key: "Auto disable sprint",
                defaultValue: false,
                description: "When enabled, sprint will automatically be toggled off when you stop moving. Ignored when Hold to sprint is enabled.");
        },
    };

    private static void ApplyMigrations(ConfigFile config)
    {
        try
        {
            ReadOnlySpan<string> lines = File.ReadAllLines(config.ConfigFilePath).Where((line) => !string.IsNullOrWhiteSpace(line)).ToArray();

            Version pluginVersion = new(MyPluginInfo.PLUGIN_VERSION);
            Version configVersion = null;

            string searchString = $"## Settings file was created by plugin {MyPluginInfo.PLUGIN_NAME} v";

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].StartsWith(searchString, StringComparison.InvariantCultureIgnoreCase) &&
                    lines[i + 1].Equals($"## Plugin GUID: {MyPluginInfo.PLUGIN_GUID}", StringComparison.InvariantCultureIgnoreCase))
                {
                    configVersion = new(lines[i][searchString.Length..]);
                    break;
                }
            }

            if (configVersion is null || // couldn't parse version from config file, treat as latest
                configVersion >= pluginVersion) // config version is same or newer than plugin version
            {   // no need to apply migrations
                return;
            }

            // define migrations
            var migratedVersion = configVersion;
            config.SaveOnConfigSet = false;
            foreach (var migration in migrations.SkipWhile((kvp) => kvp.Key <= configVersion))
            {
                Log.LogInfo($"Applying config migration v{migratedVersion} -> v{migration.Key}");
                migration.Value(config);
                migratedVersion = migration.Key;
            }
            config.SaveOnConfigSet = true;
        }
        catch (FileNotFoundException) // config file doesn't exist, no need to apply migrations
        { }
        catch // some other error occured, probably best to act as if config file doesn't exist
        { }
    }

    [HarmonyPatch(typeof(InputActions), nameof(InputActions.OnSprint))]
    [HarmonyPrefix]
    private static bool InputActions_OnSprint(InputActions __instance, InputAction.CallbackContext context)
    {
        if (HoldToSprint.Value is true || // hold to sprint is enabled in config
            !__instance.IsCurrentDeviceMouse) // user is not currently using kbm controls
        {   // run original method
            return true;
        }
        else
        {   // treat sprint as toggle
            if (context.started)
            {
                __instance.SprintInput(!__instance.m_Sprint);
            }

            return false;
        }
    }

    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Start))]
    [HarmonyPostfix]
    private static void FirstPersonController_Start(FirstPersonController __instance)
    {
        WalkSpeed = Config.Bind(
            section: "Movement",
            key: "Walk speed",
            defaultValue: __instance.MoveSpeed,
            description: "Move speed of the character in m/s");

        SprintSpeed = Config.Bind(
            section: "Movement",
            key: "Sprint speed",
            defaultValue: __instance.SprintSpeed,
            description: "Sprint speed of the character in m/s");
    }

    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Move))]
    [HarmonyPrefix]
    private static void FirstPersonController_Move_Prefix(FirstPersonController __instance)
    {
        __instance.MoveSpeed = (WalkSpeed?.Value ?? __instance.MoveSpeed) * SpeedMultiplier.Value;
        __instance.SprintSpeed = (SprintSpeed?.Value ?? __instance.SprintSpeed) * SpeedMultiplier.Value;
    }

    private static bool wasMoving;

    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Move))]
    [HarmonyPostfix]
    private static void FirstPersonController_Move_Postfix(FirstPersonController __instance)
    {
        if (HoldToSprint.Value is false && // hold to sprint is disabled in config
            AutoDisableSprint.Value is true) // auto disable sprint is enabled in config
        {
            bool isMoving = __instance._controller.velocity != Vector3.zero;
            if (!isMoving && wasMoving)
            {
                __instance.m_InputActions.SprintInput(false);
            }

            wasMoving = isMoving;
        }
    }
}
