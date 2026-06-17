using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.HideUiElements;

internal static class HideUiElementsPatches
{
    [SyncTweakSettings] private static HideUiElementsSettings Settings { get; set; }
    private static HideUiElementsProfile SelectedProfile => Settings.RecordingMode ? Settings.RecordingProfile : Settings.PlayingProfile;

    [HarmonyPatch(typeof(scrHitTextMesh), "Show")]
    private static class JudgmentTextShowPatch {
        public static void Prefix(ref Vector3 position) {
            if (SelectedProfile.HideEverything || SelectedProfile.HideJudgment)
                position = new Vector3(123456f, 123456f, 123456f);
        }
    }

    [HarmonyPatch(typeof(scrMissIndicator), "Awake")]
    private static class MissIndicatorPatch {
        public static void Postfix(scrMissIndicator __instance) {
            if (SelectedProfile.HideEverything || SelectedProfile.HideMissIndicators)
                __instance.transform.position = new Vector3(123456f, 123456f, 123456f);
        }
    }

    [HarmonyPatch(typeof(scrShowIfDebug), "Update")]
    private static class HideAutoplayTextPatch {
        private static bool prevVal;
        public static void Prefix() { prevVal = RDC.auto; if (SelectedProfile.HideEverything || SelectedProfile.HideOtto) RDC.auto = false; }
        public static void Postfix() { RDC.auto = prevVal; }
    }

    [HarmonyPatch(typeof(scnEditor), "SwitchToEditMode")]
    private static class HideNoFailAndTimingTargetPatch {
        public static void Postfix() { Settings.ShowOrHideElements(); } // Will be no-op if using minimal settings
    }

    [HarmonyPatch(typeof(scrController), "OnLandOnPortal")]
    private static class HideResultTextPatch {
        public static void Postfix(scrController __instance) {
            if (SelectedProfile.HideEverything || SelectedProfile.HideResult) {
                if (__instance.txtCongrats) __instance.txtCongrats.gameObject.SetActive(false);
                if (__instance.detailedResults) __instance.detailedResults.gameObject.SetActive(false);
                if (__instance.txtAllStrictClear) __instance.txtAllStrictClear.gameObject.SetActive(false);
            }
        }
    }
}
