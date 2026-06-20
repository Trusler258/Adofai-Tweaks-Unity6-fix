using AdofaiTweaks.Core;
using AdofaiTweaks.Core.Attributes;
using AdofaiTweaks.Strings;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.Miscellaneous;

/// <summary>
/// A tweak for holding miscellaneous toggles.
/// </summary>
[RegisterTweak(
    id: "miscellaneous",
    settingsType: typeof(MiscellaneousSettings),
    patchesType: typeof(MiscellaneousPatches),
    priority: 1000)]
public class MiscellaneousTweak : Tweak
{
    private bool chartRenderInitialized;

    /// <inheritdoc/>
    public override string Name =>
        TweakStrings.Get(TranslationKeys.Miscellaneous.NAME);

    /// <inheritdoc/>
    public override string Description =>
        TweakStrings.Get(TranslationKeys.Miscellaneous.DESCRIPTION);

    [SyncTweakSettings]
    private MiscellaneousSettings Settings { get; set; }

#if DEBUG
    private string bpmString = "";
#endif

    /// <inheritdoc/>
    public override void OnSettingsGUI() {
        // Glitch flip
        Settings.DisableGlitchFlip =
            GUILayout.Toggle(
                Settings.DisableGlitchFlip,
                TweakStrings.Get(
                    TranslationKeys.Miscellaneous.GLITCH_FLIP,
                    RDString.GetEnumValue(Filter.Glitch)));

        // Editor zoom
        Settings.DisableEditorZoom =
            GUILayout.Toggle(
                Settings.DisableEditorZoom,
                TweakStrings.Get(TranslationKeys.Miscellaneous.EDITOR_ZOOM));

        // Hitsound volume
        if (Settings.SetHitsoundVolume =
            GUILayout.Toggle(
                Settings.SetHitsoundVolume,
                TweakStrings.Get(TranslationKeys.Miscellaneous.SET_HITSOUND_VOLUME))) {
            MoreGUILayout.BeginIndent();

            // Hitsound volume slider
            bool valueChanged = Settings.HitsoundVolumeScale == (
                Settings.HitsoundVolumeScale =
                    Mathf.Min(
                        MoreGUILayout.NamedSlider(
                            TweakStrings.Get(TranslationKeys.Miscellaneous.CURRENT_HITSOUND_VOLUME),
                            Settings.HitsoundVolumeScale * 100,
                            0,
                            100f,
                            200f,
                            roundNearest: 1f,
                            valueFormat: "{0:0.#}%") / 100, 1));

            if (valueChanged) {
                Settings.UpdateVolume();
            }

            // Force volume checkbox
            Settings.HitsoundForceVolume =
                GUILayout.Toggle(
                    Settings.HitsoundForceVolume,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.HITSOUND_FORCE_VOLUME));

            // Ignore first value checkbox
            Settings.HitsoundIgnoreStartingValue =
                GUILayout.Toggle(
                    Settings.HitsoundIgnoreStartingValue,
                    TweakStrings.Get(TranslationKeys.Miscellaneous.HITSOUND_IGNORE_STARTING_VALUE));

            MoreGUILayout.EndIndent();
        }

        if (GameVersionState.OldAsyncInputAvailable) {
            // Fix settings state and async input enabled status desync
            Settings.SyncInputStateToInputOptions = GUILayout.Toggle(
                Settings.SyncInputStateToInputOptions,
                TweakStrings.Get(TranslationKeys.Miscellaneous.SYNC_INPUT_STATE));
        }

        // Chart rendering — lazy-init on reload if already enabled
        if (!chartRenderInitialized && Settings.EnableChartRendering) {
            chartRenderInitialized = true;
            ChartRendering.ChartRenderMain.Settings = ChartRendering.ChartRenderingSettings.Load(ChartRendering.ChartRenderMain.Mod);
            ChartRendering.ChartRenderMain.Settings.EnsureDefaults(ChartRendering.ChartRenderMain.Mod);
            ChartRendering.ChartRenderMain.Settings.ShowEditorOverlay = true;
            ChartRendering.ChartRenderMain.IsZh = AdofaiTweaks.GlobalSettings.Language.ToString() == "CHINESE_SIMPLIFIED";
            ChartRendering.EditorOverlay.EditorTweaksOverlayWindow.Ensure();
            ChartRendering.ChartRenderPatcher.Enable();
            ChartRendering.ChartRenderMain.Log("Chart rendering re-initialized on reload");
        }

        // Chart rendering
        bool newChartRender = GUILayout.Toggle(
            Settings.EnableChartRendering,
            "启用谱面视频渲染 (Chart Rendering)");
        if (newChartRender != Settings.EnableChartRendering) {
            Settings.EnableChartRendering = newChartRender;
            if (newChartRender) {
                ChartRendering.ChartRenderMain.Settings = ChartRendering.ChartRenderingSettings.Load(ChartRendering.ChartRenderMain.Mod);
                ChartRendering.ChartRenderMain.Settings.EnsureDefaults(ChartRendering.ChartRenderMain.Mod);
                ChartRendering.ChartRenderMain.Settings.ShowEditorOverlay = true;
                ChartRendering.ChartRenderMain.IsZh = AdofaiTweaks.GlobalSettings.Language.ToString() == "CHINESE_SIMPLIFIED";
                ChartRendering.EditorOverlay.EditorTweaksOverlayWindow.Ensure();
                ChartRendering.ChartRenderPatcher.Enable();
                chartRenderInitialized = true;
                ChartRendering.ChartRenderMain.Log("Chart rendering enabled: patches active, overlay visible");
            } else {
                ChartRendering.ChartRenderPatcher.Disable();
                ChartRendering.EditorOverlay.EditorTweaksOverlayWindow.Destroy();
                chartRenderInitialized = false;
                ChartRendering.ChartRenderMain.Log("Chart rendering disabled");
            }
        }

#if DEBUG
        // Test feature
        if (Settings.SetBpmOnStart =
            GUILayout.Toggle(
                Settings.SetBpmOnStart,
                TweakStrings.Get(TranslationKeys.Global.TEST_KEY))) {
            bpmString = MoreGUILayout.NamedTextField(TranslationKeys.Global.TEST_KEY, bpmString, 200f);

            if (float.TryParse(bpmString, out float bpm)) {
                Settings.Bpm = bpm;
            }
        }
#endif
    }

#if DEBUG
    /// <inheritdoc/>
    public override void OnEnable() {
        bpmString = Settings.Bpm.ToString();
    }
#endif
}