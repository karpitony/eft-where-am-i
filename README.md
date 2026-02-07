<p align="center">

[![GitHub Release](https://img.shields.io/github/v/release/karpitony/eft-where-am-i?include_prereleases&logo=github)](https://github.com/karpitony/eft-where-am-i/releases/latest)
[![GitHub total downloads](https://img.shields.io/github/downloads/karpitony/eft-where-am-i/total.svg?include_prerelease&logo=github)](https://github.com/karpitony/eft-where-am-i/releases)
[![GitHub License](https://img.shields.io/github/license/karpitony/eft-where-am-i)](./LICENSE)
[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-purple?logo=dotnet)](https://dotnet.microsoft.com/download/dotnet/8.0)

</p>

> **[English](README_en.md)** | **한국어 (현재)**

# EFT: Where Am I

타르코프 스크린샷 파일명에 포함된 3D 좌표와 쿼터니언 회전 데이터를 파싱하여, [tarkov-market.com](https://tarkov-market.com/) 맵 위에 현재 위치와 바라보는 방향을 표시해주는 도우미 프로그램입니다.

> [!WARNING]
> 본 프로그램은 [MIT 라이선스](./LICENSE)로 배포되며, **본 프로그램 사용으로 인한 어떠한 피해(BSG에 의한 제재 등)에 대해서도 책임지지 않습니다.**
> 본 프로그램은 [tarkov-market.com](https://tarkov-market.com/)과 공식적인 관계가 없으며, 맵 데이터와 UI를 활용하고 있습니다.

## 목차

- [주요 기능](#주요-기능)
- [지원 맵](#지원-맵)
- [시스템 요구 사항](#시스템-요구-사항)
- [다운로드 및 설치](#다운로드-및-설치)
- [사용 방법](#사용-방법)
- [설정](#설정)
- [기여하기](#기여하기)
- [버그 신고 및 건의](#버그-신고-및-건의)
- [크레딧 및 감사](#크레딧-및-감사)
- [라이선스](#라이선스)

## 주요 기능

### 스크린샷 기반 위치 표시

**스크린샷 한 장으로 맵 위의 정확한 위치를 확인하세요.**

타르코프가 저장하는 스크린샷 파일명에는 플레이어의 3D 좌표와 회전 데이터가 포함되어 있습니다. 이 데이터를 파싱하여 tarkov-market.com 맵 위에 빨간 점으로 위치를 표시합니다.

```
2026-01-10[03-59]_-318.44, 24.84, -107.49_0.00000, 0.82497, 0.00000, 0.56518_3.98 (0).png
│       날짜      │    X,    Z,      Y    │      쿼터니언 회전 (X,Y,Z,W)       │속도│
```

<!-- TODO: Add location marker screenshot -->

### 자동 스크린샷 감지

**스크린샷을 찍으면 위치가 자동으로 업데이트됩니다.**

`FileSystemWatcher`를 사용하여 스크린샷 폴더를 실시간 감시합니다. 새 스크린샷이 생성되면 자동으로 좌표를 파싱하여 위치를 갱신하므로, 매번 `강제 실행` 버튼을 누를 필요가 없습니다.

<!-- TODO: Add auto screenshot detection screenshot -->

### 자동 맵 감지

**레이드 진입 시 맵이 자동으로 전환됩니다.**

EFT 게임 로그 파일을 주기적으로 감시하여 현재 진입한 맵을 자동으로 감지합니다. 레이드에 들어가면 별도의 조작 없이 해당 맵으로 자동 전환됩니다.

<!-- TODO: Add auto map detection screenshot -->

### 다층 자동 감지

**Z좌표 기반으로 현재 층을 자동 판별합니다.**

리저브, 팩토리, 스트리트 등 다층 맵에서 스크린샷의 Z좌표(높이)를 분석하여 지상/지하 등 현재 층을 자동으로 전환합니다. `Ctrl+Numpad` 단축키로 수동 전환도 가능합니다.

<!-- TODO: Add floor detection screenshot -->

### 방향 표시기

**바라보는 방향을 화살표로 표시합니다.**

스크린샷 파일명의 쿼터니언(Quaternion) 데이터를 오일러 각도로 변환하여 SVG 삼각형 화살표로 플레이어가 바라보는 방향을 맵 위에 표시합니다.

<!-- TODO: Add direction indicator screenshot -->

### 데드존 자동 패닝

**마커가 화면 밖으로 벗어나지 않도록 자동 이동합니다.**

위치 마커가 뷰포트의 가장자리(데드존)에 도달하면 맵을 자동으로 이동시켜 마커가 항상 화면 안에 보이도록 합니다. 데드존 비율은 설정에서 조절할 수 있습니다.

<!-- TODO: Add deadzone panning screenshot -->

### 퀘스트 추적

**맵별로 선택한 퀘스트를 저장하고 복원합니다.**

tarkov-market.com의 퀘스트 패널에서 선택한 퀘스트 목록을 SQLite 데이터베이스에 맵별로 저장합니다. 다음에 같은 맵을 열면 이전에 선택한 퀘스트가 자동으로 복원됩니다.

<!-- TODO: Add quest tracking screenshot -->

### 층 구역 편집기

**폴리곤 기반의 층 구역을 시각적으로 편집합니다.**

맵 위에서 클릭하여 다각형(폴리곤) 형태의 층 구역을 정의할 수 있습니다. 홀(제외 영역)도 지원하여 복잡한 다층 건물의 층 판별을 정밀하게 설정할 수 있으며, 결과는 `floor_db.json`에 저장됩니다.

<!-- TODO: Add floor zone editor screenshot -->

## 지원 맵

| # | 맵 이름 | 내부 이름 |
|---|---------|-----------|
| 1 | Ground Zero | `ground-zero` |
| 2 | Factory | `factory` |
| 3 | Customs | `customs` |
| 4 | Interchange | `interchange` |
| 5 | Woods | `woods` |
| 6 | Shoreline | `shoreline` |
| 7 | Lighthouse | `lighthouse` |
| 8 | Reserve | `reserve` |
| 9 | Streets of Tarkov | `streets` |
| 10 | The Lab | `lab` |
| 11 | Labyrinth | `labyrinth` |
| 12 | Terminal | `terminal` |

## 시스템 요구 사항

| 항목 | 요구 사항 |
|------|-----------|
| OS | Windows 10 1809+ / Windows 11 |
| 런타임 | [.NET 8.0 Desktop Runtime (x64)](https://dotnet.microsoft.com/download/dotnet/8.0) |
| 브라우저 | [WebView2 Runtime](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) (대부분 사전 설치됨) |
| 네트워크 | 인터넷 연결 필수 (tarkov-market.com 접속) |
| 디스크 | ~50 MB |

## 다운로드 및 설치

### 다운로드

[Releases](https://github.com/karpitony/eft-where-am-i/releases/latest)에서 최신 버전의 `.zip` 파일을 다운로드하세요.

1. `.zip` 파일을 다운로드합니다.
2. 원하는 위치에 압축을 해제합니다.
3. `eft-where-am-i.exe`를 실행합니다.

<img src="assets/eft-wmi-exe.png" width="700">

### Windows 보안 경고

Windows SmartScreen 경고가 표시될 수 있습니다. `추가 정보` → `실행`을 클릭하세요. 바이러스가 포함되어 있지 않으며, [VirusTotal 검사 결과](https://www.virustotal.com/gui/file/4aa4768640a4c29ddc42ad1bc736d70c98630149985477e153bdae93aa91f010/detection)를 확인할 수 있습니다.

### 첫 실행

첫 실행 시 EFT 스크린샷 폴더와 로그 폴더 경로를 자동으로 감지합니다. 경로가 올바르지 않은 경우 설정에서 수동으로 변경하거나 `Auto Find` 버튼을 사용하세요.

## 사용 방법

### 기본 사용법

**1. 맵을 선택합니다.**

좌측 상단에서 맵을 선택합니다. 자동 맵 감지를 활성화하면 레이드 진입 시 자동으로 전환됩니다.

<img src="assets/screenshot01.png" width="700">

**2. 레이드 중 스크린샷을 촬영합니다.**

타르코프 기본 스크린샷 키는 `PrtSc`입니다.

**3. 맵에서 위치를 확인합니다.**

빨간 점과 방향 화살표로 현재 위치와 바라보는 방향이 표시됩니다.

**4. 자동 스크린샷 감지를 활성화하면 촬영할 때마다 자동 갱신됩니다.**

`강제 실행` 버튼을 매번 누를 필요 없이 스크린샷만 찍으면 됩니다.

### 고급 기능

#### 층 전환 단축키

타르코프가 활성 창일 때 아래 단축키로 층을 수동 전환할 수 있습니다.

| 단축키 | 동작 |
|--------|------|
| `Ctrl + Numpad 0` | 지하/벙커 |
| `Ctrl + Numpad 1` | 지상 (1층) |
| `Ctrl + Numpad 2` | 2층 |
| `Ctrl + Numpad 3` | 3층 |
| `Ctrl + Numpad 4` | 4층 |
| `Ctrl + Numpad 5` | 5층 |

#### 데드존 설정

설정에서 데드존 비율(기본값 93%)을 조절하여 마커가 화면 가장자리에서 얼마나 가까워졌을 때 자동 패닝을 실행할지 설정할 수 있습니다.

#### 층 구역 편집기

1. 설정에서 `층 구역 편집기` 버튼을 클릭합니다.
2. 맵 위에서 클릭하여 폴리곤 꼭짓점을 추가합니다.
3. 층 이름과 Z 범위를 설정합니다.
4. 저장하면 `floor_db.json`에 반영됩니다.

커뮤니티에서 만든 층 데이터 기여도 환영합니다.

## 설정

<img src="assets/screenshot02.png" width="700">

| 설정 항목 | 설명 |
|-----------|------|
| 언어 | 한국어 / English |
| 스크린샷 경로 | EFT 스크린샷 폴더 경로 (`Change` / `Auto Find`) |
| 로그 경로 | EFT 게임 로그 폴더 경로 |
| 자동 스크린샷 감지 | 스크린샷 촬영 시 자동 위치 갱신 |
| 자동 맵 감지 | 레이드 진입 시 자동 맵 전환 |
| 자동 패닝 | 데드존 기반 자동 맵 이동 |
| 층 구역 편집기 | 다층 맵의 층 구역 편집 |

## 기여하기

PR과 Issue 기여를 환영합니다!

1. 이 저장소를 Fork합니다.
2. 기능 브랜치를 생성합니다. (`git checkout -b feature/my-feature`)
3. 변경 사항을 커밋합니다.
4. Pull Request를 생성합니다.

`floor_db.json`에 새로운 맵의 층 데이터를 추가하는 기여도 매우 환영합니다.

## 버그 신고 및 건의

버그 신고, 기능 건의 등은 [Issues](https://github.com/karpitony/eft-where-am-i/issues)에서 남겨주세요.

## 크레딧 및 감사

- [Tarkov-Market](https://tarkov-market.com/) — 맵 데이터 및 UI
- [Tarkov-Client](https://github.com/byeong1/Tarkov-Client) by byeong1 — 방향 표시기 코드 (MIT License)
- [Freepik - Flaticon](https://www.flaticon.com/free-icons/map) — 맵 아이콘
- [Microsoft WebView2](https://developer.microsoft.com/en-us/microsoft-edge/webview2/) — 브라우저 컴포넌트

## 라이선스

MIT License — Copyright (c) 2024 karpitony

자세한 내용은 [LICENSE](./LICENSE) 파일을 참조하세요.

> [!CAUTION]
> 본 프로그램은 BSG/Battlestate Games와 무관하며, 사용으로 인한 게임 내 제재 등 어떠한 불이익에 대해서도 개발자는 책임지지 않습니다. 사용에 따른 위험은 본인이 감수해야 합니다.
