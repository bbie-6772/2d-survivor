# 0020. 심장 회수는 스포너 콜백으로 되돌리고, 획득 판정은 획득자가 소유한다

- 날짜: 2026-09-05 (D9)
- 상태: 채택
- 관련: 0007(콜백 주입), 0015(드랍 콜백), 0016(`Spawn()` protected)

## 맥락

`HeartSpawner`가 심장을 떨어뜨리기만 하고, 주운 뒤 사라지는 경로가 없었다.

처음에는 `HeartCollector`가 `HeartSpawner`를 직접 참조해 `Despawn`을 부르는 형태를
검토했으나 두 가지에 막힌다. **`Despawn`은 0016에서 `protected`로 좁혔으므로
밖에서 부를 수 없고**, 심장을 **줍는 쪽**이 심장을 **만드는 쪽**을 알게 되어
0015가 세운 절제(드랍 시 `EnemyDeath`는 `HeartSpawner`를 모르고 `Action<Vector2>`만 받음)가
회수에서만 깨진다.

두 번째로 검토한 것은 `HeartCollector`가 `event`를 발행하고 심장들이 구독하는 형태였다.
이것은 **인자 없는 이벤트에 심장 300개가 구독하는 구조**가 되어,
하나를 밟으면 바닥의 심장 전부가 자신이 먹혔다고 판단한다.
이벤트는 "누가 관심 있는지 발신자가 모를 때" 쓰는 도구인데,
여기서는 반대로 **정확히 하나를 지목해야 한다.** `OnTriggerEnter2D`의 `other`가
이미 그 하나를 들고 있다.

## 결정

**회수 경로는 드랍과 같은 방향(스포너 → 개체)의 콜백 주입으로 통일한다.**

`HeartSpawner.Setup`에서 `heart.SetReleaseCallback(Despawn)`을 주입한다.
`EnemySpawner.Setup`이 `death.SetReleaseCallback(Despawn)`을 하는 것과 같은 형태다.

**획득은 이벤트가 아니라 직접 호출이다.**

```csharp
// HeartCollector
if (!other.TryGetComponent<Heart>(out Heart heart)) { return; }
int value = heart.Collect();
_experience.Add(value);
```

`Heart.Collect()`는 **획득량을 반환하면서 자기 자신을 회수시킨다.**
읽기와 소멸이 한 호출로 묶여, 값만 받고 없애지 않거나 없애고 값을 받지 않는
실수가 구조적으로 불가능하다. 0016에서 `SpawnHeart`가 두 줄을 묶어둔 것과 같은 방어다.

## 판정 구조 — 두 층

| 층 | 담당 | 목적 |
|---|---|---|
| 1차 | Layer Collision Matrix (`Pickup × Player`만 허용) | 물리 검사 비용 절감 |
| 2차 | `TryGetComponent<Heart>` | 실제 판정 + 획득량 접근 |

중복이 아니라 역할이 다르다. `MeleeWeapon`이 `_targetLayer`로 후보를 좁힌 뒤
`IDamageable`로 최종 판정하는 구조와 같은 모양이다.

2차가 필요한 이유는 **어차피 `Heart` 컴포넌트를 꺼내야 하기 때문**이다.
꺼내는 행위 자체가 판정을 겸한다.

## 콜라이더 배치

**심장에는 `Rigidbody2D`를 붙이지 않는다.** trigger 이벤트는 둘 중 하나에만
`Rigidbody2D`가 있으면 발생하므로, 플레이어의 것으로 충분하다.
심장 300개에 각각 `Rigidbody2D`를 붙이는 비용을 피한다.

**획득 트리거는 플레이어 본체가 아니라 자식(`PickUpRange`)에 둔다.**
본체 콜라이더는 이미 `PlayerHitReceiver.OnCollisionStay2D`가 쓰고 있어,
같은 오브젝트에 트리거를 더하면 어느 이벤트가 어느 콜라이더에서 왔는지 구분되지 않는다.

Layer Collision Matrix에서 `Pickup × Pickup`, `Pickup × Enemy`를 해제했다.
해제하지 않으면 좀비가 심장을 밀고 다니고, 심장끼리도 충돌 검사를 한다.

## 참조 확보 방식

`HeartCollector`는 자식 오브젝트이므로 `PlayerExperience`를
`GetComponentInParent<PlayerExperience>()`로 `Awake`에서 확보한다.

0009는 씬 주입을 기본으로 정했으나, **부모-자식이 하나의 프리팹으로 묶여 있으면
씬 배선 실수가 발생할 수 없다.** `MeleeWeapon`이 같은 오브젝트의 `PlayerAim`을
`GetComponent`로 잡은 근거("같은 오브젝트까지는 초기설정으로 취급")를
프리팹 내부 계층까지 확장한 것이다.

`[RequireComponent(typeof(PlayerExperience))]`는 쓰지 않는다.
그것은 **같은 GameObject**에 강제하므로, 자식에 `PlayerExperience`를
하나 더 만들어 경험치가 두 곳으로 갈라진다.

## 대가

- `Heart`가 `Collect()`라는 이름으로 소멸까지 수행한다. 통보와 동작이 한 메서드에 있어
  이름만으로 소멸이 일어난다는 것이 드러나지 않는다.
- 획득 판정이 Layer Matrix(에디터)와 코드 두 곳에 걸쳐 있어,
  매트릭스를 잘못 만지면 코드는 정상인데 획득이 안 되는 상황이 가능하다.
  `HeartCollector`에 그 사실을 주석으로 남겼다.

## 재검토 시점

D6 풀링 도입 시. `Despawn`이 `Destroy`에서 풀 반납으로 바뀌면
`Heart` / `HeartCollector`는 **변경 없이** 그대로 동작해야 한다.
그렇지 않다면 이 결정의 격리가 부족했다는 뜻이다.
