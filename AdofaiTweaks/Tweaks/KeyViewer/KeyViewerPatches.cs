using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.KeyViewer;

internal static class KeyViewerPatches
{
    [SyncTweakSettings]
    private static KeyViewerSettings Settings { get; set; }

    // FIXED for Unity 6 / r120+: CountValidKeysPressed moved from scrController to scrPlayer
    [HarmonyPatch(typeof(scrPlayer), "CountValidKeysPressed")]
    private static class CountValidKeysPressedPatch
    {
        [HarmonyBefore("adofai_tweaks.key_limiter")]
        public static bool Prefix(ref int __result) {
            if (Settings.IsListening) {
                __result = 0;
                return false;
            }
            return true;
        }
    }
}
