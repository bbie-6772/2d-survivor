# 0026. UI 버튼도 콜백 주입으로 연결하고, 리스너는 `Awake`에서 한 번만 등록한다

- 날짜: 2026-09-05 (D9)
- 상태: 채택
- 관련: **0024(람다 구독 금지)의 첫 적용**, 0007 / 0015 / 0020(콜백 주입), 0019(UI 부착 위치)

## 맥락

카드 버튼 3장은 **누를 때마다 다른 카드**를 넘겨야 한다.
인스펙터의 On Click은 인자를 고정값으로만 전달하므로 배선으로 해결되지 않는다.
`GameEndScreen.Restart`가 항상 같은 일을 하는 것과 다른 점이다.

코드로 붙이면 두 가지 함정이 동시에 열린다.

```csharp
_buttons[i].onClick.AddListener(() => Select(cards[i]));
```

1. **람다는 뗄 수 없다**(0024). 창을 열 때마다 붙이면 리스너가 쌓이고,
   두 번째 레벨업에서 한 번 클릭에 `Select`가 두 번 불린다.
2. **`for` 루프 변수 캡처.** 람다가 `i`를 변수 자체로 붙잡아
   세 버튼이 모두 루프 종료 후의 값을 본다.

## 결정

**B안: 버튼마다 전용 컴포넌트를 둔다.**

| 후보 | 내용 | 판단 |
|---|---|---|
| A | 창을 열 때마다 `RemoveAllListeners()` 후 재등록 | 짧지만 루프 캡처는 여전히 조심해야 하고, 매번 리스너를 재구성한다 |
| **B** | **버튼별 컴포넌트가 자기 카드를 소유. 리스너는 `Awake`에 한 번** | **람다도 루프 캡처도 발생하지 않는다** |

`UpgradeCardButton`은 셋을 안다. 자기 카드, 자기 텍스트, 눌렸을 때 알릴 콜백.
다른 버튼도 `CardSelector`도 패널도 모른다.

```csharp
Awake       : Button 캐싱 + onClick.AddListener(HandleClick)   ← 평생 한 번, 명명된 메서드
SetCallback : _onClicked 주입                                   ← 초기화 시 한 번
Bind(card)  : _card 교체 + _title.text 갱신                     ← 창이 열릴 때마다
HandleClick : _onClicked(_card)                                 ← 인자 없는 UnityEvent에서 호출
```

**`SetCallback`과 `Bind`를 나눈 이유는 생명주기가 다르기 때문이다.**
콜백은 한 번, 카드는 매번. 합치면 창을 열 때마다 콜백까지 다시 넘겨야 한다.

**`onClick`이 인자를 못 넘기는 문제는 필드로 해결된다.**
`HandleClick`이 자기 `_card`를 읽으면 되므로 클로저가 필요 없다.

## 이유

버튼이 `_selector.Select(_card)`를 직접 부르게 하면 버튼 3개가 각각
`CardSelector` 참조를 물어야 해 배선이 3배가 된다.

콜백 주입이면 **부모 화면이 자기 메서드 하나를 세 버튼에 나눠주기만 하면 된다.**
`EnemySpawner`가 좀비마다 `Despawn`을 쥐어주고(0007),
`HeartSpawner`가 심장마다 회수 콜백을 쥐어주는(0020) 것과 같은 형태다.
**이 패턴이 스폰·드랍·회수·UI에서 반복되면서 프로젝트의 표준 연결 방식이 되었다.**

## 함께 확정된 것

### `Screen` 접미 규칙

> **`~Screen`은 Canvas에 붙어 이벤트를 받아 패널을 제어하는 컴포넌트다.**
> 패널 자체가 아니라 `[SerializeField] GameObject`로 참조한다.

`GameEndScreen`, `UpgradeSelectScreen` 둘 다 해당한다.
0019가 정한 "비활성 오브젝트 위의 컴포넌트는 `Start`도 호출되지 않는다"의 직접적 귀결이다.

처음 이름이 `UpgradePanel`이었으나, 실체가 패널이 아니라 패널의 제어자이므로 변경했다.

### 확장은 지금 하지 않는다

버튼을 프리팹화해 `_choiceCount`만큼 생성하고 `Horizontal Layout Group`에 맡기면
개수가 바뀌어도 코드도 씬도 안 건드린다.

**이번 범위에서 카드 개수가 3에서 바뀔 일이 없으므로(0017) 고정 배열로 둔다.**
`UpgradeCardButton`의 `Bind` / `SetCallback` 인터페이스가 그대로이므로
나중에 생성 방식으로 바꿔도 버튼 쪽은 손댈 것이 없다.

## 대가

- 클래스가 하나 늘었다(`UpgradeCardButton`).
- `_buttons`가 고정 배열이라 `_choiceCount`를 바꾸면 씬에서 버튼을 직접 늘려야 한다.
  `Start` 검증이 이를 잡아주지만 자동은 아니다.
- `enabled = false`는 `UnityEvent` 발화를 막지 못한다.
  버튼 컴포넌트를 비활성해도 `onClick`은 그대로 불리므로,
  검증 실패 시 방어는 `HandleClick` 내부의 null 가드가 담당한다.

## D9에 발견된 운영 문제

`UpgradeSelectScreen`의 `_buttons` 배선을 빠뜨렸을 때 **증상이 "레벨업 후 화면이 멈춤"** 으로 나타났다.
검증이 `enabled = false`로 자기를 껐지만, `CardSelector`가 이미 건 `timeScale = 0`은 풀리지 않았다.

> **`enabled = false`는 자기 콜백만 멈춘다. 남이 건 전역 상태는 되돌리지 않는다.**

에디터에서는 콘솔로 원인을 찾을 수 있으나 빌드에서는 그냥 멈춘 게임이다.
0019의 검증 방식이 가진 한계로 기록해둔다.

## 재검토 시점

카드 개수가 3에서 바뀌거나, 카드에 아이콘·설명 등 표시 요소가 늘어날 때.
