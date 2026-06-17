using AdofaiTweaks.Core.Attributes;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.JudgmentVisuals;

internal static class JudgmentVisualsPatches
{
    [SyncTweakSettings]
    private static JudgmentVisualsSettings Settings { get; set; }

    [HarmonyPatch(typeof(scrPlanet), "SwitchChosen")]
    private static class PlanetSwitchChosenPatch
    {
        public static void Prefix(scrPlanet __instance) {
            var errorMeter = HitErrorMeter.Instance;
            if (!errorMeter) return;

            float angleDiff = (float)(__instance.angle - __instance.targetExitAngle);
            if (!__instance.planetarySystem.isCW) {
                angleDiff *= -1;
            }
            if (RDC.auto) {
                errorMeter.AddHit(0);
            } else {
                errorMeter.AddHit(angleDiff);
            }
        }
    }

    [HarmonyPatch(typeof(scrController), "Awake_Rewind")]
    private static class ControllerAwakeRewindPatch
    {
        public static void Prefix() {
            HitErrorMeter.Instance?.Reset();
        }
    }

    // FIXED for Unity 6 / r120+: ShowHitText moved from scrController to scrHitTextManager
    [HarmonyPatch(typeof(scrHitTextManager), "ShowHitText")]
    private static class ControllerShowHitTextPatch
    {
        public static bool Prefix(HitMargin hitMargin) {
            if (!Settings.HidePerfects) return true;
            return hitMargin != HitMargin.Perfect;
        }
    }
}
