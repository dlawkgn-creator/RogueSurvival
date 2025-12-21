# 경일게임아카데미 팀 11번가
---
## RogueSurvival
Unity 6 (URP) 기반의 2D 로그라이크 프로젝트입니다.
턴제 기반의 이동과 전투를 중심으로 한 픽셀 아트 스타일의 생존 게임입니다.
Unity Learn의 공식 2D Rouelike Tutorial 을 따라 프로젝트 구조와 기본 세팅을 구성했습니다.

## Development Environment
- **Engine**: Unity 6 (6000.2.10f1)
- **Render Pipeline**: Universal Render Pipeline (URP)
- **Renderer**: 2D Renderer
- **Platform**: PC (Windows)

## System structure
1. GameManager
- 게임 전체 흐름 관리
- 플레이어 상태, 게임 오버 처리
- UI (GameOver Panel 등) 제어

2. BoardManager
- 맵 생성 및 셀 데이터 관리
- CellData 배열로 보드 상태 관리
- 오브젝트 배치 및 충돌 판정

3. TurnManager
- 턴 단위 이벤트 관리
- OnTick 이벤트로 턴 진행 알림
- 플레이어와 적 번갈아 진행

4. PlayerController
- Unity Input System 을 활용해 입력 처리
- 이동 요청 저장 후 턴에 반영
- 애니메이션 및 방향 처리

5. Enemy
- 턴 이벤트 구독
- 이동 및 공격 로직 처리
- 체력 관리 및 제거 처리

6. Visual & UI
- 픽셀 아트 스타일
- Panel 기반 UI
    - Title UI
    - GameOver UI
- SoundManager
    - BGM / SFX 분리
    - 볼륨 조절 UI
    - PlayerPrefs 로 설정 저장

## Project Setup
- Universal 2D 템플릿 사용
- URP Asset + **Renderer2D** 적용
- Renderer2D 기본 머티리얼을 **Unlit**으로 설정
- 픽셀 아트에 적합한 2D 렌더링 환경 구성

## View Settings
- Game View 고정 해상도: **1800 × 1800**
- 2D 카메라 기반 화면 구성

## Reference
- Unity Learn  
  **2D Roguelike Tutorial (Unity 6)**  
  https://learn.unity.com/course/2d-roguelike-tutorial

## Future plans
- 다양한 적 패턴 추가
    - 강력하지만 2턴마다 움직이는 느린 적 등
- 아이템 및 무기 시스템 확장
- 난이도 증가 로직 추가
- 레벨업 시 특성 선택
- 사운드 및 이펙트 강화

## License
이 프로젝트는 교육 및 포트폴리오 용도로 제작되었습니다.