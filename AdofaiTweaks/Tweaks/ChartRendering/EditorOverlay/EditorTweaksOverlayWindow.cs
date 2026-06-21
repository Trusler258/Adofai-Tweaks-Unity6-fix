using System;
using System.IO;
using UnityEngine;

namespace AdofaiTweaks.Tweaks.ChartRendering.EditorOverlay
{
    internal sealed class EditorTweaksOverlayWindow : MonoBehaviour
    {
        private const int WindowId = 0x7E71A01;
        private const float MinWidth = 320f;
        private const float MaxWidth = 800f;
        private const float CollapsedH = 36f;
        private const float BaseExpandedH = 720f;
        private const float ResizeHandleSize = 12f;

        private static EditorTweaksOverlayWindow instance;
        private static bool mouseCapturedByOverlay;
        private static int mouseCaptureReleaseFrame = -1;
        private Rect windowRect;
        private ChartRenderSession chartRenderSession;
        private string chartRenderMessage;
        private bool cursorWasVisible;
        private CursorLockMode cursorWasLocked;
        private bool isResizing;
        private Vector2 resizeStartMouse;
        private Rect resizeStartRect;
        private bool overResizeHandle;

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
            mouseCapturedByOverlay = false;
            mouseCaptureReleaseFrame = -1;
        }

        private static bool IsRenderActive => instance != null && instance.chartRenderSession != null && instance.chartRenderSession.IsActive;

        public static bool ShouldBlockEditorInput() => IsRenderActive || ShouldBlockMouseInput();
        public static bool ShouldBlockGameplayInput() => IsRenderActive;
        public static bool ShouldBlockUnityUiInput() => ShouldBlockMouseInput();

        public static bool ShouldBlockMouseInput()
        {
            if (mouseCaptureReleaseFrame >= 0 && Time.frameCount > mouseCaptureReleaseFrame)
            {
                mouseCapturedByOverlay = false;
                mouseCaptureReleaseFrame = -1;
            }

            if (instance == null || !ShouldDraw())
            {
                mouseCapturedByOverlay = false;
                mouseCaptureReleaseFrame = -1;
                return false;
            }

            bool insideOverlay = instance.IsMouseInsideWindow();
            bool mouseDown = IsAnyMouseButtonDown();
            bool mouseUp = IsAnyMouseButtonUp();
            bool mouseHeld = IsAnyMouseButtonHeld();
            bool mouseActivity = mouseDown || mouseUp || mouseHeld || HasMouseWheelActivity();

            if (mouseDown)
                mouseCapturedByOverlay = insideOverlay;

            bool capturedThisFrame = mouseCapturedByOverlay || mouseCaptureReleaseFrame == Time.frameCount;
            bool block = mouseActivity && (insideOverlay || capturedThisFrame);

            if (mouseUp && mouseCapturedByOverlay && !mouseHeld)
                mouseCaptureReleaseFrame = Time.frameCount;

            return block;
        }

        private bool IsMouseInsideWindow()
        {
            Vector2 guiMouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            return windowRect.Contains(guiMouse);
        }

        private static bool IsAnyMouseButtonDown()
            => Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1) || Input.GetMouseButtonDown(2);

        private static bool IsAnyMouseButtonHeld()
            => Input.GetMouseButton(0) || Input.GetMouseButton(1) || Input.GetMouseButton(2);

        private static bool IsAnyMouseButtonUp()
            => Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1) || Input.GetMouseButtonUp(2);

        private static bool HasMouseWheelActivity()
            => Mathf.Abs(Input.mouseScrollDelta.x) > 0.01f || Mathf.Abs(Input.mouseScrollDelta.y) > 0.01f;

        private void Awake()
        {
            float savedW = ChartRenderMain.Settings.EditorOverlayWidth;
            float x = ChartRenderMain.Settings.EditorOverlayX;
            float y = ChartRenderMain.Settings.EditorOverlayY;
            if (savedW < MinWidth) savedW = MinWidth;
            if (x < 0) x = Screen.width - savedW - 20;
            if (y < 0) y = 80;
            windowRect = new Rect(x, y, savedW, CollapsedH);
        }

        private void OnGUI()
        {
            if (!ShouldDraw()) return;

            // Resize handle logic
            HandleResize();

            float h = ChartRenderMain.Settings.EditorOverlayCollapsed ? CollapsedH : GetExpandedHeight();
            if (!isResizing)
            {
                windowRect.height = h;
                windowRect.width = Mathf.Clamp(windowRect.width, MinWidth, MaxWidth);
            }

            windowRect = GUI.Window(WindowId, windowRect, DrawWindow, T("谱面视频渲染"));
        }

        private float GetExpandedHeight()
        {
            float extra = ChartRenderMain.Settings.ChartRenderAdvancedSettingsExpanded ? 120f : 0f;
            return BaseExpandedH + extra;
        }

        private void HandleResize()
        {
            Vector2 guiMouse = new Vector2(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            float br = ResizeHandleSize;
            Rect handleRect = new Rect(windowRect.xMax - br, windowRect.yMax - br, br, br);

            if (Event.current.type == EventType.Repaint)
            {
                overResizeHandle = handleRect.Contains(guiMouse);
            }

            if (Event.current.isMouse && Event.current.type == EventType.MouseDown && overResizeHandle)
            {
                isResizing = true;
                resizeStartMouse = guiMouse;
                resizeStartRect = windowRect;
                Event.current.Use();
            }

            if (isResizing)
            {
                if (Event.current.isMouse && Event.current.rawType == EventType.MouseUp)
                {
                    isResizing = false;
                    SaveWindowSize();
                    Event.current.Use();
                }
                else if (Event.current.isMouse)
                {
                    float dw = guiMouse.x - resizeStartMouse.x;
                    float dh = guiMouse.y - resizeStartMouse.y;
                    windowRect.width = Mathf.Clamp(resizeStartRect.width + dw, MinWidth, MaxWidth);
                    windowRect.height = Mathf.Max(resizeStartRect.height + dh, CollapsedH);
                    Event.current.Use();
                }
            }
        }

        private void SaveWindowSize()
        {
            ChartRenderMain.Settings.EditorOverlayX = windowRect.x;
            ChartRenderMain.Settings.EditorOverlayY = windowRect.y;
            ChartRenderMain.Settings.EditorOverlayWidth = windowRect.width;
            ChartRenderMain.Settings.Save(ChartRenderMain.Mod);
        }

        private static bool ShouldDraw()
        {
            if (!ChartRenderMain.Settings.ShowEditorOverlay) return false;
            return ADOBase.isEditingLevel || ChartRenderSession.IsPlayableLevelLoaded() || ChartRenderSession.IsRendering;
        }

        private void DrawWindow(int id)
        {
            bool renderActive = chartRenderSession != null && chartRenderSession.IsActive;
            float w = windowRect.width;

            // Title bar buttons
            GUI.backgroundColor = Color.clear;
            if (GUI.Button(new Rect(w - 30, 4, 22, 22), ChartRenderMain.Settings.EditorOverlayCollapsed ? "+" : "-"))
            {
                ChartRenderMain.Settings.EditorOverlayCollapsed = !ChartRenderMain.Settings.EditorOverlayCollapsed;
                SaveSettings();
            }

            // Drag header
            GUI.DragWindow(new Rect(0, 0, w - 52, CollapsedH));

            if (ChartRenderMain.Settings.EditorOverlayCollapsed) return;

            float y = 40;
            float lw = Mathf.Clamp(w * 0.22f, 72f, 110f);
            float cw = w - lw - 28f;
            bool narrow = w < 380f;

            // === Output Paths ===
            if (!narrow) GUI.Label(new Rect(14, y, lw, 22), T("工作目录"));
            else GUI.Label(new Rect(14, y, 60, 22), T("工作目录"));
            string workspace = GUI.TextField(new Rect(lw + 10, y, cw - 30, 22), ChartRenderMain.Settings.ChartRenderWorkspaceDirectory);
            if (workspace != ChartRenderMain.Settings.ChartRenderWorkspaceDirectory) { ChartRenderMain.Settings.ChartRenderWorkspaceDirectory = workspace; SaveSettings(); }
            if (GUI.Button(new Rect(w - 34, y, 22, 22), "D")) { ChartRenderMain.Settings.ChartRenderWorkspaceDirectory = Path.Combine(ChartRenderMain.Mod.Path, "Workspace"); SaveSettings(); }

            y += 28;
            if (!narrow) GUI.Label(new Rect(14, y, lw, 22), T("导出目录"));
            else GUI.Label(new Rect(14, y, 60, 22), T("导出目录"));
            string export = GUI.TextField(new Rect(lw + 10, y, cw - 30, 22), ChartRenderMain.Settings.ChartRenderExportDirectory);
            if (export != ChartRenderMain.Settings.ChartRenderExportDirectory) { ChartRenderMain.Settings.ChartRenderExportDirectory = export; SaveSettings(); }
            if (GUI.Button(new Rect(w - 34, y, 22, 22), "D")) { ChartRenderMain.Settings.ChartRenderExportDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyVideos), "ADOFAI Renders"); SaveSettings(); }

            y += 34;
            // Width / Height with presets
            float hw = narrow ? 45f : 50f;
            GUI.Label(new Rect(14, y, narrow ? 36f : lw, 22), narrow ? "W" : T("宽度"));
            var ws = GUI.TextField(new Rect(lw + 10, y, hw, 22), ChartRenderMain.Settings.ChartRenderWidth.ToString());
            if (int.TryParse(ws, out int wv)) { ChartRenderMain.Settings.ChartRenderWidth = Mathf.Clamp(wv, 16, 7680); SaveSettings(); }

            float hx = lw + hw + 16;
            GUI.Label(new Rect(hx, y, narrow ? 14f : 35, 22), narrow ? "H" : T("高度"));
            var hs = GUI.TextField(new Rect(hx + (narrow ? 18f : 35f), y, hw, 22), ChartRenderMain.Settings.ChartRenderHeight.ToString());
            if (int.TryParse(hs, out int hv)) { ChartRenderMain.Settings.ChartRenderHeight = Mathf.Clamp(hv, 16, 4320); SaveSettings(); }

            // Resolution presets
            float px = hx + (narrow ? 18f : 35f) + hw + 8;
            GUI.skin.button.fontSize = 10;
            if (GUI.Button(new Rect(px, y, 28, 22), "1K")) { ChartRenderMain.Settings.ChartRenderWidth = 1920; ChartRenderMain.Settings.ChartRenderHeight = 1080; SaveSettings(); }
            if (GUI.Button(new Rect(px + 30, y, 28, 22), "2K")) { ChartRenderMain.Settings.ChartRenderWidth = 2560; ChartRenderMain.Settings.ChartRenderHeight = 1440; SaveSettings(); }
            if (GUI.Button(new Rect(px + 60, y, 28, 22), "4K")) { ChartRenderMain.Settings.ChartRenderWidth = 3840; ChartRenderMain.Settings.ChartRenderHeight = 2160; SaveSettings(); }
            GUI.skin.button.fontSize = 11;

            y += 30;
            // FPS / CRF with presets
            GUI.Label(new Rect(14, y, narrow ? 36f : lw, 22), narrow ? "FPS" : T("帧率"));
            var fs = GUI.TextField(new Rect(lw + 10, y, 40, 22), ChartRenderMain.Settings.ChartRenderFps.ToString());
            if (int.TryParse(fs, out int fv)) { ChartRenderMain.Settings.ChartRenderFps = Mathf.Clamp(fv, 1, 240); SaveSettings(); }

            GUI.skin.button.fontSize = 10;
            if (GUI.Button(new Rect(lw + 54, y, 28, 22), "30")) { ChartRenderMain.Settings.ChartRenderFps = 30; SaveSettings(); }
            if (GUI.Button(new Rect(lw + 84, y, 28, 22), "60")) { ChartRenderMain.Settings.ChartRenderFps = 60; SaveSettings(); }
            if (GUI.Button(new Rect(lw + 114, y, 28, 22), "120")) { ChartRenderMain.Settings.ChartRenderFps = 120; SaveSettings(); }
            GUI.skin.button.fontSize = 11;

            GUI.Label(new Rect(lw + 150, y, 35, 22), T("CRF"));
            var cs = GUI.TextField(new Rect(lw + 185, y, 40, 22), ChartRenderMain.Settings.ChartRenderCrf.ToString());
            if (int.TryParse(cs, out int cv)) { ChartRenderMain.Settings.ChartRenderCrf = Mathf.Clamp(cv, 0, 51); SaveSettings(); }

            GUI.Label(new Rect(lw + 230, y, w - lw - 240, 22), GetProfileText());

            y += 30;
            // Tail / Judgments
            GUI.Label(new Rect(14, y, narrow ? 50f : lw, 22), T("尾巴(秒)"));
            var ts = GUI.TextField(new Rect(lw + 10, y, 60, 22), ChartRenderMain.Settings.ChartRenderCompletionTailSeconds.ToString("0.0"));
            if (float.TryParse(ts, out float tv)) { ChartRenderMain.Settings.ChartRenderCompletionTailSeconds = Mathf.Clamp(tv, 0, 30); SaveSettings(); }

            bool showJ = GUI.Toggle(new Rect(lw + 80, y, cw - 60, 22), ChartRenderMain.Settings.ChartRenderShowHitJudgments, T("显示判定"));
            if (showJ != ChartRenderMain.Settings.ChartRenderShowHitJudgments) { ChartRenderMain.Settings.ChartRenderShowHitJudgments = showJ; SaveSettings(); }

            y += 30;
            // Audio Sync Offset
            GUI.Label(new Rect(14, y, lw, 22), T("音频偏移(ms)"));
            var os = GUI.TextField(new Rect(lw + 10, y, 60, 22), ChartRenderMain.Settings.ChartRenderAudioSyncOffsetMs.ToString("0.#"));
            if (float.TryParse(os, out float ov)) { ChartRenderMain.Settings.ChartRenderAudioSyncOffsetMs = Mathf.Clamp(ov, -500, 500); SaveSettings(); }
            int gameCalMs = 0;
            try { gameCalMs = scrConductor.currentPreset.inputOffset; } catch { }
            GUI.Label(new Rect(lw + 80, y, cw - 60, 22), T("游戏") + ": " + gameCalMs + "ms");

            y += 30;
            // Rate control
            GUI.Label(new Rect(14, y, lw, 22), T("码率模式"));
            string[] rcModes = { "CQP", "VBR", "CBR" };
            string[] rcVals = { "crf", "vbr", "cbr" };
            int rcSel = Array.IndexOf(rcVals, ChartRenderMain.Settings.ChartRenderRateControl);
            if (rcSel < 0) rcSel = 0;
            int rcNew = GUI.SelectionGrid(new Rect(lw + 10, y, cw + 40, 22), rcSel, rcModes, 3);
            if (rcNew != rcSel) { ChartRenderMain.Settings.ChartRenderRateControl = rcVals[rcNew]; SaveSettings(); }

            y += 30;
            // Bitrate (only for VBR/CBR)
            bool isBitrate = ChartRenderMain.Settings.ChartRenderRateControl != "crf";
            GUI.enabled = isBitrate;
            GUI.Label(new Rect(14, y, lw, 22), T("码率(Mbps)"));
            string brText = ChartRenderMain.Settings.ChartRenderBitrateMbps <= 0f ? "auto" : ChartRenderMain.Settings.ChartRenderBitrateMbps.ToString("0.#");
            var brs = GUI.TextField(new Rect(lw + 10, y, 60, 22), brText);
            if (brs.ToLower() == "auto" || brs == "0") { ChartRenderMain.Settings.ChartRenderBitrateMbps = 0f; SaveSettings(); }
            else if (float.TryParse(brs, out float brv)) { ChartRenderMain.Settings.ChartRenderBitrateMbps = Mathf.Clamp(brv, 0.5f, 500f); SaveSettings(); }
            GUI.enabled = true;

            y += 30;
            // Encoder
            GUI.Label(new Rect(14, y, lw, 22), T("编码器"));
            string[] modes = { "auto", "fastest", "balanced", "quality", "cpu" };
            int sel = Array.IndexOf(modes, ChartRenderMain.Settings.ChartRenderEncoderMode);
            if (sel < 0) sel = 0;
            int ns = GUI.SelectionGrid(new Rect(lw + 10, y, cw + 40, 22), sel, modes, (int)Mathf.Min(5, Mathf.Floor(cw / 70f)));
            if (ns != sel) { ChartRenderMain.Settings.ChartRenderEncoderMode = modes[ns]; SaveSettings(); }

            y += 30;
            // Preview mode
            GUI.Label(new Rect(14, y, lw, 22), T("预览"));
            string[] pvModes = { "完整", "暗淡", "关闭" };
            string[] pvVals = { "full", "dim", "minimal" };
            int pvSel = Array.IndexOf(pvVals, ChartRenderMain.Settings.ChartRenderPreviewMode);
            if (pvSel < 0) pvSel = 0;
            int pvNew = GUI.SelectionGrid(new Rect(lw + 10, y, cw + 40, 22), pvSel, pvModes, 3);
            if (pvNew != pvSel) { ChartRenderMain.Settings.ChartRenderPreviewMode = pvVals[pvNew]; SaveSettings(); }

            y += 30;
            // Output format
            GUI.Label(new Rect(14, y, lw, 22), T("输出格式"));
            string[] ofModes = { "MP4", "MKV", "MOV" };
            string[] ofVals = { "mp4", "mkv", "mov" };
            int ofSel = Array.IndexOf(ofVals, ChartRenderMain.Settings.ChartRenderOutputFormat);
            if (ofSel < 0) ofSel = 0;
            int ofNew = GUI.SelectionGrid(new Rect(lw + 10, y, cw + 40, 22), ofSel, ofModes, 3);
            if (ofNew != ofSel) { ChartRenderMain.Settings.ChartRenderOutputFormat = ofVals[ofNew]; SaveSettings(); }

            y += 30;
            // Audio format
            GUI.Label(new Rect(14, y, lw, 22), T("音频格式"));
            string[] afModes = { "AAC", "FLAC", "ALAC" };
            string[] afVals = { "aac", "flac", "alac" };
            int afSel = Array.IndexOf(afVals, ChartRenderMain.Settings.ChartRenderAudioFormat);
            if (afSel < 0) afSel = 0;
            int afNew = GUI.SelectionGrid(new Rect(lw + 10, y, cw + 40, 22), afSel, afModes, 3);
            if (afNew != afSel) { ChartRenderMain.Settings.ChartRenderAudioFormat = afVals[afNew]; SaveSettings(); }

            // === Advanced FFmpeg Settings ===
            y += 34;
            bool adv = ChartRenderMain.Settings.ChartRenderAdvancedSettingsExpanded;
            bool newAdv = GUI.Toggle(new Rect(14, y, cw, 22), adv, T("专业 FFmpeg 设置"));
            if (newAdv != adv) { ChartRenderMain.Settings.ChartRenderAdvancedSettingsExpanded = newAdv; SaveSettings(); }

            if (ChartRenderMain.Settings.ChartRenderAdvancedSettingsExpanded)
            {
                y += 26;
                GUI.color = new Color(1f, 0.85f, 0.3f);
                GUI.Label(new Rect(lw + 10, y, cw - 40, 36), T("警告：非专业用户请勿修改以下参数，可能导致渲染失败或画质异常。"));
                GUI.color = Color.white;

                y += 40;
                string customArgs = ChartRenderMain.Settings.ChartRenderCustomMuxArguments ?? "";
                string newArgs = GUI.TextField(new Rect(14, y, w - 28, 26), customArgs);
                if (newArgs != customArgs) { ChartRenderMain.Settings.ChartRenderCustomMuxArguments = newArgs; SaveSettings(); }

                y += 30;
                if (GUI.Button(new Rect(lw + 10, y, 180, 24), T("打开 FFmpeg 参数参考")))
                {
                    string helpPath = Path.Combine(ChartRenderMain.Mod.Path, "Resources", "FFmpegReference.html");
                    if (File.Exists(helpPath))
                        Application.OpenURL("file://" + helpPath);
                    else
                        Application.OpenURL("https://ffmpeg.org/ffmpeg-all.html");
                }
            }

            y += adv ? 40 : 30;
            // Status / Progress
            if (renderActive && chartRenderSession != null)
            {
                DrawProgress(y);
            }
            else
            {
                string reason = GetDisabledReason();
                bool canRender = string.IsNullOrEmpty(reason) && !renderActive;
                string status = (!string.IsNullOrEmpty(chartRenderMessage) ? chartRenderMessage
                    : (canRender ? T("就绪，点击开始渲染") : reason));
                GUI.Label(new Rect(14, y, w - 28, 40), status);

                y += 50;
                GUI.enabled = canRender;
                GUI.backgroundColor = canRender ? new Color(0.3f, 0.7f, 0.3f) : Color.gray;
                if (GUI.Button(new Rect(14, y, w - 28, 36), T("开始渲染")))
                {
                    StartChartRender();
                }
                GUI.backgroundColor = Color.white;
                GUI.enabled = true;
            }
        }

        private static string GetProfileText()
        {
            var s = ChartRenderMain.Settings;
            string rc = s.ChartRenderRateControl == "crf" ? "CRF:" + s.ChartRenderCrf
                : (s.ChartRenderRateControl.ToUpper() + " " + (s.ChartRenderBitrateMbps <= 0f ? "auto" : s.ChartRenderBitrateMbps.ToString("0.#") + "M"));
            return $"{s.ChartRenderWidth}x{s.ChartRenderHeight} @ {s.ChartRenderFps}fps {rc}";
        }

        private void DrawProgress(float y)
        {
            var s = chartRenderSession;
            float fw = windowRect.width - 28;
            float pct = Mathf.Clamp01(s.Progress);
            int wf = s.WrittenFrames, tf = s.TotalFrames;
            double fps = s.ProcessingFps;
            var eta = s.EstimatedRemaining;

            var barRect = new Rect(14, y, fw, 20);
            GUI.Box(barRect, "");
            var fillRect = new Rect(14, y, fw * pct, 20);
            GUI.Box(fillRect, "");
            GUI.Label(barRect, $"  {wf} / {tf}  ({pct * 100f:F0}%)");

            y += 24;
            GUI.Label(new Rect(14, y, fw, 20), T("步骤") + ": " + s.StageText);

            y += 20;
            string fpsStr = fps > 0.5 ? fps.ToString("F1") + " fps" : "-- fps";
            string etaStr = eta.TotalSeconds > 0.1 ? eta.Minutes + "m " + eta.Seconds + "s" : "--";
            GUI.Label(new Rect(14, y, fw, 20), fpsStr + "  |  ETA: " + etaStr + "  |  " + s.DuplicateFrames + T(" 重复帧"));

            y += 20;
            GUI.Label(new Rect(14, y, fw, 20), s.DetailText);

            y += 26;
            GUI.backgroundColor = new Color(0.8f, 0.3f, 0.3f);
            if (GUI.Button(new Rect(14, y, fw, 28), T("取消渲染")))
            {
                s.Cancel();
                chartRenderSession = null;
            }
            GUI.backgroundColor = Color.white;
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

            cursorWasVisible = Cursor.visible;
            cursorWasLocked = Cursor.lockState;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            chartRenderSession = new ChartRenderSession(ChartRenderMain.Mod, ChartRenderMain.Settings);
            StartCoroutine(chartRenderSession.Run(result =>
            {
                Cursor.visible = cursorWasVisible;
                Cursor.lockState = cursorWasLocked;

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
            "音频偏移(ms)" => "Audio Offset(ms)",
            "游戏" => "Game",
            "码率模式" => "Rate Control",
            "码率(Mbps)" => "Bitrate(Mbps)",
            "预览" => "Preview",
            "输出格式" => "Format",
            "音频格式" => "Audio",
            "步骤" => "Step",
            "取消渲染" => "Cancel Render",
            " 重复帧" => " dup",
            "完成: " => "Done: ", "失败: " => "Failed: ",
            "专业 FFmpeg 设置" => "Pro FFmpeg Settings",
            "警告：非专业用户请勿修改以下参数，可能导致渲染失败或画质异常。" => "WARNING: Do not modify unless you know what you're doing. Incorrect values may break rendering.",
            "打开 FFmpeg 参数参考" => "Open FFmpeg Reference",
            _ => zh
        };

        private void SaveSettings() => ChartRenderMain.Settings.Save(ChartRenderMain.Mod);
    }
}
