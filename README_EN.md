# AdofaiTweaks v2.9.0 — Unity 6 / r143 Enhanced

[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[中文](README.md) · [한국어](README_KR.md) · [日本語](README_JP.md)

---

### What is this

A Unity 6 / r143 compatibility fork of [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) for A Dance of Fire and Ice, enhanced with chart video rendering and other new features.

### Feature Origins

| Module | Source | Notes |
|--------|--------|-------|
| Core tweak framework | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony patch architecture, settings, localization |
| Harmony codegen fixes | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` adaptation |
| FFmpeg detection | This project | 3-tier: System PATH → Tools folder → mirror download |
| Chart video rendering | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | Offline frame-locked MP4/MKV/MOV export |
| FFmpeg reference docs | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### Features

- **Key Limiting** — Limit triggerable keys
- **Key Viewing** — Display current key state on screen
- **Judgment Limiting** — Limit judgment range, kill/restart on miss
- **Judgment Visuals** — Hide perfect judgments, hit error table
- **Editor Enhancements** — Floor numbering, disable editor zoom
- **Disable Effects** — Disable flashes, screen shake, filters
- **Hide UI** — Hide Otto, result text, etc.
- **Planet Color/Opacity** — Customize planet appearance
- **Miscellaneous** — Disable Glitch flip, force hit sound volume
- **Chart Video Rendering** — Offline level-to-video export with MP4/MKV/MOV, AAC/FLAC/ALAC, smart bitrate

### Installation

1. Install [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. Download `AdofaiTweaks-v2.9.0.zip` from [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases)
3. Drag the zip into UnityModManager
4. Launch the game

FFmpeg is auto-detected: System PATH → Tools folder → auto-download from mirror.

### Building

```bash
# Requires .NET Framework 4.8 SDK
# Decompile game DLL into adofai_decompiled/ for reference
git clone https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix
cd AdofaiTweaks
dotnet build -c Release
```

### License

[MIT License](LICENSE)
