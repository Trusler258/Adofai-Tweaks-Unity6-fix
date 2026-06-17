using System;

namespace AdofaiTweaks.Core.Attributes;

public class RegisterTweakAttribute : Attribute
{
    public string Id { get; private set; }
    public int Priority { get; private set; }
    public Type SettingsType { get; private set; }
    public Type PatchesType { get; private set; }

    public RegisterTweakAttribute(
        string id, Type settingsType, Type patchesType, int priority = 0) {
        Id = id;
        Priority = priority;
        SettingsType = settingsType;
        PatchesType = patchesType;
    }
}
