using System;
using System.Linq;
using System.Text;
using ADOFAI;
using ADOFAI.Editor.Actions;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Utils;
using HarmonyLib;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.EditorTweaks;

internal static class EditorTweaksPatches
{
    [SyncTweakSettings] private static EditorTweaksSettings Settings { get; set; }

    internal static readonly LevelEventType[] WhitelistedLevelEvents = [
        LevelEventType.MoveTrack, LevelEventType.MoveCamera, LevelEventType.PositionTrack
    ];

    private static scrFloor lastDisplayedFloor;
    private static int updateDisplayFloorFrame = -1;

    [HarmonyPatch(typeof(scnEditor), "Update")]
    private static class RunPerFramePatch {
        private static void Postfix(scnEditor __instance) {
            if (Settings.FineTuneFloorRotations && RDInput.holdingControl && RDInput.holdingAlt) {
                bool cw = Input.GetKeyDown(KeyCode.Period);
                bool ccw = Input.GetKeyDown(KeyCode.Comma);
                if ((cw || ccw) && !__instance.SelectionIsEmpty()) {
                    var levelData = __instance.levelData;
                    float rotAngle = Settings.FloorRotationStep;
                    if (levelData.isOldLevel) {
                        levelData.pathData = levelData.pathData.Remove(__instance.selectedFloors[0].seqID - 1, __instance.selectedFloors.Count);
                        rotAngle = 15;
                    } else {
                        levelData.angleData.RemoveRange(__instance.selectedFloors[0].seqID - 1, __instance.selectedFloors.Count);
                    }
                    foreach (var floor in __instance.selectedFloors) {
                        if (levelData.isOldLevel) {
                            levelData.pathData = levelData.pathData.Insert(floor.seqID - 1,
                                PathDataUtils.GetRotatedPath(floor.stringDirection, cw).ToString());
                        } else {
                            float angle = floor.floatDirection == 999 ? floor.floatDirection :
                                floor.floatDirection + (cw ? -1 : 1) * Settings.FloorRotationStep;
                            levelData.angleData.Insert(floor.seqID - 1, angle);
                        }
                    }
                    __instance.RemakePath();
                }
            }
            if (updateDisplayFloorFrame == Time.frameCount) { updateDisplayFloorFrame = -1; UpdateFloorDisplay(); }
        }
    }

    [HarmonyPatch(typeof(scnEditor), "OnSelectedFloorChange")]
    private static class FloorTextDisplayPatch {
        private static void Postfix(scrFloor ___lastSelectedFloor) => UpdateFloorDisplay(___lastSelectedFloor);
    }

    [HarmonyPatch(typeof(scnEditor), "UpdateFloorCountTexts")]
    private static class FloorTextDisplayAfterPathRemakePatch {
        private static void Postfix() => UpdateFloorDisplay(lastDisplayedFloor);
    }

    private static void UpdateFloorDisplay(scrFloor displayFloor = null) {
        var editor = ADOBase.editor;
        if (!editor || editor.SelectionIsEmpty() || editor.showFloorNums) { if (lastDisplayedFloor && lastDisplayedFloor.editorNumText) lastDisplayedFloor.editorNumText.gameObject.SetActive(false); lastDisplayedFloor = null; return; }
        if (lastDisplayedFloor && lastDisplayedFloor.editorNumText) lastDisplayedFloor.editorNumText.gameObject.SetActive(false);
        displayFloor ??= editor.selectedFloors[0];
        if (!displayFloor || !displayFloor.enabled) displayFloor = editor.selectedFloors[editor.selectedFloors.Count - 1];
        if (!displayFloor || !displayFloor.enabled) return;

        ADOBase.lm.CalculateFloorAngleLengths();
        var sb = new StringBuilder();
        double totalAngle = 0;
        int iterations = editor.selectedFloors.Count;
        if (Settings.UseTulttakModBehavior && iterations > 1) iterations--;

        for (int i = 0; i < iterations; i++) {
            var floor = editor.selectedFloors[i];
            if (floor.seqID == editor.floors.Count - 1) continue;
            float speedFactor = i == 0 ? 1 : editor.selectedFloors[0].speed / floor.speed;
            totalAngle += floor.angleLength * speedFactor * Mathf.Rad2Deg;
        }

        if (totalAngle != 0) {
            if (Settings.ShowFloorAngle) sb.AppendLine($"<color=#ff5252>{totalAngle:#.####}\u00b0</color>");
            if (Settings.ShowFloorBeats) sb.AppendLine($"<color=#52a9ff>{totalAngle / 180:#.####}\u2669</color>");
            if (Settings.ShowFloorCount) sb.AppendLine($"<color=#8a8a8a>{editor.selectedFloors.Count}#</color>");
            if (Settings.ShowFloorDuration) {
                sb.AppendLine($"<color=#ffffff>{totalAngle / (editor.selectedFloors[0].speed * editor.levelData.bpm * 3):0.######}s</color>");
            }
        }

        displayFloor.editorNumText.gameObject.SetActive(true);
        displayFloor.editorNumText.letterText.text = sb.ToString();
        lastDisplayedFloor = displayFloor;
    }
}
