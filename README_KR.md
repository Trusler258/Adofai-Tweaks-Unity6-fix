# AdofaiTweaks v2.9.0 — Unity 6 / r143 강화판

[![C#](https://img.shields.io/badge/C%23-4.8-blue)](https://learn.microsoft.com/en-us/dotnet/csharp/)
[![License](https://img.shields.io/badge/License-MIT-green)](LICENSE)
[![Version](https://img.shields.io/badge/version-v2.9.0-ff69b4)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)
[![Downloads](https://img.shields.io/github/downloads/Trusler258/Adofai-Tweaks-Unity6-fix/total)](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix)

[中文](README.md) · [English](README_EN.md) · [日本語](README_JP.md)

---

### 소개

《불과 얼음의 춤》이 Unity 6 (r143) 으로 업그레이드되면서 많은 Harmony 패치가 깨졌습니다. 이 포크는 [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) 의 API 변경 사항을 수정하고, 차트 영상 렌더링 등 새로운 기능을 추가했습니다.

### 기능 출처

| 모듈 | 출처 | 설명 |
|------|------|------|
| 핵심 트윅 프레임워크 | [PizzaLovers007/AdofaiTweaks](https://github.com/PizzaLovers007/AdofaiTweaks) v2.8.1 | Harmony 패치, 설정, 다국어 |
| Harmony 코드 생성 수정 | [BepInEx/HarmonyX](https://github.com/BepInEx/HarmonyX) | `MethodInfo.Invoke` → `DynamicMethod` |
| FFmpeg 검색 | 이 프로젝트 | 시스템 PATH → Tools → 미러 다운로드 |
| 차트 영상 렌더링 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) v1.2.5 | 오프라인 MP4/MKV/MOV 출력 |
| FFmpeg 참조 문서 | [ADOFAI.EditorTweaks](https://github.com/memsys-lizi/ADOFAI.EditorTweaks) | `Resources/FFmpegReference.html` |

### 주요 기능

- **키 제한** — 트리거 가능한 키 제한
- **키 표시** — 현재 키 상태를 화면에 표시
- **판정 제한** — 판정 범위 제한, 초과 시 사망/재시작
- **판정 시각 효과** — 퍼펙트 판정 숨기기, 히트 오차 표
- **편집기 향상** — 타일 번호 표시, 편집기 줌 비활성화
- **효과 비활성화** — 플래시, 화면 흔들림, 필터 등 비활성화
- **UI 숨기기** — Otto, 결과 텍스트 등 숨기기
- **행성 색상/투명도** — 행성 외관 사용자 정의
- **기타** — Glitch 플립 비활성화, 히트 사운드 볼륨 고정
- **차트 영상 렌더링** — 오프라인 레벨-비디오 내보내기 (MP4/MKV/MOV, AAC/FLAC/ALAC, 스마트 비트레이트)

### 설치

1. [UnityModManager](https://www.nexusmods.com/site/mods/21) 설치
2. [Releases](https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix/releases) 에서 `AdofaiTweaks-v2.9.0.zip` 다운로드
3. UnityModManager 에 zip 드래그
4. 게임 실행

FFmpeg 는 자동 감지: 시스템 PATH → Tools 폴더 → 미러에서 자동 다운로드.

### 빌드

```bash
# .NET Framework 4.8 SDK 필요
git clone https://github.com/Trusler258/Adofai-Tweaks-Unity6-fix
cd AdofaiTweaks
dotnet build -c Release
```

### 라이선스

[MIT License](LICENSE)
