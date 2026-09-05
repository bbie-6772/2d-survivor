# 0025. 다중 레벨업은 카운터 큐로 직렬화하고, 창의 개폐 신호는 선택기가 소유한다

- 날짜: 2026-09-05 (D9)
- 상태: 채택
- 관련: **0021의 미해결 항목 해소**, 0019(전역 정지), 0022(스탯 적용)

## 맥락

0021이 미해결로 남긴 문제가 있었다. `PlayerExperience.Add`의 `while`이 여러 바퀴 돌면
`OnLevelUp`이 **한 프레임에 여러 번 발화**한다. 구독자가 정지 후 카드 3장을 띄우는
코드라면 두 번째 발화가 첫 번째 창을 덮어 플레이어가 레벨을 잃는다.

**이것은 예외 상황이 아니다.** `OnTriggerEnter2D`는 같은 프레임에 여러 콜라이더에 대해
각각 호출되므로, 심장이 몰려 있는 곳을 지나가면 그대로 발생한다.
실제 테스트에서 한 프레임 3회 발화를 확인했다.

## 결정

### 큐는 정수 카운터 하나

`CardSelector`가 `_pendingLevelUps`를 소유한다.

```
HandleLevelUp : ++_pendingLevelUps; 이미 열려 있으면 return; 아니면 OpenSelection()
OpenSelection : timeScale = 0 → 3장 뽑기 → OnSelectionOpened 발화
Select(card)  : 가드 → Apply → --_pendingLevelUps
                남았으면 OpenSelection(), 없으면 timeScale = 1 + OnSelectionClosed 발화
```

**별도의 `_working` 플래그를 두지 않는다.** 상태 변수가 둘이면 어긋날 수 있고,
카운터만으로 "열려 있는가"가 판별된다.
`GameOutcome.Pending`이 1회 가드를 겸한 것과 같은 계열이다.

`_pendingLevelUps == 1`이 "열려 있다"와 "방금 열렸다"를 겸하는 성질을 갖는다.

### `Select`는 public이며 가드가 필수

UI 버튼이 부르므로 열어야 한다(`GameEndScreen.Restart`와 같은 근거).
**public이라는 것은 언제 불릴지 모른다는 뜻**이므로,
대기 중인 레벨업이 없으면 즉시 return한다.
가드가 없으면 중복 클릭 시 카운터가 음수가 되고 창이 뜬 채 게임이 재개된다.

### 창을 닫는 신호도 `CardSelector`가 발행한다

`OnSelectionOpened`의 짝으로 `OnSelectionClosed`를 둔다.

**재진입 때문이다.** 화면이 `Select` 호출 직후 스스로 패널을 끄면,
`Select` 안에서 큐가 남아 `OpenSelection`이 다시 불린 경우
**이미 켜진 패널을 그 다음 줄에서 다시 끄게 된다.** 창 없이 멈춘 게임이 된다.

두 줄의 순서를 바꾸면 우연히 동작하지만, 우연히 맞는 코드는 다음 수정에서 조용히 깨진다.
개폐 신호를 둘 다 발행자가 소유하면 **화면의 클릭 핸들러가 `Select` 한 줄로 줄고
순서 의존이 사라진다.**

그리고 "다 끝났는가"를 아는 것은 `_pendingLevelUps`를 가진 `CardSelector`뿐이다.

### 뽑기

`_pool`을 `List`로 **복사한 뒤** 뽑을 때마다 제거해 중복을 막는다.
`_pool` 자체를 섞지 않는 이유는 SO 참조 배열이라 원본 순서가 흐트러지면
인스펙터에서 확인한 배치와 달라지기 때문이다.

이벤트 인자는 `IReadOnlyList<UpgradeCard>`로 싣는다.
`List`를 그대로 넘기면 구독자가 수정할 수 있고, `ToArray()`는 할당이 하나 는다.
`event`가 `add`/`remove`만 여는 것과 같은 발상이다.

`_choiceCount`를 `[SerializeField]`로 두고 `public int ChoiceCount`로 공개한다.
화면이 버튼 개수를 검증할 때 읽는다.

### 검증

`_pool`의 원소 null까지 `Awake`에서 검사하고 **인덱스를 로그에 넣는다.**
빈 슬롯은 에디터 배선 실수이며, 뽑기에서 조용히 걸러내면 카드가 안 나오는 것을
눈치채기 어렵다(0016의 "조용히 틀리는 버그" 기준).

`_buttons.Length >= _selector.ChoiceCount` 검증은 **`Start`에서** 한다.
다른 오브젝트의 값을 읽으므로 컨벤션 5항을 따른다.

## 대가

- `_pendingLevelUps == 1`이 두 의미를 겸하므로, 나중에 "창은 열렸지만 아직 카드가
  안 뽑힌 상태" 같은 중간 단계가 필요해지면 카운터만으로는 표현할 수 없다.
- `timeScale = 0`을 거는 주체가 `GameEnder`와 `CardSelector` 둘이 되었다.
  현재는 겹치지 않지만(창이 열린 동안 게임이 끝날 수 없다) 정지 소유권이 분산됐다.

## 재검토 시점

보스 페이즈 구현 시. `timeScale` 정지 주체가 셋이 되면 소유권을 한 곳으로 모은다(0019).

## 부수 확인 — 구독 해제는 조건부다

`Start`에서 건 구독은 `-=`하지 않기로 했다. 판단 기준 둘.

1. 구독 코드가 두 번 이상 실행되는가 (`OnEnable`은 그렇고 `Start`는 아니다)
2. 발행자보다 구독자가 먼저 파괴될 수 있는가

둘 다 아니면 생략한다. 씬 리로드로 재시작하므로 발행자와 구독자가 함께 사라진다.
`EnemyDeath` / `DamageFlash`가 `OnEnable`/`OnDisable` 대칭을 지키는 것은 1번에 해당하기 때문이다.
