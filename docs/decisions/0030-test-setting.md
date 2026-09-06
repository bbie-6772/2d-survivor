# 0030. 성능 측정 전, 고정해야할 항목들을 정리한다

- 날짜: 2026-09-06 (D10)
- 상태: 문서 기록

## 측정 환경
에디터 Play / 해상도 1920:1080(Full HD) / Profiler on / Scene 뷰 닫음 (Game 뷰 Maximize On Play)

## 세팅 (커밋하지 않음)
Health.MaxHealth = 9999
MeleeWeapon: Angle 360 / Damage 10 / AttacksPerSecond 1
레벨업 비활성 방법: Player -> CardSelector 비활성화
WaveTable: 원본 (수정 없음)
무조작

## 측정 지점
15초, 27초, 39초 (웨이브 변환 + 3초) / 각 지점 5초 관찰
60초, 90초, 120초, 180초, 240초, 280초
ㅡㅡㅡㅡㅡ Remaining Time 기준으로 정리 ㅡㅡㅡㅡㅡ
4:45, 4:33, 4:21 (웨이브 변환 + 3초)
4:00, 3:30, 3:00, 2:00, 1:00, 0:20

중단 기준 불필요 — 처치율이 스폰율의 7%
(콜라이더 2×1로 사거리 원(π×3²≈28.3)에 약 14마리, 10초당 14마리면 초당 1.4 vs 스폰 초당 20)

## 기록 항목
프레임 타임(ms) 중앙값 / GC Alloc / CPU Hierarchy 상위 3 / AliveCount