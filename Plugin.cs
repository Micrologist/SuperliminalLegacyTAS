using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using SuperliminalTAS.Demo;
using SuperliminalTAS.Patches;
using System.Diagnostics;
using System.Reflection;
using BepInEx.Unity.IL2CPP.UnityEngine;


namespace SuperliminalLegacyTAS;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SuperliminalTASPlugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        TimeManagerPatcher.Patch(Process.GetCurrentProcess());
        var rec = AddComponent<DemoRecorder>();
        var hud = AddComponent<DemoHUD>();
    }
}
 