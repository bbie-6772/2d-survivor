# 0019. 전역 정지는 `Time.timeScale`로 하고, 복구 지점을 `Awake`에 둔다

- 날짜: 2026-09-05 (D9)
- 상태: 채택
- 관련: 0009(씬 주입) 및 `conventions.md` 5항 정밀화, 0016의 정지 수단 표 확장

## 맥락

레벨업 선택 화면과 결과 화면에서 게임을 **전부** 멈춰야 한다.
0016이 정리한 표는 **컴포넌트 하나를 멈추는** 수단(`enabled` /
`StopAllCoroutines` / `SetActive`)을 다뤘고, 여기서 필요한 것은 전역 정지다.

후보는 셋이었다.

| 수단 | 멈추는 것 | 안 멈추는 것 |
|---|---|---|
| `Time.timeScale = 0` | `FixedUpdate`, 물리, `Time.deltaTime`, `WaitForSeconds` | `Update` **호출 자체**, `Input`, `unscaledDeltaTime`, `WaitForSecondsRealtime`, UI |
| 컴포넌트별 `enabled = false` | 해당 컴포넌트의 콜백만 | 이미 도는 코루틴(0016), 다른 컴포넌트 전부, 물리 |
| 진행 상태 필드 검사 | 그 필드를 직접 보는 코드만 | 검사를 빠뜨린 코드 전부 |

## 결정

전역 정지는 `Time.timeScale = 0f`로 한다.

**복구는 `GameEnder.Awake`에서 `Time.timeScale = 1f`로 한다.**

```csharp
void Awake()
{
    _outcome = GameOutcome.Pending;
    Time.timeScale = 1f;
    ...
}
```

## 이유

두 번째 안은 0016이 이미 기각 사유를 갖고 있다. 스포너와 무기가 모두 코루틴으로
도는 이상 `enabled = false`로는 멈추지 않는다. 세 번째 안은 시스템이 늘어날 때마다
분기를 심어야 하고, 빠뜨리면 조용히 새는 종류의 버그가 된다.
일부만 멈추는 상황(`BossPhase`의 "메인 타이머만 정지")에 쓸 도구이지
전역 정지용이 아니다.

`timeScale = 0`이 `Update` 호출 자체는 막지 않는다는 점이 이 선택의 이득이다.
`spec.md`의 "약 제조 선택: 1 / 2 / 3 숫자 키" 입력을 정지 중에도 그대로 받을 수 있다.

**복구 지점을 `Awake`로 정한 근거는 "깜빡할 가능성이 가장 낮은 자리"다.**
`timeScale`은 전역 정적 값이라 **씬을 리로드해도 되돌아오지 않는다.**
재시작 버튼 쪽에 복구를 두면 재시작 경로가 늘어날 때마다 누락 위험이 생기지만,
`Awake`에 두면 씬이 로드되는 모든 경로가 자동으로 덮인다.

## 함께 확정된 것 — UI 스크립트의 부착 위치

결과 패널은 비활성 상태로 시작한다. 그런데 **비활성 GameObject 위의 컴포넌트는
`Awake`도 `Start`도 호출되지 않는다.** 따라서 패널에 스크립트를 붙이면
이벤트 구독이 영원히 일어나지 않는다.

> **이벤트를 구독하는 UI 스크립트는 항상 활성 상태인 오브젝트(Canvas)에 붙이고,
> 껐다 켤 대상은 `[SerializeField] GameObject`로 참조만 한다.**

`GameEndScreen`은 Canvas에 부착하고 `_panel`을 참조한다.

## 함께 정밀화되는 것 (`conventions.md` 5항 / 0009)

0009는 씬 주입 필드의 null 대응을 `Debug.LogError(메시지, this)` + `enabled = false`로
통일했다. 여기에 두 가지를 덧붙인다.

> **검증은 `Awake`에서 한다.** `Awake`에서 `enabled = false`가 되면 `Start`는
> 호출되지 않으므로, `Start`에서의 구독 코드가 `NullReferenceException`을 내지 않는다.
> 가드를 두 겹으로 쌓을 필요가 없다.
>
> **검증 대상 필드가 둘 이상이면 `return` 없이 전부 검사한다.**
> 첫 실패에서 빠져나오면 나머지 누락은 다음 실행에야 드러나 왕복이 늘어난다.
> 기존 예시(`SpawnerBase`, `CameraFollow`)가 `return`을 쓴 것은 검증 필드가 하나였고
> 뒤에 이어지는 로직이 있었기 때문이며, 검증만 하는 `Awake`에는 해당하지 않는다.

이 패턴은 `SpawnerBase` / `EnemySpawner` / `CameraFollow` / `GameEnder` /
`GameEndScreen`에서 반복되지만 **헬퍼로 추출하지 않는다.**
`Debug.LogError`의 두 번째 인자(클릭 시 해당 오브젝트로 점프하는 컨텍스트)를
넘기는 형태가 지저분해지고, 규칙 문장으로 고정하는 편이 싸다.

## 미해결 (D9 부채)

**`OnGameEnd` 발화와 `Time.timeScale = 0f`의 순서.**

현재는 정지가 먼저이고 발화가 나중이다. 구독자가 **이미 얼어붙은 세계에서**
깨어나므로, 결과 화면에 `WaitForSeconds` 기반 연출을 넣으면 영원히 끝나지 않는다.
(`WaitForSecondsRealtime`으로 회피 가능)

발화를 앞으로 옮기면 연출은 돌아가지만, 구독자가 실행되는 동안
`timeScale`과 구독 해제가 아직 완료되지 않은 상태를 보게 된다.

현재 구독자가 `GameEndScreen` 하나뿐이고 즉시 패널만 켜므로 두 배치 모두 동작한다.
**연출을 붙이는 시점에 재검토한다.**

## 재검토 시점

`BossPhase` 구현 시. "메인 타이머만 정지, 보스 타이머는 진행"은 `timeScale`로
표현할 수 없으므로, 그 시점에 `GameTimer`에 `Pause()` / `Resume()`을 뚫는다.
