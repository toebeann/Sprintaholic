using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace Tobey.Sprintaholic.SupermarketSimulator;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;
    internal static new ConfigFile Config;

    internal static ConfigEntry<float> SpeedMultiplier;
    internal static ConfigEntry<float> WalkSpeed;
    internal static ConfigEntry<float> SprintSpeed;

    private void Awake()
    {
        Logger = base.Logger;
        Config = base.Config;

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

        if (__instance.IsCurrentDeviceMouse && sprint.FieldExists())
        {
            if (context.started)
            {
                __instance.SprintInput(!sprint.GetValue<bool>());
            }

            return false;
        }
        else
        {
            return true;
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
    private static void FirstPersonController_Move(FirstPersonController __instance)
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
}
