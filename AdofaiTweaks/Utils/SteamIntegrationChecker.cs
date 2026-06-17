using System.Reflection;
using HarmonyLib;

namespace AdofaiTweaks.Utils;

public static class SteamIntegrationChecker {
    public static bool Check() {
        if (AdofaiTweaks.ReleaseNumber < 131) {
            return OldSteamIntegrationCheck();
        }
        return SteamIntegration.initialized;
    }

    private static readonly FieldInfo SteamIntegrationInstanceField =
        AccessTools.Field(typeof(SteamIntegration), "Instance");
    private static readonly FieldInfo SteamIntegrationInitializedField =
        AccessTools.Field(typeof(SteamIntegration), "initialized");

    private static bool OldSteamIntegrationCheck() {
        var integration = (SteamIntegration)SteamIntegrationInstanceField.GetValue(null);
        if (integration == null) return false;
        return (bool)SteamIntegrationInitializedField.GetValue(integration);
    }
}
