using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using UnityEngine.InputSystem;

namespace Tobey.Sprintaholic.SupermarketSimulator;
[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin
{
    internal static new ManualLogSource Logger;

    private void Awake()
    {
        Logger = base.Logger;
        Harmony.CreateAndPatchAll(typeof(Plugin));
    }

    [HarmonyPatch(typeof(InputActions), nameof(InputActions.OnSprint))]
    private static bool Prefix(InputActions __instance, InputAction.CallbackContext context)
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
}
