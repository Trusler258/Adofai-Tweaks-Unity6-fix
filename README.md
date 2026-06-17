# AdofaiTweaks — Unity 6 兼容移植版

[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Stars](https://img.shields.io/github/stars/Trusler258/Adofai-Tweaks-Unity6-fix?style=flat)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Language](https://img.shields.io/github/languages/top/Trusler258/Adofai-Tweaks-Unity6-fix)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Repo Size](https://img.shields.io/github/repo-size/Trusler258/Adofai-Tweaks-Unity6-fix)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[English](#english) | [中文](#chinese)

---

<a name="chinese"></a>
## 中文

基于 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) 的项目，修复了《冰与火之舞》升级到 **Unity 6（r143）** 后的兼容性问题，大部分功能可用，但可能存在未知 Bug。

### 变更说明

游戏从旧版 Unity 升级到 Unity 6 时大幅重构了 C# 代码，导致原版 Harmony 补丁多处失效。本仓库修复了以下 API 变更等：

| 旧 API | 新 API |
|--------|--------|
| `scrController.ShowHitText()` | `scrHitTextManager.ShowHitText()` |
| `scrController.CountValidKeysPressed()` | `scrPlayer.CountValidKeysPressed()` |
| `scrController.isCW` | `planetarySystem.isCW` |
| `scrController.txtResults` | `scrController.detailedResults` |
| `PlanetarySystem.ColorPlanets()` | `ApplyMultiplanetColors()` |
| `RDString.Get(key, params, section)` | `RDString.Get(key, params)` |

此外，旧版 AssetBundle（`adofai_tweaks.assets`）在 Unity 6 无法加载，已改为将原始 PNG 素材内嵌到代码中。

### 安装

1. 安装 [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. 从 [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) 下载 `AdofaiTweaks-v2.9.0.zip`
3. 打开 UnityModManager，将 zip 拖入窗口
4. 启动游戏

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

<sub>Code by DeepSeek v4 Pro &amp; Trusler</sub>

### 许可

[MIT License](LICENSE)

---

<a name="english"></a>
## English

Fork of [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks), updated for **Unity 6 (r143)** compatibility; most features work, unknown bugs may exist.

### What changed

The game underwent significant C# code refactoring during the Unity 6 upgrade, breaking many Harmony patches. This fork addresses all API changes and adds defensive error handling to prevent single-patch failures from crashing the entire mod.

### Installation

1. Install [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. Download `AdofaiTweaks-v2.9.0.zip` from [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases)
3. Drag the zip into UnityModManager
4. Launch the game

<sub>Code by DeepSeek v4 Pro &amp; Trusler</sub>

### License

[MIT License](LICENSE)
