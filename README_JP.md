# AdofaiTweaks v2.9.0 — Unity 6 / r143 互換性ポート

[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[中文](README.md) · [English](README_EN.md) · [한국어](README_KR.md)

---

### 概要

《氷と炎のダンス》が Unity 6 (r143) にアップグレードされた際、多数の Harmony パッチが破損しました。このフォークは [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) のすべての API 変更を修正し、単一パッチの失敗が Mod 全体をクラッシュさせないよう防御的エラーハンドリングを追加しています。

### 機能の出典

| モジュール | 出典 | 説明 |
|------|------|------|
| コア調整フレームワーク | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony パッチ、設定、多言語 |
| Harmony コード生成修正 | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` |
| FFmpeg 検出 | このプロジェクト | システム PATH → Tools → ミラー DL |
| 譜面動画レンダリング | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | オフライン MP4/MKV/MOV 出力 |
| FFmpeg リファレンス | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### 主な機能

- **キー制限** — トリガー可能なキーを制限
- **キー表示** — 現在のキー状態を画面に表示
- **判定制限** — 判定範囲を制限し、超過時に即死/リスタート
- **判定視覚効果** — パーフェクト判定の非表示、ヒット誤差表
- **エディター強化** — フロア番号表示、エディターズーム無効化
- **エフェクト無効化** — フラッシュ、画面揺れ、フィルター等を無効化
- **UI非表示** — Otto、結果テキスト等を非表示
- **惑星の色/透明度** — 惑星の外観をカスタマイズ
- **その他** — Glitch フリップ無効化、ヒットサウンド音量固定
- **譜面動画レンダリング** — オフラインレベル→動画出力（MP4/MKV/MOV, AAC/FLAC/ALAC, スマートビットレート）

### インストール

1. [UnityModManager](https://www.nexusmods.com/site/mods/21) をインストール
2. [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) から `AdofaiTweaks-v2.9.0.zip` をダウンロード
3. UnityModManager に zip をドラッグ
4. ゲームを起動

FFmpeg は自動検出: システム PATH → Tools フォルダ → ミラーから自動ダウンロード。

### ビルド

```bash
# .NET Framework 4.8 SDK が必要
git clone https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix
cd AdofaiTweaks
dotnet build -c Release
```

### ライセンス

[MIT License](LICENSE)
