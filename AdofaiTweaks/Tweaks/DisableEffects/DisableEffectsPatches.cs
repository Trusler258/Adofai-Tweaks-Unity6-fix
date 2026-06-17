using System;
using System.Collections.Generic;
using System.Reflection;
using AdofaiTweaks.Core.Attributes;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.DisableEffects;

internal static class DisableEffectsPatches
{
    private static FieldInfo filterToCompFieldInfo;

    [SyncTweakSettings] private static DisableEffectsSettings Settings { get; set; }

    static DisableEffectsPatches() {
        filterToCompFieldInfo = AdofaiTweaks.ReleaseNumber <= 82
            ? AccessTools.Field(typeof(scrController), "filterToComp")
            : AccessTools.Field(typeof(scrVfxPlus), "filterToComp");
    }

    [TweakPatch("DisableEffects.SetFilterAlwaysFalse", "ffxSetFilterPlus", "SetFilter")]
    private static class SetFilterAlwaysFalsePatch {
        public static void Prefix(ffxSetFilterPlus __instance, ref bool fEnable) {
            if (Settings.DisableFilter && !Settings.FilterExcludeList.Contains(__instance.filter)) fEnable = false;
        }
    }

    [TweakPatch("DisableEffects.BloomStartEffectDoNothing", "ffxPlusBase", "StartEffect")]
    private static class BloomStartEffectDoNothingPatch {
        public static bool Prefix(ffxPlusBase __instance) {
            if (__instance is ffxBloomPlus && Settings.DisableBloom) return false;
            return true;
        }
    }

    [TweakPatch("DisableEffects.FlashStartEffectAlwaysClear", "ffxPlusBase", "StartEffect")]
    private static class FlashStartEffectAlwaysClearPatch {
        public static void Prefix(ffxPlusBase __instance) {
            if (__instance is ffxFlashPlus flash && Settings.DisableFlash) {
                flash.startColor = Color.clear; flash.endColor = Color.clear;
            }
        }
    }

    [TweakPatch("DisableEffects.FlashScrubToTimeAlwaysClear", "ffxPlusBase", "ScrubToTime")]
    private static class FlashScrubToTimeAlwaysClearPatch {
        public static void Prefix(ffxPlusBase __instance) {
            if (__instance is ffxFlashPlus flash && Settings.DisableFlash) {
                flash.startColor = Color.clear; flash.endColor = Color.clear;
            }
        }
    }

    [TweakPatch("DisableEffects.ShakeScreenStartEffectAlwaysDisabled", "ffxPlusBase", "StartEffect")]
    private static class ShakeScreenStartEffectAlwaysDisabledPatch {
        public static bool Prefix(ffxPlusBase __instance) {
            if (__instance is ffxShakeScreenPlus && Settings.DisableScreenShake) return false;
            return true;
        }
    }

    [TweakPatch("DisableEffects.ShakeScreenScrubToTimeAlwaysDisabled", "ffxPlusBase", "ScrubToTime")]
    private static class ShakeScreenScrubToTimeAlwaysDisabledPatch {
        public static bool Prefix(ffxPlusBase __instance) {
            if (__instance is ffxShakeScreenPlus && Settings.DisableScreenShake) return false;
            return true;
        }
    }

    [TweakPatch("DisableEffects.ControllerDisableStartVfx", "scrController", "WaitForStartCo")]
    private static class ControllerDisableStartVfxPatch {
        public static void Postfix(scrController __instance) {
            if (!Settings.DisableFilter) return;
            var filterToComp = (Dictionary<Filter, MonoBehaviour>)(AdofaiTweaks.ReleaseNumber <= 82
                ? filterToCompFieldInfo.GetValue(__instance)
                : filterToCompFieldInfo.GetValue(scrVfxPlus.instance));
            foreach (var behavior in filterToComp.Values) behavior.enabled = false;
        }
    }

    [TweakPatch("DisableEffects.HomDisabledPost105", "ffxPlusBase", "StartEffect", MinVersion = 105)]
    private static class HomStartEffectDisabledPatch {
        public static void Postfix(ffxPlusBase __instance) {
            if (__instance is ffxHallOfMirrorsPlus hom && Settings.DisableHallOfMirrors)
                hom.cam.Bgcamstatic.clearFlags = CameraClearFlags.Color;
        }
    }

    [TweakPatch("DisableEffects.MoveFloorLimitRange", "ffxPlusBase", "StartEffect")]
    private static class MoveFloorStartEffectLimitRangePatch {
        private static int origStart, origEnd;
        public static void Prefix(ffxPlusBase __instance) {
            if (__instance is not ffxMoveFloorPlus move || Settings.MoveTrackMax > 500) return;
            int index = scrController.instance.currFloor.seqID;
            origStart = move.start; origEnd = move.end;
            move.start = Math.Max(index - Settings.MoveTrackMax / 2, origStart);
            move.end = Math.Min(index + Settings.MoveTrackMax / 2, origEnd);
        }
        public static void Postfix(ffxPlusBase __instance) {
            if (__instance is not ffxMoveFloorPlus move || Settings.MoveTrackMax > 500) return;
            move.start = origStart; move.end = origEnd;
        }
    }
}
