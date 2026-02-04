using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tobey.Sprintaholic.SupermarketSimulator;

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

        Config.ApplyMigrations(MyPluginInfo.PLUGIN_VERSION, Log);

        HoldToSprint = Config.Bind(Definitions.HoldToSprint);
        AutoDisableSprint = Config.Bind(Definitions.AutoDisableSprint);
        SpeedMultiplier = Config.Bind(Definitions.SpeedMultiplier);

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(InputActions), nameof(InputActions.OnSprint))]
    [HarmonyPrefix]
    private static bool InputActions_OnSprint(InputActions __instance, InputAction.CallbackContext context)
    {
        if (HoldToSprint.Value || // hold to sprint is enabled in config
            !__instance.IsCurrentDeviceMouse) // user is not currently using kbm controls
        {   // run original method
            return true;
        }

        // treat sprint as toggle
        if (context.started)
        {
            __instance.SprintInput(!__instance.m_Sprint);
        }

        return false;
    }

    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Start))]
    [HarmonyPostfix]
    private static void FirstPersonController_Start(FirstPersonController __instance)
    {
        WalkSpeed = Config.Bind(Definitions.WalkSpeed, __instance.MoveSpeed);
        SprintSpeed = Config.Bind(Definitions.SprintSpeed, __instance.SprintSpeed);
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
