using BepInEx;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;
using Il2CppInterop.Runtime.Injection;
using SuperliminalTAS.Demo;
using SuperliminalTAS.Patches;
using System.Diagnostics;
using System.Reflection;


namespace SuperliminalLegacyTAS;

[BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
public class SuperliminalTASPlugin : BasePlugin
{
    internal static new ManualLogSource Log;

    public override void Load()
    {
        Log = base.Log;
        Log.LogInfo($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");

        // Register custom MonoBehaviour types with IL2CPP before they can be used
        ClassInjector.RegisterTypeInIl2Cpp<DemoRecorder>();
        ClassInjector.RegisterTypeInIl2Cpp<DemoHUD>();
        ClassInjector.RegisterTypeInIl2Cpp<ColliderVisualizer>();
        ClassInjector.RegisterTypeInIl2Cpp<PathProjector>();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        TimeManagerPatcher.Patch(Process.GetCurrentProcess());
        var rec = AddComponent<DemoRecorder>();
        var hud = AddComponent<DemoHUD>();
    }
}
 