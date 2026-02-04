using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using SprintMode = Tobey.Sprintaholic.Definitions.SprintMode;

namespace Tobey.Sprintaholic.MegastoreSimulator;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
[BepInProcess("Megastore Simulator")]
[BepInProcess("Megastore Simulator Prologue")]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static new ConfigFile Config;

    internal static ConfigEntry<SprintMode> SprintControlMode;
    internal static ConfigEntry<bool> AutoDisableSprint;
    internal static ConfigEntry<bool> SprintByDefault;
    internal static ConfigEntry<bool> InstantAcceleration;
    internal static ConfigEntry<bool> DisableHeadBobbing;
    internal static ConfigEntry<float> SpeedMultiplier;
    internal static ConfigEntry<float> WalkSpeed;
    internal static ConfigEntry<float> SprintSpeed;

    private static bool isSprintToggled;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Config = base.Config;

        Config.ApplyMigrations(MyPluginInfo.PLUGIN_VERSION, Logger);

        SprintControlMode = Config.Bind(Definitions.SprintControlMode);
        AutoDisableSprint = Config.Bind(Definitions.AutoDisableSprint);
        SprintByDefault = Config.Bind(Definitions.SprintByDefault);
        InstantAcceleration = Config.Bind(new ConfigDefinition<bool>(
            Definition: new("Movement", "Instant acceleration"),
            Description: new("Disables the acceleration/deceleration of movement, so that you always move at maximum speed.")));
        DisableHeadBobbing = Config.Bind("Movement", "Disable head bobbing", false);
        SpeedMultiplier = Config.Bind(Definitions.SpeedMultiplier);

        SprintControlMode.SettingChanged += SprintControlMode_SettingChanged;

        Harmony.CreateAndPatchAll(typeof(Plugin), MyPluginInfo.PLUGIN_GUID);
    }

    private void SprintControlMode_SettingChanged(object _, System.EventArgs __) => isSprintToggled = false;

    private void OnDestroy()
    {
        SprintControlMode.SettingChanged -= SprintControlMode_SettingChanged;
        Harmony.UnpatchID(MyPluginInfo.PLUGIN_GUID);
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.IsPressingRun), MethodType.Getter)]
    [HarmonyPostfix]
    private static bool GetIsSprinting(bool __result) =>
        SprintControlMode.Value switch
        {
            SprintMode.Hold => __result, // simply return the original result which evaluates whether the run key is pressed

            SprintMode.Always => true,

            _ => isSprintToggled,
        };

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.Update))]
    [HarmonyPostfix]
    private static void UpdateSprintToggle()
    {
        if (SprintControlMode.Value != SprintMode.Toggle) return;

        var inputManager = SingletonBehaviour<InputManager>.Instance;

        if (AutoDisableSprint.Value && // user wants to automatically stop sprinting when they stop moving
            isSprintToggled && // user is sprinting
            inputManager.MovementInput is (0f, 0f)) // user has released all movement inputs
        {
            isSprintToggled = false; // disable sprint
        }

        if (inputManager.RunActionRef.action.WasPerformedThisFrame())
        {
            isSprintToggled = !isSprintToggled; // toggle sprint
        }
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.Awake))]
    [HarmonyPostfix]
    private static void InitMovementSpeedConfig(float ___walkSpeed, float ___runSpeed)
    {
        WalkSpeed = Config.Bind(Definitions.WalkSpeed, ___walkSpeed);
        SprintSpeed = Config.Bind(Definitions.SprintSpeed, ___runSpeed);
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.SetMovementSpeed))]
    [HarmonyPrefix]
    private static void ModifyBaseMovementSpeed(ref float ___walkSpeed, ref float ___runSpeed)
    {
        ___walkSpeed = SpeedMultiplier.Value * SprintByDefault.Value switch
        {
            true => SprintSpeed?.Value ?? ___runSpeed,
            _ => WalkSpeed?.Value ?? ___walkSpeed,
        };

        ___runSpeed = SpeedMultiplier.Value * SprintByDefault.Value switch
        {
            true => WalkSpeed?.Value ?? ___walkSpeed,
            _ => SprintSpeed?.Value ?? ___runSpeed,
        };
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.SetMovementSpeed))]
    [HarmonyPostfix]
    private static void ApplyInstantAcceleration(float ___walkSpeed, float ___runSpeed, ref float ___movementSpeed)
    {
        if (InstantAcceleration.Value)
        {
            var multiplier = SingletonBehaviour<VehicleManager>.Instance.GetSpeedMultiplier();
            ___movementSpeed = SingletonBehaviour<InputManager>.Instance.IsPressingRun switch
            {
                true => ___runSpeed * multiplier,
                false => ___walkSpeed * multiplier,
            };
        }
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.PlayerMovement))]
    [HarmonyPostfix]
    private static void KillHeadBobbing(Transform ___boxParent, Transform ___charParent)
    {
        if (DisableHeadBobbing.Value)
        {
            ___boxParent.DOKill(false);
            ___charParent.DOKill(false);
        }
    }
}
