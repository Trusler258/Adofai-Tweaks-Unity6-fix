using System;
using System.IO;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.ChartRendering.EditorOverlay
{
    internal sealed class EditorTweaksOverlayWindow : MonoBehaviour
    {
        private const int WindowId = 0x7E71A01;
        private const float Width = 400f;
        private const float CollapsedH = 36f;
        private const float ExpandedH = 520f;

        private static EditorTweaksOverlayWindow instance;
        private Rect windowRect;
        private ChartRenderSession chartRenderSession;
        private string chartRenderMessage;

        public static void Ensure()
        {
            if (instance != null) return;
            var host = new GameObject("AdofaiTweaks.ChartOverlay");
            DontDestroyOnLoad(host);
            instance = host.AddComponent<EditorTweaksOverlayWindow>();
        }

        public static void Destroy()
        {
            if (instance == null) return;
            Destroy(instance.gameObject);
            instance = null;
        }

        public static bool ShouldBlockEditorInput() => instance != null && instance.chartRenderSession != null && instance.chartRenderSession.IsActive;
        public static bool ShouldBlockMouseInput() => false;
        public static bool ShouldBlockGameplayInput() => ShouldBlockEditorInput();
        public static bool ShouldBlockUnityUiInput() => false;

        private void Awake()
        {
            float x = ChartRenderMain.Settings.EditorOverlayX;
            float y = ChartRenderMain.Settings.EditorOverlayY;
            if (x < 0) x = Screen.width - Width - 20;
            if (y < 0) y = 80;
            windowRect = new Rect(x, y, Width, CollapsedH);
        }

        private void OnGUI()
        {
            if (!ShouldDraw()) return;
            float h = ChartRenderMain.Settings.EditorOverlayCollapsed ? CollapsedH : ExpandedH;
            windowRect.height = h;
            windowRect.width = Width;
            windowRect = GUI.Window(WindowId, windowRect, DrawWindow, T("谱面视频渲染"));
        }

        private static bool ShouldDraw()
        {
            if (!ChartRenderMain.Settings.ShowEditorOverlay) return false;
            return ADOBase.isEditingLevel || ChartRenderSession.IsPlayableLevelLoaded() || ChartRenderSession.IsRendering;
        }

        private void DrawWindow(int id)
        {
            bool renderActive = chartRenderSession != null && chartRenderSession.IsActive;

            // Collapse button
            if (GUI.Button(new Rect(Width - 30, 4, 22, 22), ChartRenderMain.Settings.EditorOverlayCollapsed ? "+" : "-"))
            {
                ChartRenderMain.Settings.EditorOverlayCollapsed = !ChartRenderMain.Settings.EditorOverlayCollapsed;
                SaveSettings();
            }

            // Drag header
            GUI.DragWindow(new Rect(0, 0, Width - 36, CollapsedH));

            if (ChartRenderMain.Settings.EditorOverlayCollapsed) return;

            float y = 40;
            float lw = 100, vw = Width - lw - 50;

            // === Output Paths ===
            GUI.Label(new Rect(14, y, lw, 22), T("工作目录"));
            string workspace = GUI.TextField(new Rect(lw + 10, y, vw - 30, 22), ChartRenderMain.Settings.ChartRenderWorkspaceDirectory);
            if (workspace != ChartRenderMain.Settings.ChartRenderWorkspaceDirectory) { ChartRenderMain.Settings.ChartRenderWorkspaceDirectory = workspace; SaveSettings(); }
            if (GUI.Button(new Rect(Width - 34, y, 22, 22), "D")) { ChartRenderMain.Settings.ChartRenderWorkspaceDirectory = Path.Combine(ChartRenderMain.Mod.Path, "Workspace"); SaveSettings(); }

            y += 28;
            GUI.Label(new Rect(14, y, lw, 22), T("导出目录"));
            string export = GUI.TextField(new Rect(lw + 10, y, vw - 30, 22), ChartRenderMain.Settings.ChartRenderExportDirectory);
            if (export != ChartRenderMain.Settings.ChartRenderExportDirectory) { ChartRenderMain.Settings.ChartRenderExportDirectory = export; SaveSettings(); }
            if (GUI.Button(new Rect(Width - 34, y, 22, 22), "D")) { ChartRenderMain.Settings.ChartRenderExportDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "ADOFAI Renders"); SaveSettings(); }

            y += 38;
            // === Video Settings ===
            // Width / Height
            GUI.Label(new Rect(14, y, lw, 22), T("宽度"));
            var ws = GUI.TextField(new Rect(lw + 10, y, 60, 22), ChartRenderMain.Settings.ChartRenderWidth.ToString());
            if (int.TryParse(ws, out int wv)) { ChartRenderMain.Settings.ChartRenderWidth = Mathf.Clamp(wv, 16, 7680); SaveSettings(); }

            GUI.Label(new Rect(lw + 80, y, 40, 22), T("高度"));
            var hs = GUI.TextField(new Rect(lw + 120, y, 60, 22), ChartRenderMain.Settings.ChartRenderHeight.ToString());
            if (int.TryParse(hs, out int hv)) { ChartRenderMain.Settings.ChartRenderHeight = Mathf.Clamp(hv, 16, 4320); SaveSettings(); }

            y += 30;
            // FPS / CRF
            GUI.Label(new Rect(14, y, lw, 22), T("帧率"));
            var fs = GUI.TextField(new Rect(lw + 10, y, 60, 22), ChartRenderMain.Settings.ChartRenderFps.ToString());
            if (int.TryParse(fs, out int fv)) { ChartRenderMain.Settings.ChartRenderFps = Mathf.Clamp(fv, 1, 240); SaveSettings(); }

            GUI.Label(new Rect(lw + 80, y, 40, 22), T("CRF"));
            var cs = GUI.TextField(new Rect(lw + 120, y, 60, 22), ChartRenderMain.Settings.ChartRenderCrf.ToString());
            if (int.TryParse(cs, out int cv)) { ChartRenderMain.Settings.ChartRenderCrf = Mathf.Clamp(cv, 0, 51); SaveSettings(); }

            y += 30;
            // Tail / Judgments
            GUI.Label(new Rect(14, y, lw, 22), T("尾巴(秒)"));
            var ts = GUI.TextField(new Rect(lw + 10, y, 60, 22), ChartRenderMain.Settings.ChartRenderCompletionTailSeconds.ToString("0.0"));
            if (float.TryParse(ts, out float tv)) { ChartRenderMain.Settings.ChartRenderCompletionTailSeconds = Mathf.Clamp(tv, 0, 30); SaveSettings(); }

            bool showJ = GUI.Toggle(new Rect(lw + 80, y, vw - 60, 22), ChartRenderMain.Settings.ChartRenderShowHitJudgments, T("显示判定"));
            if (showJ != ChartRenderMain.Settings.ChartRenderShowHitJudgments) { ChartRenderMain.Settings.ChartRenderShowHitJudgments = showJ; SaveSettings(); }

            y += 30;
            // Encoder
            GUI.Label(new Rect(14, y, lw, 22), T("编码器"));
            string[] modes = { "auto", "fastest", "balanced", "quality", "cpu" };
            int sel = Array.IndexOf(modes, ChartRenderMain.Settings.ChartRenderEncoderMode);
            if (sel < 0) sel = 0;
            int ns = GUI.SelectionGrid(new Rect(lw + 10, y, vw + 40, 22), sel, modes, modes.Length);
            if (ns != sel) { ChartRenderMain.Settings.ChartRenderEncoderMode = modes[ns]; SaveSettings(); }

            y += 38;
            // Status
            string reason = GetDisabledReason();
            bool canRender = string.IsNullOrEmpty(reason) && !renderActive;
            string status = renderActive ? T("渲染中...")
                : (!string.IsNullOrEmpty(chartRenderMessage) ? chartRenderMessage
                : (canRender ? T("就绪，点击开始渲染") : reason));
            GUI.Label(new Rect(14, y, Width - 28, 40), status);

            y += 50;
            // Render button
            GUI.enabled = canRender;
            GUI.backgroundColor = canRender ? new Color(0.3f, 0.7f, 0.3f) : Color.gray;
            if (GUI.Button(new Rect(14, y, Width - 28, 36), T("开始渲染")))
            {
                StartChartRender();
            }
            GUI.backgroundColor = Color.white;
            GUI.enabled = true;

            y += 44;
            GUI.Label(new Rect(14, y, Width - 28, 18), GetProfileText());
        }

        private static string GetProfileText()
        {
            var s = ChartRenderMain.Settings;
            return $"{s.ChartRenderWidth}x{s.ChartRenderHeight} @ {s.ChartRenderFps}fps  CRF:{s.ChartRenderCrf}";
        }

        private static string GetDisabledReason()
        {
            if (!ChartRenderSession.IsPlayableLevelLoaded())
                return T("请打开一个有谱面的关卡");
            if (!ChartRenderSession.HasRenderableAudio())
                return T("缺少音频文件");
            if (!File.Exists(ChartRenderPaths.GetFfmpegPath()))
                return T("FFmpeg 未安装");
            if (string.IsNullOrWhiteSpace(ChartRenderMain.Settings.ChartRenderExportDirectory))
                return T("请设置导出目录");
            return string.Empty;
        }

        private void StartChartRender()
        {
            chartRenderMessage = string.Empty;
            ChartRenderMain.Settings.EnsureDefaults(ChartRenderMain.Mod);
            chartRenderSession = new ChartRenderSession(ChartRenderMain.Mod, ChartRenderMain.Settings);
            StartCoroutine(chartRenderSession.Run(result =>
            {
                chartRenderMessage = result.Success
                    ? T("完成: ") + result.OutputPath
                    : T("失败: ") + result.Message;
            }));
        }

        private static string T(string zh) => ChartRenderMain.IsZh ? zh : zh switch
        {
            "谱面视频渲染" => "Chart Renderer",
            "宽度" => "Width", "高度" => "Height", "帧率" => "FPS",
            "尾巴(秒)" => "Tail (s)", "显示判定" => "Show Judgments",
            "编码器" => "Encoder", "渲染中..." => "Rendering...",
            "就绪，点击开始渲染" => "Ready, click to render",
            "开始渲染" => "Start Render",
            "请打开一个有谱面的关卡" => "Open a level with chart",
            "缺少音频文件" => "Missing audio file",
            "FFmpeg 未安装" => "FFmpeg not installed",
            "请设置导出目录" => "Set export directory",
            "工作目录" => "Workspace",
            "导出目录" => "Export Dir",
            "完成: " => "Done: ", "失败: " => "Failed: ",
            _ => zh
        };

        private void SaveSettings() => ChartRenderMain.Settings.Save(ChartRenderMain.Mod);
    }
}
