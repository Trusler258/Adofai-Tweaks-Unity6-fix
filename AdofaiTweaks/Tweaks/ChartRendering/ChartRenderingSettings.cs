using System.Collections.Generic;
using System.IO;
using System;
using System.Xml.Serialization;
using UnityModManagerNet;

namespace AdofaiTweaks.Tweaks.ChartRendering;

/// <summary>
/// Chart rendering settings with defaults optimized for reasonable file sizes.
/// </summary>
public class ChartRenderingSettings
{
    // --- Editor overlay ---
    public bool ShowEditorOverlay = true;
    public bool EditorOverlayCollapsed = false;
    public float EditorOverlayX = -1f;
    public float EditorOverlayY = -1f;

    // --- Editor preferences ---
    public bool PersistEditorPreferences = true;

    // --- Numeric editing (from EditorTweaks overlay) ---
    public float DecorationMoveSnapStep = 0.5f;
    public float FloatStepPerPixel = 0.1f;
    public float IntStepPerPixel = 1f;
    public int MaxFloatingPoints = 3;

    // --- Chart rendering ---
    public string ChartRenderWorkspaceDirectory = string.Empty;
    public string ChartRenderExportDirectory = string.Empty;
    public int ChartRenderWidth = 1920;
    public int ChartRenderHeight = 1080;
    public int ChartRenderFps = 60;
    /// <summary>CRF/QP value, default 23 (was 18 in original — too high quality/unnecessary bitrate)</summary>
    public int ChartRenderCrf = 23;
    public string ChartRenderPreset = "veryfast";
    public string ChartRenderEncoderMode = "auto-balanced";
    public string ChartRenderCaptureFormat = "rgba";
    public string ChartRenderPreviewMode = "full";
    public float ChartRenderCompletionTailSeconds = 5f;
    public float ChartRenderAudioSyncOffsetMs = 0f;
    public bool ChartRenderShowHitJudgments = true;
    public bool ChartRenderAdvancedSettingsExpanded = false;

    /// <summary>Rate control mode: "crf" (default), "vbr", "cbr"</summary>
    public string ChartRenderRateControl = "crf";
    /// <summary>Video bitrate in Mbps (used when rate control is vbr or cbr)</summary>
    public float ChartRenderBitrateMbps = 50f;

    /// <summary>
    /// Ensure defaults for paths (called before render session).
    /// </summary>
    public void EnsureDefaults(UnityModManagerNet.UnityModManager.ModEntry modEntry)
    {
        if (string.IsNullOrWhiteSpace(ChartRenderWorkspaceDirectory))
            ChartRenderWorkspaceDirectory = Path.Combine(modEntry.Path, "Workspace");
        if (string.IsNullOrWhiteSpace(ChartRenderExportDirectory))
            ChartRenderExportDirectory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),
                "ADOFAI Renders");
    }

    // --- Localization ---
    [XmlIgnore]
    private Dictionary<string, string> _localization = new();

    /// <summary>
    /// Get localized text for a key. Falls back to the key itself.
    /// </summary>
    public string Text(string key)
    {
        if (_localization.TryGetValue(key, out var value))
            return value;
        return key;
    }

    /// <summary>
    /// Set a localization entry manually (called at startup).
    /// </summary>
    public void SetText(string key, string value)
    {
        _localization[key] = value;
    }

    // --- Serialization ---
    public string GetPath(UnityModManager.ModEntry modEntry)
        => Path.Combine(modEntry.Path, "ChartRenderingSettings.xml");

    public void Save(UnityModManager.ModEntry modEntry)
    {
        try
        {
            var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ChartRenderingSettings));
            using var writer = new StreamWriter(GetPath(modEntry));
            serializer.Serialize(writer, this);
        }
        catch (Exception e)
        {
            ChartRenderMain.Log($"Failed to save chart rendering settings: {e.Message}");
        }
    }

    public static ChartRenderingSettings Load(UnityModManager.ModEntry modEntry)
    {
        try
        {
            var path = Path.Combine(modEntry.Path, "ChartRenderingSettings.xml");
            if (File.Exists(path))
            {
                var serializer = new System.Xml.Serialization.XmlSerializer(typeof(ChartRenderingSettings));
                using var reader = new StreamReader(path);
                return (ChartRenderingSettings)serializer.Deserialize(reader);
            }
        }
        catch (Exception e)
        {
            ChartRenderMain.Log($"Failed to load chart rendering settings: {e.Message}");
        }
        return new ChartRenderingSettings();
    }
}
