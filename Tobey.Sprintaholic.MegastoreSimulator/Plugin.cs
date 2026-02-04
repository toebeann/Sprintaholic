using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Tobey.Sprintaholic.Config;

namespace Tobey.Sprintaholic.MegastoreSimulator;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static new ConfigFile Config;

    internal static ConfigEntry<bool> HoldToSprint;
    internal static ConfigEntry<bool> AutoDisableSprint;
    internal static ConfigEntry<float> SpeedMultiplier;
    internal static ConfigEntry<float> WalkSpeed;
    internal static ConfigEntry<float> SprintSpeed;

    private static bool isSprinting;

    private void Awake()
    {
        // Plugin startup logic
        Logger = base.Logger;
        Config = base.Config;

        Config.ApplyMigrations(MyPluginInfo.PLUGIN_VERSION, Logger);

        HoldToSprint = Config.Bind(Definitions.HoldToSprint);
        AutoDisableSprint = Config.Bind(Definitions.AutoDisableSprint);
        SpeedMultiplier = Config.Bind(Definitions.SpeedMultiplier);

        HoldToSprint.SettingChanged += (_, __) => isSprinting = false;

        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(InputManager), nameof(InputManager.IsPressingRun), MethodType.Getter)]
    [HarmonyPostfix]
    private static bool GetIsSprinting(bool __result)
    {
        if (HoldToSprint.Value) // hold to sprint is enabled in config
        {   // return original method result, which simply evaluates to whether the user is currently pressing the run key
            return __result;
        }

        // treat sprint as a toggle
        return isSprinting;
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.Update))]
    [HarmonyPostfix]
    private static void UpdateIsSprinting()
    {
        if (SingletonBehaviour<InputManager>.Instance.RunActionRef.action.WasPerformedThisFrame())
        {
            isSprinting = !isSprinting;
        }
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.Awake))]
    [HarmonyPostfix]
    private static void InitialiseMovementSpeedConfig(float ___walkSpeed, ref float ___runSpeed)
    {
        WalkSpeed = Config.Bind(Definitions.WalkSpeed, ___walkSpeed);
        SprintSpeed = Config.Bind(Definitions.SprintSpeed, ___runSpeed);
    }

    [HarmonyPatch(typeof(PlayerMove), nameof(PlayerMove.SetMovementSpeed))]
    [HarmonyPrefix]
    private static void UpdateMovementSpeedFromConfig(ref float ___walkSpeed, ref float ___runSpeed)
    {
        ___walkSpeed = (WalkSpeed?.Value ?? ___walkSpeed) * SpeedMultiplier.Value;
        ___runSpeed = (SprintSpeed?.Value ?? ___runSpeed) * SpeedMultiplier.Value;
    }
}
