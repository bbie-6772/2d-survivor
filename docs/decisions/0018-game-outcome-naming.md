# 0018. 판의 승패를 `GameOutcome`으로 명명하고 `GameState`를 비워둔다

- 날짜: 2026-09-05 (D9)
- 상태: 채택
- 관련: `conventions.md` 1항·6항, `spec.md` 게임 상태 표

## 맥락

`GameEnder`가 판의 승패를 담을 enum이 필요했다. 처음에는 클래스 내부에
`private enum Result { Running, Win, Lose }`로 중첩해 두었다.

두 가지가 겹쳤다.

**첫째, 중첩의 근거가 사라졌다.** 값이 `private` 필드로만 쓰일 때는 내부 표현이
맞았으나, `event Action<T>`로 밖에 내보내기로 하면서 타입도 밖에서 보여야 한다.
그 상태에서 `GameResult.Result.Win`은 "결과의 결과"로 읽힌다.

**둘째, `Result`라는 단어가 두 층위에서 다른 뜻으로 쓰인다.**
`spec.md`의 상태 표에서 `Result`는 **화면 / 페이즈**를 가리킨다
(`Normal` / `BossPhase` / `Result`). 반면 이 enum이 담는 것은 **판의 승패**다.
둘은 같은 것이 아니라 포함 관계다. `Result` 페이즈에 진입한 시점에
승패가 `Win`이거나 `Lose`인 것이다.

## 결정

독립 타입 `GameOutcome`을 `Scripts/Core/`에 둔다.

```csharp
public enum GameOutcome
{
    Pending,
    Win,
    Lose
}
```

`GameState`라는 이름은 **쓰지 않고 비워둔다.** `spec.md`의 페이즈
(`Normal` / `BossPhase` / `Result`)를 구현할 때 그 이름을 쓴다.

동반 변경: 클래스 이름을 `GameResult` → `GameEnder`로 변경했다.
`GameOutcome`(값)과 나란히 놓였을 때 어느 쪽이 컴포넌트인지 이름만으로 갈리며,
`CameraFollow` / `EnemyChase` / `DamageFlash`가 따르는
**[대상] + [하는 일]** 형태와도 일치한다.

## 이유

`Outcome`(결과)과 `State`(상태)를 어휘로 분리하면, 두 타입이 공존하게 될 때
이름만으로 구분된다. 지금 `GameState`를 승패에 써 버리면 페이즈 타입이
추가되는 시점에 둘 다 "게임 상태"로 읽혀 구분이 불가능해진다.

`Running` 대신 `Pending`을 택한 것도 같은 이유다. "돌고 있다"는 상태의 어휘이고,
"아직 정해지지 않았다"는 결과의 어휘다.

폴더 위치는 `conventions.md` 2항이 `Core/`를 **"GameState, 타이머, 이벤트"** 로
규정한 것을 그대로 따랐다.

## 대가

이번 범위에서 보스를 제외했으므로(0017) 페이즈 타입은 실제로 만들어지지 않는다.
따라서 이 결정의 이득은 **현 시점에 회수되지 않는다.**
비용이 파일 하나와 이름 하나뿐이라 선지불하는 쪽을 택했다.

## 재검토 시점

`spec.md`의 페이즈 상태 머신을 구현할 때. 그 시점에 `GameState`가
실제로 필요한 이름인지 확인한다.
