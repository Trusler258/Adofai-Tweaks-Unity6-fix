using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Text.RegularExpressions;

namespace AdofaiTweaks.Tweaks.ChartRendering
{
    internal static class ChartRenderPaths
    {
        private const int MaxBaseFileNameLength = 96;
        private static string _cachedFfmpegPath;
        private static bool _cachedFfmpegChecked;

        /// <summary>
        /// Find ffmpeg: system PATH → Tools folder → download from mirror → official fallback.
        /// Result is cached after first successful resolution.
        /// </summary>
        public static string GetFfmpegPath()
        {
            if (_cachedFfmpegChecked)
                return _cachedFfmpegPath ?? string.Empty;

            _cachedFfmpegChecked = true;

            // 1. Try system PATH
            string systemPath = FindInSystemPath();
            if (systemPath != null)
            {
                _cachedFfmpegPath = systemPath;
                ChartRenderMain.Log("FFmpeg found in system PATH: " + systemPath);
                return systemPath;
            }

            // 2. Try Tools folder
            string toolsPath = GetToolsFfmpegPath();
            if (File.Exists(toolsPath))
            {
                _cachedFfmpegPath = toolsPath;
                ChartRenderMain.Log("FFmpeg found in Tools folder: " + toolsPath);
                return toolsPath;
            }

            // 3. Download
            ChartRenderMain.Log("FFmpeg not found, attempting download...");
            if (TryDownloadFfmpeg(toolsPath))
            {
                _cachedFfmpegPath = toolsPath;
                return toolsPath;
            }

            ChartRenderMain.Log("FFmpeg download failed, unavailable.");
            return string.Empty;
        }

        /// <summary>
        /// Reset cache so next call re-scans (used after manual install).
        /// </summary>
        public static void InvalidateCache()
        {
            _cachedFfmpegChecked = false;
            _cachedFfmpegPath = null;
        }

        private static string GetToolsFfmpegPath()
        {
            return ChartRenderMain.Mod == null ? string.Empty
                : Path.Combine(ChartRenderMain.Mod.Path, "Tools", "ffmpeg.exe");
        }

        private static string FindInSystemPath()
        {
            try
            {
                var psi = new ProcessStartInfo
                {
                    FileName = "where",
                    Arguments = "ffmpeg",
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };
                using var proc = Process.Start(psi);
                if (proc == null) return null;
                string output = proc.StandardOutput.ReadToEnd().Trim();
                proc.WaitForExit(3000);
                if (proc.ExitCode != 0 || string.IsNullOrEmpty(output))
                    return null;
                // Take first line (first match in PATH order)
                string first = output.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries)[0];
                if (File.Exists(first)) return first;
            }
            catch { }
            return null;
        }

        private static bool TryDownloadFfmpeg(string destPath)
        {
            // Mirror sites, tried in order
            var urls = new[]
            {
                "https://mirror.ghproxy.com/https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip",
                "https://ghproxy.net/https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip",
                "https://github.com/BtbN/FFmpeg-Builds/releases/download/latest/ffmpeg-master-latest-win64-gpl.zip",
            };

            string tempZip = Path.Combine(Path.GetTempPath(), "adoftweak_ffmpeg.zip");
            string tempExtract = Path.Combine(Path.GetTempPath(), "adoftweak_ffmpeg_extract");

            foreach (string url in urls)
            {
                try
                {
                    ChartRenderMain.Log("Downloading FFmpeg from: " + url);
                    DownloadFile(url, tempZip);

                    // Extract from zip: ffmpeg-master-latest-win64-gpl/bin/ffmpeg.exe
                    if (Directory.Exists(tempExtract))
                        Directory.Delete(tempExtract, true);
                    Directory.CreateDirectory(tempExtract);
                    ZipFile.ExtractToDirectory(tempZip, tempExtract);

                    // Find ffmpeg.exe anywhere in the extracted tree
                    string found = null;
                    foreach (string f in Directory.GetFiles(tempExtract, "ffmpeg.exe", SearchOption.AllDirectories))
                    {
                        found = f;
                        break;
                    }

                    if (found != null)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(destPath)!);
                        File.Copy(found, destPath, true);
                        ChartRenderMain.Log("FFmpeg downloaded and installed to: " + destPath);
                        return true;
                    }

                    ChartRenderMain.Log("FFmpeg.exe not found in downloaded archive");
                }
                catch (Exception ex)
                {
                    ChartRenderMain.Log("FFmpeg download failed from " + url + ": " + ex.Message);
                }
                finally
                {
                    try { if (File.Exists(tempZip)) File.Delete(tempZip); } catch { }
                    try { if (Directory.Exists(tempExtract)) Directory.Delete(tempExtract, true); } catch { }
                }
            }

            return false;
        }

        private static void DownloadFile(string url, string destPath)
        {
            // WebClient is the simplest cross-framework approach
            using var client = new WebClient();
            client.DownloadFile(url, destPath);
        }

        public static string GetWorkspaceDirectory(ChartRenderingSettings settings)
        {
            return ExpandPath(settings.ChartRenderWorkspaceDirectory);
        }

        public static string GetExportDirectory(ChartRenderingSettings settings)
        {
            return ExpandPath(settings.ChartRenderExportDirectory);
        }

        public static string MakeSafeFileName(string raw)
        {
            raw = StripRichTextTags(raw);
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = "ADOFAI_Render";
            }

            foreach (char c in Path.GetInvalidFileNameChars())
            {
                raw = raw.Replace(c, '_');
            }

            raw = Regex.Replace(raw, @"\s+", " ");
            raw = Regex.Replace(raw, @"_+", "_");
            raw = raw.Trim(' ', '.', '_');
            if (string.IsNullOrWhiteSpace(raw))
            {
                raw = "ADOFAI_Render";
            }

            return raw.Length <= MaxBaseFileNameLength
                ? raw
                : raw.Substring(0, MaxBaseFileNameLength).Trim(' ', '.', '_');
        }

        private static string StripRichTextTags(string raw)
        {
            return string.IsNullOrWhiteSpace(raw)
                ? string.Empty
                : Regex.Replace(raw, @"<[^>]*>", string.Empty);
        }

        private static string ExpandPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return string.Empty;
            }

            string expanded = Environment.ExpandEnvironmentVariables(path.Trim());
            return Path.GetFullPath(expanded);
        }
    }
}
