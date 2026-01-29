using HarmonyLib;
using System;
using System.Reflection;
using UnityEngine;
using Random = UnityEngine.Random;

namespace SuperliminalTAS.Patches;

#region Rewired Input Patches

[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButton))]
[HarmonyPatch([typeof(string)])]
public class GetButtonPatch
{
    static void Postfix(string actionName, ref bool __result)
    {
        __result = TASInput.GetButton(actionName, __result);
    }
}

[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonDown))]
[HarmonyPatch([typeof(string)])]
public class GetButtonDownPatch
{
    static void Postfix(string actionName, ref bool __result)
    {
        __result = TASInput.GetButtonDown(actionName, __result);
    }
}

[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetButtonUp))]
[HarmonyPatch([typeof(string)])]
public class GetButtonUpPatch
{
    static void Postfix(string actionName, ref bool __result)
    {
        __result = TASInput.GetButtonUp(actionName, __result);
    }
}

[HarmonyPatch(typeof(Rewired.Player), nameof(Rewired.Player.GetAxis))]
[HarmonyPatch([typeof(string)])]
public class GetAxisPatch
{
    static void Postfix(string actionName, ref float __result)
    {
        __result = TASInput.GetAxis(actionName, __result);
    }
}

// TODO: The obfuscated Rewired method name below was for the Mono version.
// For the legacy IL2CPP build, the obfuscated names will differ.
// This patch needs updating with the correct IL2CPP method target.
// Also, "unscaledDeltaTime" may be exposed as a property rather than a field in IL2CPP.
/*
[HarmonyPatch]
public class RewiredDeltaTimePatch
{
    private static MemberInfo memberInfo;

    static MethodBase TargetMethod()
    {
        // IL2CPP: Obfuscated names differ from Mono version - this needs the correct IL2CPP target
        var method = AccessTools.Method(
            "Rewired.ReInput+YADQJtjjsJnFpIRXsWZbJAPaLFd+WJEYbcppSteIUfAbygiJwLxOmigk:AgiDzcfIDplWkWrlRVaBMFfZJjUH");
        if (method == null)
            Debug.LogWarning("[TAS] RewiredDeltaTimePatch: Could not find target method. Obfuscated name may differ in IL2CPP build.");
        return method;
    }

    static void Postfix()
    {
        if (memberInfo == null)
        {
            // IL2CPP: Try field first, then property
            memberInfo = (MemberInfo)AccessTools.Field(typeof(Rewired.ReInput), "unscaledDeltaTime")
                      ?? AccessTools.Property(typeof(Rewired.ReInput), "unscaledDeltaTime");
        }

        if (memberInfo is FieldInfo fi)
            fi.SetValue(null, Time.fixedDeltaTime);
        else if (memberInfo is PropertyInfo pi)
            pi.SetValue(null, Time.fixedDeltaTime);
    }
}

*/
#endregion

#region RNG Patches

[HarmonyPatch(typeof(Random), nameof(Random.onUnitSphere))]
[HarmonyPatch(MethodType.Getter)]
public class OnUnitSpherePatch
{
    static void Postfix(ref Vector3 __result)
    {
        __result = Vector3.up;
    }
}

[HarmonyPatch(typeof(Random), nameof(Random.insideUnitSphere))]
[HarmonyPatch(MethodType.Getter)]
public class InUnitSpherePatch
{
    static void Postfix(ref Vector3 __result)
    {
        __result = Vector3.zero;
    }
}

[HarmonyPatch(typeof(Random), nameof(Random.value))]
[HarmonyPatch(MethodType.Getter)]
public class ValuePatch
{
    static void Postfix(ref float __result)
    {
        __result = .5f;
    }
}

[HarmonyPatch(typeof(Random), nameof(Random.Range))]
[HarmonyPatch([typeof(int), typeof(int)])]
public class RandomRangeIntPatch
{
    static void Postfix(int min, int max, ref int __result)
    {
        __result = Mathf.FloorToInt((min + max) / 2f);
    }
}

[HarmonyPatch(typeof(Random), nameof(Random.Range))]
[HarmonyPatch([typeof(float), typeof(float)])]
public class RandomRangeFloatPatch
{
    static void Postfix(float min, float max, ref float __result)
    {
        __result = (min + max) / 2f;
    }
}

// TODO: System.Random.InternalSample is a .NET implementation detail that may not exist
// in the IL2CPP runtime. If this patch fails, try patching Il2CppSystem.Random.Sample()
// or Il2CppSystem.Random.Next() instead.
/*
[HarmonyPatch(typeof(System.Random), "InternalSample")]
public class InternalSamplePatch
{
    static void Postfix(ref int __result)
    {
        __result = 0;
    }
}
*/

#endregion



[HarmonyPatch(typeof(PlayerLerpMantle), "LerpPlayer")]
public class LerpPlayerMantlePatch
{
    static void Prefix()
    {
        Debug.Log(Time.time + ": LerpPlayer()");
    }
}

[HarmonyPatch(typeof(SaveAndCheckpointManager), "_SaveGame", [typeof(CheckPoint)])]
public class SaveGamePatch
{
    public static CheckPoint lastCheckpoint;

    static void Prefix(CheckPoint checkpoint)
    {
        Debug.Log(Time.time + ": _SaveGame() " + checkpoint?.name);
        lastCheckpoint = checkpoint;
    }
}

[HarmonyPatch(typeof(UnityEngine.Time), nameof(UnityEngine.Time.time))]
[HarmonyPatch(MethodType.Getter)]
public class TimeTimePatch
{
    static void Postfix(ref float __result)
    {
        __result = Time.timeSinceLevelLoad;
    }
}

// TODO: ref Il2CppSystem.Nullable<int> causes AccessViolationException in the
// il2cpp_value_box trampoline. HarmonyX cannot safely marshal ref nullable value
// types in IL2CPP. Needs a native patch or alternative approach (e.g. patching a
// caller instead, or using a transpiler to avoid the ref parameter).
/*
[HarmonyPatch(typeof(LevelInformation), nameof(LevelInformation.GetLoadingSceneIndex))]
[HarmonyPatch([typeof(string), typeof(Il2CppSystem.Nullable<int>)])]
public class LoadingScenePatch
{
    static void Prefix(string scenePath, ref Il2CppSystem.Nullable<int> debugOverride)
    {
        debugOverride = (Il2CppSystem.Nullable<int>)(-1);
    }
}
*/

[HarmonyPatch(typeof(FMODUnity.StudioEventEmitter),"OnEnable")]
public class EventEmitterPlayPatch
{
    static void Prefix(FMODUnity.StudioEventEmitter __instance)
    {
        if(__instance.name == "AlarmSound")
        {
            GameObject.Destroy(__instance.gameObject);
        }
        return;
    }
}

[HarmonyPatch(typeof(UnityEngine.Collider), nameof(UnityEngine.Collider.enabled))]
[HarmonyPatch(MethodType.Setter)]
public class ColliderEnabledPatch
{
    static void Prefix(ref bool value, UnityEngine.Collider __instance)
    {
        var name = __instance.gameObject.name;
        if (name == "SecretTriggerObject") return;
        //Debug.Log($"{Time.time}: {name} collider enabled set to {value}");
    }
}

[HarmonyPatch(typeof(UnityEngine.Collider), nameof(UnityEngine.Collider.isTrigger))]
[HarmonyPatch(MethodType.Setter)]
public class ColliderIsTriggerPatch
{
    static void Prefix(ref bool value, UnityEngine.Collider __instance)
    {
        //Debug.Log($"{Time.time}: {__instance.gameObject.name} isTrigger set to {value}");
    }
}

[HarmonyPatch(typeof(UnityEngine.Rigidbody), nameof(UnityEngine.Rigidbody.isKinematic))]
[HarmonyPatch(MethodType.Setter)]
public class GBKinematicPatch
{
    static void Prefix(ref bool value, UnityEngine.Rigidbody __instance)
    {
        //Debug.Log($"{Time.time}: {__instance.gameObject.name} isKinematic set to {value}");
    }
}

/*
[HarmonyPatch(typeof(WarningController), "Start")]
public class WarningScreenPatch
{
    static void Prefix()
    {
        // IL2CPP: Field may be exposed as a property by Il2CppInterop
        var member = (MemberInfo)AccessTools.Field(typeof(WarningController), "ShowedWarning")
                  ?? AccessTools.Property(typeof(WarningController), "ShowedWarning");

        if (member is FieldInfo fi)
            fi.SetValue(null, true);
        else if (member is PropertyInfo pi)
            pi.SetValue(null, true);
        else
            Debug.LogWarning("[TAS] WarningScreenPatch: Could not find ShowedWarning member on WarningController");
    }
}
*/
