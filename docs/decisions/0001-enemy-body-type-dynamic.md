# 0001. 좀비 Rigidbody2D를 Dynamic으로 한다

- 날짜: 2026-08-29 (D2)
- 상태: 채택

## 맥락

좀비는 한 스테이지에 동시 300마리가 존재한다. 이때 좀비끼리 서로 밀어낼 것인지,
겹쳐서 통과할 것인지를 정해야 했다. 이 선택이 Body Type과 충돌 처리 방식을 전부 결정한다.

| Body Type | 밀리는가 | 위치를 정하는 주체 | Kinematic끼리 충돌 |
|---|---|---|---|
| Dynamic | O | 물리 엔진 | - |
| Kinematic | X | 내 코드 | 기본적으로 계산 안 함 |

## 결정

**Dynamic**을 쓴다. 좀비끼리 밀어낸다.

## 이유

- 겹침을 허용하면 300마리가 한 점에 포개져 개별 개체로 보이지 않는다.
- 밀어내기를 원하는데 Kinematic으로 가면 `Use Full Kinematic Contacts`를 켜야 하고,
  그래도 분리(contact resolution)는 직접 구현해야 한다. 지금 단계에서 감당할 범위가 아니다.
- D6 성능 측정에서 `Physics2D.Simulate` 비용이 얼마인지 재보고, 그때 다시 판단할 여지를 남긴다.

## 함께 강제되는 것

- 접촉 피격 판정은 Trigger가 아닌 **Collision 계열**을 쓴다.
  Trigger는 밀어내기를 하지 않으므로 이 결정과 양립하지 않는다.
- 탑다운이므로 Gravity Scale = 0.
- 좀비끼리 비스듬히 부딪힐 때 토크가 생기므로 Freeze Rotation Z.

## 대가 (인지하고 있음)

- 300개 Dynamic body의 broadphase + contact resolution 비용이 발생한다. D6 측정 대상.
- `EnemyChase`가 매 물리 스텝 `linearVelocity`에 **대입**하고 있어,
  엔진이 만들어낸 분리 속도가 매번 지워진다. 밀어내기가 누적되지 못하는 상태다.
  현재는 "어정쩡하게 밀린다"로 굴러가므로 D6까지 유지한다.

## 재검토 시점

D6 (`perf/object-pooling` 브랜치). Kinematic + 직접 분리 계산(spatial hash)이
수치상 유리하면 뒤집는다.
