# AdofaiTweaks v2.9.0 — Unity 6 / r143 兼容移植

[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[中文](#中文) · [English](#english) · [한국어](#한국어) · [日本語](#日本語)

---

<a name="中文"></a>
## 中文

### 这是什么

基于 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) 的项目，修复了《冰与火之舞》升级到 Unity 6（r143）后的兼容性问题。

### 功能来源

| 模块 | 来源 | 说明 |
|------|------|------|
| 核心 tweak 框架 | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony 补丁架构、设置系统、多语言 |
| Harmony 代码生成修复 | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` 调用适配 |
| FFmpeg 下载/检测逻辑 | 本项目 | 系统 PATH → Tools → 镜像下载 三级检测 |
| 谱面视频渲染 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | 离线定帧导出 MP4/MKV/MOV |
| FFmpeg 参数参考文档 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### 功能

- **按键限制** — 限制可触发的按键
- **按键显示** — 屏幕显示当前按键状态
- **判定限制** — 限制判定范围，越界直接杀死/立即重启
- **判定视觉效果** — 隐藏完美判定、命中误差表
- **编辑器增强** — 地板编号显示、禁止编辑器缩放
- **禁用特效** — 禁用闪烁、屏幕震动、滤镜等
- **隐藏 UI** — 隐藏 Otto、结果文字等元素
- **星球颜色/透明度** — 自定义星球颜色和透明度
- **杂项** — 禁用 Glitch 翻转、强制音效音量等
- **谱面视频渲染** — 游戏内离线渲染关卡为视频，支持 MP4/MKV/MOV、AAC/FLAC/ALAC、智能码率推荐

### 安装

1. 安装 [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. 从 [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) 下载 `AdofaiTweaks-v2.9.0(2.8.1).zip`
3. 打开 UnityModManager，将 zip 拖入窗口
4. 启动游戏

FFmpeg 无需手动安装：打开渲染功能后会自动按「系统 PATH → Tools 文件夹 → 镜像下载」三级查找。

### 开发

```bash
# 需要先安装 .NET Framework 4.8 SDK
# 反编译游戏 DLL 到 adofai_decompiled/ 用于参考
git clone https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix
cd AdofaiTweaks
dotnet build -c Release
```

<sub>Code by DeepSeek v4 Pro & Trusler</sub>

### 许可

[MIT License](LICENSE)

---

<a name="english"></a>
## English

### What is this

A Unity 6 / r143 compatibility fork of [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) for A Dance of Fire and Ice.

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
2. Download `AdofaiTweaks-v2.9.0(2.8.1).zip` from [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases)
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

---

<a name="한국어"></a>
## 한국어

《불과 얼음의 춤》이 Unity 6 (r143) 으로 업그레이드된 후 호환성 문제를 수정한 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) 의 포크입니다.

### 기능 출처

| 모듈 | 출처 | 설명 |
|------|------|------|
| 핵심 트윅 프레임워크 | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony 패치, 설정, 다국어 |
| Harmony 코드 생성 수정 | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` |
| FFmpeg 검색 | 이 프로젝트 | 시스템 PATH → Tools 폴더 → 미러 다운로드 |
| 차트 영상 렌더링 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | 오프라인 MP4/MKV/MOV 출력 |
| FFmpeg 참조 문서 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### 주요 기능

- **키 제한**, **키 표시**, **판정 제한**, **판정 시각 효과**
- **편집기 향상**, **효과 비활성화**, **UI 숨기기**
- **행성 색상/투명도**, **잡다한 설정**
- **차트 영상 렌더링**

### 설치

1. [UnityModManager](https://www.nexusmods.com/site/mods/21) 설치
2. [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) 에서 zip 다운로드
3. UnityModManager 에 zip 드래그

### 라이선스

[MIT License](LICENSE)

---

<a name="日本語"></a>
## 日本語

《氷と炎のダンス》が Unity 6 (r143) にアップグレードされた後の互換性の問題を修正した [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) のフォークです。

### 機能の出典

| モジュール | 出典 | 説明 |
|------|------|------|
| コア調整フレームワーク | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony パッチ、設定、多言語 |
| Harmony コード生成修正 | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` |
| FFmpeg 検出 | このプロジェクト | システム PATH → Tools → ミラー DL |
| 譜面動画レンダリング | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | オフライン MP4/MKV/MOV 出力 |
| FFmpeg リファレンス | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### 主な機能

- **キー制限**, **キー表示**, **判定制限**, **判定視覚効果**
- **エディター強化**, **エフェクト無効化**, **UI非表示**
- **惑星の色/透明度**, **その他**
- **譜面動画レンダリング**

### インストール

1. [UnityModManager](https://www.nexusmods.com/site/mods/21) をインストール
2. [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) から zip をダウンロード
3. UnityModManager に zip をドラッグ

### ライセンス

[MIT License](LICENSE)
