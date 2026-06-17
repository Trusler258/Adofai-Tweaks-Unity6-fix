using System.Collections.Generic;
using ADOFAI;
using AdofaiTweaks.Core.Attributes;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous;

internal static class MiscellaneousPatches
{
    [SyncTweakSettings]
    private static MiscellaneousSettings Settings { get; set; }

    [HarmonyPatch(typeof(CameraFilterPack_FX_Glitch1), "OnRenderImage")]
    private static class GlitchOnRenderImagePatch
    {
        private static readonly float[,] RANGES = {
            { 9f, 9.125f }, { 11.125f, 11.25f }, { 23.25f, 23.735f }, { 39f, 39.125f },
            { 45.25f, 45.375f }, { 78.625f, 78.75f }, { 78.875f, 79f }, { 87.75f, 87.875f },
        };
        public static void Prefix(ref float ___TimeX) {
            if (!Settings.DisableGlitchFlip) return;
            ___TimeX += Time.deltaTime;
            for (int i = 0; i < RANGES.GetLength(0); i++) {
                if (RANGES[i, 0] - 0.01 <= ___TimeX && ___TimeX < RANGES[i, 1] + 0.01f) ___TimeX += 0.15f;
            }
            ___TimeX -= Time.deltaTime;
        }
    }

    [TweakPatch("Miscellaneous.RemoveZoomPostTweenPatch", "scnEditor", "Update", minVersion: 108)]
    private static class RemoveZoomPostTweenPatch {
        public static void Postfix() {
            if (!Settings.DisableEditorZoom || !scnEditor.instance.playMode) return;
            if (Mathf.Abs(Input.mouseScrollDelta.y) > 0.05f) {
                float orig = scrCamera.instance.userSizeMultiplier;
                DOTween.To(() => scrCamera.instance.userSizeMultiplier, x => scrCamera.instance.userSizeMultiplier = orig, orig, 0.1f)
                    .SetEase(Ease.OutQuad).SetUpdate(true);
            }
        }
    }

    [TweakPatch("Miscellaneous.ForceHitsoundVolumePostScnGamePatch", "scnGame", "ApplyEvent", minVersion: 108)]
    private static class ForceHitsoundVolumePostScnGamePatch {
        public static void Postfix(ref LevelEvent evnt, ref List<scrFloor> floors) {
            if (evnt.eventType == LevelEventType.SetHitsound && Settings.IsEnabled && Settings.SetHitsoundVolume) {
                var ffxList = floors[evnt.floor].gameObject.GetComponentsInChildren<ffxSetHitsound>();
                if (ffxList != null) foreach (var ffx in ffxList) Settings.UpdateVolume(ffx);
            }
        }
    }

    [TweakPatch("Miscellaneous.EnforceAsyncInputStateAtControllerStart", "scrController", "Start", minVersion: 97)]
    private static class EnforceAsyncInputStateAtControllerStartPatch {
        public static void Postfix() {
            if (Settings.SyncInputStateToInputOptions) {
                AsyncInputManager.ToggleHook(Persistence.GetChosenAsynchronousInput());
            }
        }
    }
}
