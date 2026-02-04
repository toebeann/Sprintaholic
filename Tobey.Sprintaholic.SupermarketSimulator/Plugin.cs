using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;
using SprintMode = Tobey.Sprintaholic.Definitions.SprintMode;

namespace Tobey.Sprintaholic.SupermarketSimulator;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Supermarket Simulator")]
public class Plugin : BasePlugin
{
    internal static new ManualLogSource Log;
    internal static new ConfigFile Config;

    internal static ConfigEntry<SprintMode> SprintControlMode;
    internal static ConfigEntry<bool> AutoDisableSprint;
    internal static ConfigEntry<bool> SprintByDefault;
    internal static ConfigEntry<float> SpeedMultiplier;
    internal static ConfigEntry<float> WalkSpeed;
    internal static ConfigEntry<float> SprintSpeed;

    public override void Load()
    {
        Log = base.Log;
        Config = base.Config;

        Config.ApplyMigrations(MyPluginInfo.PLUGIN_VERSION, Log);

        SprintControlMode = Config.Bind(Definitions.SprintControlMode);
        AutoDisableSprint = Config.Bind(Definitions.AutoDisableSprint);
        SprintByDefault = Config.Bind(Definitions.SprintByDefault);
        SpeedMultiplier = Config.Bind(Definitions.SpeedMultiplier);

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(InputActions), nameof(InputActions.OnSprint))]
    [HarmonyPrefix]
    private static bool InputActions_OnSprint(InputActions __instance, InputAction.CallbackContext context)
    {
        if (SprintControlMode.Value == SprintMode.Hold ||
            !__instance.IsCurrentDeviceMouse) // user is not currently using kbm controls
        {   // run original method
            return true;
        }

        switch (SprintControlMode.Value)
        {
            case SprintMode.Toggle:
                if (context.started)
                    __instance.SprintInput(!__instance.m_Sprint);
                break;

            case SprintMode.Always
            when !__instance.m_Sprint:
                __instance.SprintInput(true);
                break;
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
        __instance.MoveSpeed = SpeedMultiplier.Value * SprintByDefault.Value switch
        {
            true => SprintSpeed?.Value ?? __instance.SprintSpeed,
            _ => WalkSpeed?.Value ?? __instance.MoveSpeed,
        };

        __instance.SprintSpeed = SpeedMultiplier.Value * SprintByDefault.Value switch
        {
            true => WalkSpeed?.Value ?? __instance.MoveSpeed,
            _ => SprintSpeed?.Value ?? __instance.SprintSpeed,
        }; ;
    }

    private static bool wasMoving;

    [HarmonyPatch(typeof(FirstPersonController), nameof(FirstPersonController.Move))]
    [HarmonyPostfix]
    private static void FirstPersonController_Move_Postfix(FirstPersonController __instance)
    {
        switch (SprintControlMode.Value)
        {
            case SprintMode.Toggle
            when AutoDisableSprint.Value:
                bool isMoving = __instance._controller.velocity != Vector3.zero;
                if (!isMoving && wasMoving)
                {
                    __instance.m_InputActions.SprintInput(false);
                }

                wasMoving = isMoving;
                break;

            case SprintMode.Always
            when !__instance.m_InputActions.m_Sprint:
                __instance.m_InputActions.SprintInput(true);
                break;
        }
    }
}
