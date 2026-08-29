# 0005. 컨벤션 문서에 세 줄을 추가한다

- 날짜: 2026-08-29 (D2)
- 상태: 채택
- 변경 대상: `conventions.md` 1항, 5항

## 맥락

D2 작업 중 기존 규칙만으로는 판단이 안 되는 경계를 세 번 만났다.
규칙을 운용해보니 정밀해져야 할 지점이 드러난 것이므로 문서에 반영한다.

## 결정

### 1항에 추가

> **`public` 금지는 필드에 한정한다.** 메서드, 프로퍼티, event는 해당하지 않는다.
> **델리게이트를 노출할 때는 반드시 `event` 키워드를 붙인다.**

이유: 1항이 금지한 것은 데이터의 직접 노출이다. 누구나 `TakeDamage`를 거치지 않고
값을 바꿀 수 있으면 사망 판정이 통째로 건너뛰어진다. 메서드는 애초에 그 통로이므로 해당 없음.

`event` 키워드가 없는 `public Action`은 외부에서 목록 통째 대입(`= null`)과
임의 발행(`OnDied()`)이 가능하다. `event`를 붙이면 `+=`, `-=`만 남는다.
이는 델리게이트에 대한 `{ get; private set; }`과 같은 역할이므로 1항의 정신에 부합한다.

### 5항에 추가

> **빈 생명주기 함수를 남기지 않는다.** 템플릿이 생성한 빈 `Update()`도 엔진이 매 프레임 호출한다.
> 300마리면 프레임당 300회의 managed↔native 경계 비용이 하는 일 없이 발생한다.

> **자동 생성된 주석과 `using`을 방치하지 않는다.**
> `// Update is called once per frame` 이 `FixedUpdate` 위에 남는 식의 틀린 주석은 없는 주석보다 나쁘다.
> IDE가 자동 추가한 `using System;`이 `UnityEngine.Object`와 충돌해 CS0104를 낸 사례가 있다.

## 관련 사례

- `Health.OnDied` — `event Action`으로 선언
- `EnemyChase` — 빈 `Update()` 및 미사용 `using System;` 제거
- `PlayerHitReceiver` — IDE가 넣은 `using Unity.VisualScripting;` 제거
