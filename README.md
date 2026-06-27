# AdofaiTweaks v2.9.1 — Unity 6 / r143(r145) 增强版
# 原项目已更新,此仓库不再维护(除版本更新适配)
[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[English](README_EN.md) · [한국어](README_KR.md) · [日本語](README_JP.md)

---

### 这是什么

基于 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) 的项目，修复了《冰与火之舞》升级到 Unity 6（r143/r145）后的兼容性问题，并集成了谱面视频渲染等新功能。

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
2. 从 [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) 下载 `AdofaiTweaks-v2.9.0.zip`
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
