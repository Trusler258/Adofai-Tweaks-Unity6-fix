using System;
using HarmonyLib;

namespace AdofaiTweaks.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class TweakPatchAttribute : Attribute
{
    public string PatchId { get; set; }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public int MinVersion { get; set; }
    public int MaxVersion { get; set; }

    public TweakPatchAttribute(
        string patchId,
        string className,
        string methodName,
        int minVersion = -1,
        int maxVersion = -1) {
        PatchId = patchId;
        ClassName = className;
        MethodName = methodName;
        MinVersion = minVersion;
        MaxVersion = maxVersion;
    }
}
