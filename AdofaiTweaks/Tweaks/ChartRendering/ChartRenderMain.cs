using System;
using UnityModManagerNet;

namespace AdofaiTweaks.Tweaks.ChartRendering;

/// <summary>
/// Entry point bridge for the ChartRendering module.
/// Mirrors the original ADOFAI.EditorTweaks.Main class.
/// </summary>
public static class ChartRenderMain
{
    /// <summary>
    /// The UMM mod entry. Set by the ChartRendering tweak on load.
    /// </summary>
    public static UnityModManager.ModEntry Mod { get; set; }

    /// <summary>
    /// Chart rendering settings. Set by the ChartRendering tweak on load.
    /// </summary>
    public static ChartRenderingSettings Settings { get; set; } = new ChartRenderingSettings();

    /// <summary>
    /// Whether the current language is Chinese Simplified.
    /// </summary>
    public static bool IsZh { get; set; }

    /// <summary>
    /// Logger wrapper using AdofaiTweaks.Logger.
    /// </summary>
    public static void Log(string message)
    {
        AdofaiTweaks.Logger?.Log($"  [ChartRender] {message}");
    }

    /// <summary>
    /// Log an exception with message.
    /// </summary>
    public static void Log(string message, Exception ex)
    {
        AdofaiTweaks.Logger?.Log($"  [ChartRender] {message}: {ex}");
    }
}
