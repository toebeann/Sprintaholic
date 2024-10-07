using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Tobey.Sprintaholic.SupermarketSimulator;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource? Logger;
    internal static new ConfigFile? Config;

    internal static ConfigEntry<bool>? HoldToSprint;
    internal static ConfigEntry<bool>? AutoDisableSprint;
    internal static ConfigEntry<float>? SpeedMultiplier;
    internal static ConfigEntry<float>? WalkSpeed;
    internal static ConfigEntry<float>? SprintSpeed;

    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;

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

    [HarmonyPatch(typeof(InputActions), nameof(InputActions.OnSprint))]
    [HarmonyPrefix]
    private static bool InputActions_OnSprint(InputActions __instance, InputAction.CallbackContext context)
    {
        var sprint = Traverse.Create(__instance).Field("m_Sprint");

        if (HoldToSprint?.Value is true || // hold to sprint is enabled in config
            !__instance.IsCurrentDeviceMouse || // user is not currently using kbm controls
            !sprint.FieldExists()) // couldn't get current sprint value
        {   // run original method
            return true;
        }
        else
        {   // treat sprint as toggle
            if (context.started)
            {
                __instance.SprintInput(!sprint.GetValue<bool>());
            }

            return false;
        }
        }

    [HarmonyPatch(typeof(FirstPersonController), "Start")]
    [HarmonyPostfix]
    private static void FirstPersonController_Start(FirstPersonController __instance)
    {
        var instance = Traverse.Create(__instance);

        var moveSpeed = instance.Field("MoveSpeed");
        if (moveSpeed.FieldExists())
        {
            WalkSpeed = Config.Bind(
                section: "Movement",
                key: "Walk speed",
                defaultValue: moveSpeed.GetValue<float>(),
                description: "Move speed of the character in m/s");
        }

        var sprintSpeed = instance.Field("SprintSpeed");
        if (sprintSpeed.FieldExists())
        {
            SprintSpeed = Config.Bind(
                section: "Movement",
                key: "Sprint speed",
                defaultValue: sprintSpeed.GetValue<float>(),
                description: "Sprint speed of the character in m/s");
        }
    }

    [HarmonyPatch(typeof(FirstPersonController), "Move")]
    [HarmonyPrefix]
    private static void FirstPersonController_Move_Prefix(FirstPersonController __instance)
    {
        var instance = Traverse.Create(__instance);

        var moveSpeed = instance.Field("MoveSpeed");
        if (moveSpeed.FieldExists())
        {
            moveSpeed.SetValue(WalkSpeed.Value * SpeedMultiplier.Value);
        }

        var sprintSpeed = instance.Field("SprintSpeed");
        if (sprintSpeed.FieldExists())
        {
            sprintSpeed.SetValue(SprintSpeed.Value * SpeedMultiplier.Value);
        }
    }

    private static bool wasMoving;

    [HarmonyPatch(typeof(FirstPersonController), nameof(CharacterController.Move))]
    [HarmonyPrefix]
    private static void FirstPersonController_Move_Postfix(FirstPersonController __instance)
    {
        if (HoldToSprint?.Value is false && // hold to sprint is disabled in config
            AutoDisableSprint?.Value is true) // auto disable sprint is enabled in config
        {
            var _controller = Traverse.Create(__instance).Field("_controller");
            if (!_controller.FieldExists())
            {
                return;
            }

            bool moving = _controller.GetValue<CharacterController>().velocity != Vector3.zero;
            if (!moving && wasMoving) // player has just stopped moving
            {
                InputActions.Instance.SprintInput(false);
            }

            wasMoving = moving;
        }
    }
}
