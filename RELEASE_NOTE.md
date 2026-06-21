# AdofaiTweaks v2.9.0(2.8.1)fix_audio_desync — 发布说明

基于 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 的 Unity 6 / r143 增强版。

---

## 新增功能

- **谱面视频渲染**（整合自 [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5）
  - 游戏内离线渲染关卡为 MP4/MKV/MOV
  - 音频格式可选 AAC / FLAC / ALAC
  - 智能码率推荐（自动根据分辨率+帧率计算）
  - 分辨率快捷预设（1K / 2K / 4K）、帧率快捷预设（30 / 60 / 120）
  - 输出格式 / 音频格式选择
  - 专业 FFmpeg 设置折叠面板（自定义混流参数 + 参数参考帮助）
  - 渲染期间强制显示鼠标指针

- **可调整大小的 GUI 浮窗**（320-800px 宽度拖拽调整，控件自适应）

- **四语 README**（中 / 英 / 韩 / 日）

---

## 修复

- **音画延迟修复**：渲染时将玩家游玩偏移强制归零，且归零在视觉时钟锚定**之前**执行，确保 DSP 起点不含偏移量，视频与音频无相位差
- **编辑器冻结修复**：不再常驻注入 `scnEditor.Update` / `scrController.Update` 屏蔽补丁，改为仅渲染期间注入、完成即卸载
- **鼠标穿透修复**：采用原始 EditorTweaks 的鼠标捕获机制，仅在有实际鼠标操作时才屏蔽底层谱面交互
- **ESC 返回编辑器**：移除重复的 `SwitchToEditMode` 调用（之前会销毁场景两次导致编辑器状态异常）
- **GUI 样式修复**：按钮恢复默认样式，预设按钮尺寸加宽，字号保存恢复不影响其他 UI
- **进度条修复**：渲染进度实时更新
- **Unity 6 API 适配**：所有 Harmony 补丁适配 r143 的字段/方法名变更

---

## 快速开始

1. 安装 [UnityModManager](https://www.nexusmods.com/site/mods/21)
2. 拖入 `AdofaiTweaks-v2.9.0(2.8.1)fix_audio_desync.zip`
3. FFmpeg 自动检测（系统 PATH → Tools → 自动下载）

---

## 引用

| 来源 | 用途 |
|------|------|
| [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | 核心框架、所有基础 tweak |
| [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | 谱面视频渲染、FFmpeg 参考文档 |
| [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` 适配 |

---

<sub>Code by DeepSeek v4 Pro & Trusler</sub>
