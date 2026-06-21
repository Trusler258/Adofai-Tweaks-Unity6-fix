using System;
using HarmonyLib;

namespace AdofaiTweaks.Tweaks.ChartRendering
{
    /// <summary>
    /// Manages Harmony patching for the ChartRendering module.
    /// These are standalone [HarmonyPatch] classes that aren't managed by TweakRunner.
    /// </summary>
    public static class ChartRenderPatcher
    {
        private static Harmony harmony;
        private static bool isPatched;

        private static readonly Type[] PatchTypes = new[]
        {
            // Auto-player (catches up on beats, suppresses async angle adjustment)
            typeof(ChartRenderConductorUpdatePatch),
            typeof(ChartRenderAsyncAnglePatch),

            // Visual clock (forces songposition to follow audio capture cursor)
            typeof(ChartRenderConductorSongPositionPatch),
            typeof(ChartRenderConductorSongPositionGetterPatch),

            // Audio (suppresses UI sounds during render)
            typeof(ChartRenderAudioPatches),

            // Judgments (toggles hit text display)
            typeof(ChartRenderJudgmentPatches),

            // Editor input blocking (only gameplay input during render)
            typeof(EditorOverlay.EditorOverlayInputBlockPatches.ControllerTogglePausePatch),
            typeof(EditorOverlay.EditorOverlayInputBlockPatches.PlayerManagerInputPatch),
            typeof(EditorOverlay.EditorOverlayInputBlockPatches.PlayerInputTriggeredPatch),
            typeof(EditorOverlay.EditorOverlayInputBlockPatches.PlayerInputReleasedPatch),
            typeof(EditorOverlay.EditorOverlayInputBlockPatches.PlayerInputCountPatch),
        };

        public static void Enable()
        {
            if (isPatched) return;

            try
            {
                harmony = new Harmony("adofai_tweaks.chartrender");
                foreach (Type type in PatchTypes)
                {
                    try
                    {
                        harmony.CreateClassProcessor(type).Patch();
                    }
                    catch (Exception ex)
                    {
                        ChartRenderMain.Log($"Failed to patch {type.Name}: {ex.Message}");
                    }
                }
                isPatched = true;
                ChartRenderMain.Log("Chart render patches enabled (auto-player, visual clock, input block, audio suppression, judgments)");
            }
            catch (Exception ex)
            {
                ChartRenderMain.Log($"Failed to enable chart render patches: {ex}");
            }
        }

        public static void Disable()
        {
            if (!isPatched) return;

            try
            {
                harmony?.UnpatchAll(harmony.Id);
                harmony = null;
                isPatched = false;
                ChartRenderMain.Log("Chart render patches disabled");
            }
            catch (Exception ex)
            {
                ChartRenderMain.Log($"Failed to disable chart render patches: {ex}");
            }
        }
    }
}
