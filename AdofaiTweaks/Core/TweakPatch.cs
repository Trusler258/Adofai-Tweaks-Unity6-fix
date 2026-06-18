using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Core;

internal class TweakPatch
{
    internal TweakPatch(Type patchType, TweakPatchAttribute metadata, Harmony harmony, Assembly assembly = null) {
        PatchType = patchType;
        Metadata = metadata;
        Harmony = harmony;
        ClassType = (assembly ?? typeof(ADOBase).Assembly).GetType(Metadata.ClassName);
        PatchTargetMethods = ClassType?.GetMethods(AccessTools.all)
            .Where(m => m.Name.Equals(Metadata.MethodName) && !m.IsAbstract);
    }

    private Harmony Harmony { get; }
    private Type ClassType { get; }
    private Type PatchType { get; }
    private IEnumerable<MethodInfo> PatchTargetMethods { get; }
    private readonly string[] _hardcodedMethodNames = ["Prefix", "Postfix", "Transpiler", "Finalizer"];

    internal TweakPatchAttribute Metadata { get; }
    internal bool IsEnabled { get; private set; }

    internal bool IsValidPatch(bool showDebuggingMessage = false) {
        if ((Metadata.MinVersion <= AdofaiTweaks.ReleaseNumber || Metadata.MinVersion == -1) &&
            (Metadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || Metadata.MaxVersion == -1) &&
            ClassType != null &&
            PatchType != null &&
            (PatchTargetMethods?.Count() ?? 0) != 0) {
            return true;
        }

#if DEBUG
        if (showDebuggingMessage) {
            AdofaiTweaks.Logger.Log(
                string.Format(
                    "Patch {0} is inapplicable!\n" +
                    " ├ ClassType is {8} - Expected Not Null\n" +
                    " ├ PatchTargetMethods count is {10}{11} - Expected above 0\n" +
                    " └ Patch target method name is {12}",
                    Metadata.PatchId, Metadata.MinVersion, AdofaiTweaks.ReleaseNumber,
                    Metadata.MinVersion <= AdofaiTweaks.ReleaseNumber || Metadata.MinVersion == -1,
                    Metadata.MinVersion == -1 ? " (-1)" : "",
                    Metadata.MaxVersion,
                    Metadata.MaxVersion >= AdofaiTweaks.ReleaseNumber || Metadata.MaxVersion == -1,
                    Metadata.MaxVersion == -1 ? " (-1)" : "",
                    ClassType,
                    PatchType,
                    PatchTargetMethods?.Count() ?? 0,
                    PatchTargetMethods == null ? " (null)" : "",
                    Metadata.MethodName));
        }
#endif
        return false;
    }

    internal void Patch() {
        if (!IsEnabled) {
#if DEBUG
            AdofaiTweaks.Logger.Log($"Applying patch {Metadata.PatchId}");
#endif
            foreach (MethodInfo method in PatchTargetMethods) {
                List<HarmonyMethod> hardcodedMethods = new List<HarmonyMethod>();
                foreach (string methodName in _hardcodedMethodNames) {
                    MethodInfo patchMethod = AccessTools.Method(PatchType, methodName);
                    hardcodedMethods.Add(patchMethod == null ? null : new HarmonyMethod(patchMethod));
                }
                Harmony.Patch(method, hardcodedMethods[0], hardcodedMethods[1], hardcodedMethods[2], hardcodedMethods[3]);
            }
            IsEnabled = true;
        }
    }

    internal void Unpatch() {
        if (IsEnabled) {
#if DEBUG
            AdofaiTweaks.Logger.Log($"Cancelling patch {Metadata.PatchId}");
#endif
            foreach (MethodInfo original in PatchTargetMethods) {
                foreach (MethodInfo patch in PatchType.GetMethods()) {
                    Harmony.Unpatch(original, patch);
                }
            }
            IsEnabled = false;
        }
    }
}
