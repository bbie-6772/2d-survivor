# 프로젝트 컨벤션

혼자 진행하는 프로젝트지만, 협업을 전제로 한 코드를 작성하기 위해 규칙을 먼저 고정한다.
아래 규칙은 프로젝트 시작 시점에 확정했으며, 변경 시 `docs/decisions/` 에 이유를 기록한다.

---

## 1. C# 네이밍

| 대상 | 규칙 | 예 |
|---|---|---|
| 클래스 | PascalCase | `ZombieSpawner` |
| 메서드 | PascalCase | `TakeDamage()` |
| 프로퍼티 | PascalCase | `CurrentHp` |
| 지역변수, 매개변수 | camelCase | `moveInput`, `damageAmount` |
| private 필드 | `_camelCase` | `_currentHp` |
| `[SerializeField] private` | `_camelCase` | `_moveSpeed` (인스펙터엔 "Move Speed"로 표시) |
| 상수 | PascalCase | `MaxZombieCount` |
| 인터페이스 | `I` + PascalCase | `IDamageable` |
| enum (타입/멤버) | PascalCase | `PlayerState.Dashing` |

### 규칙

- **`public` 필드를 사용하지 않는다.**
  - 인스펙터 노출이 필요하면 → `[SerializeField] private`
  - 외부 읽기가 필요하면 → `public int CurrentHp { get; private set; }`
- private 필드의 `_` 접두는 **지역변수와 필드를 눈으로 구분**하기 위해 채택한다.
  이 프로젝트의 주제가 상태 관리(FSM, 풀링)이므로, "이 값이 프레임을 넘어 살아남는가"가 즉시 보이는 편이 유리하다.
- 일관성이 규칙 자체보다 중요하다. 중간에 스타일을 바꾸지 않는다.

---

## 2. 폴더 구조

```
Assets/
  _Project/
    Scripts/
      Player/
      Enemy/
      Weapon/
      Core/          # GameState, 타이머, 이벤트
      Data/          # ScriptableObject 정의 (코드)
      UI/
      Utils/
    Prefabs/
    Data/            # ScriptableObject 에셋 인스턴스 (.asset)
    Art/
    Scenes/
  Plugins/           # 외부 에셋
```

- `_Project/` 언더스코어: 임포트한 외부 에셋과 직접 작성한 자산을 분리한다.
- `Scripts/Data`(코드)와 `Data`(에셋 인스턴스)는 다른 물건이므로 분리한다.
  - `EnemyStats.cs` → `Scripts/Data/`
  - `Zombie_Basic.asset` → `Data/`
- 규모가 적은 프로젝트 이기에 사람이 파일 찾기 쉬운 구조 **기능별(타입별 대신)** 분류를 사용한다.
  `Managers/`, `Interfaces/` 같은 타입별 폴더는 관련 코드를 흩어놓는다.

---

## 3. 에셋 네이밍

| 종류 | 규칙 | 예 |
|---|---|---|
| 프리팹 | PascalCase | `Zombie_Basic`, `Projectile_Shotgun` |
| ScriptableObject 에셋 | `카테고리_이름` | `Weapon_Sniper`, `Wave_01` |
| 씬 | PascalCase | `MainGame`, `Title` |

- `PF_`, `SO_` 같은 접두사는 사용하지 않는다. 폴더로 이미 구분됨
- **금지**: `Zombie 1`, `New Prefab`, `Untitled` 등 기본 생성 이름 방치.

---

## 4. Git

### 초기 설정 (D0에 반드시 완료)

1. **`.gitignore`** — Unity 공식 템플릿 사용 (`github/gitignore` 저장소).
   `Library/`, `Temp/`, `Logs/`, `Build/` 제외 확인.
2. **Project Settings > Editor**
   - Asset Serialization: **Force Text** (git diff 추적)
   - Version Control: **Visible Meta Files** (.meta 파일을 노출하여 유니티 참조가 유지되도록 함)
   - 씬·프리팹이 바이너리면 diff와 merge가 불가능

### 커밋 메시지 (Conventional Commits)

```
feat(player): 대시 입력과 무적 판정 추가
fix(spawn): 풀 반환 시 이전 히트 기록이 남던 문제 수정
refactor(fsm): enum switch를 상태 인터페이스로 분리
perf(pool): 투사체 생성을 오브젝트 풀로 대체
docs(decision): 관통 구현을 D7로 미룬 이유 기록
chore: gitignore 추가
```

- **`refactor`와 `perf`는 반드시 독립 커밋으로 분리한다.**
  기술 축 2번(FSM 리팩토링)과 3번(성능 최적화)의 증거가 이 커밋 로그다.
  기능 커밋에 섞이면 증거가 사라진다.

### 브랜치

- 기본은 `main` 직접 커밋 (1인 / 10일 프로젝트).
- **예외**: 성능 실험 구간(D6~D7)은 `perf/object-pooling` 브랜치에서 작업 후 merge.
- 최적화 전 상태에 태그를 남긴다: `v0.1-before-pooling`
  → 수치 재검증과 비교 스크린샷 재촬영이 가능해진다.

---

## 5. Unity 코드 관례

이번 프로젝트(동시 300마리 스폰)에 직접 영향을 주는 것만 채택한다.

- **`GetComponent`는 `Awake`에서 캐싱한다.** Update 호출은 매 프레임 탐색이다.
- **`Awake`는 자기 자신의 초기화, `Start`는 다른 오브젝트와의 연결.**
  실행 순서 관련 버그 대부분이 이 구분을 지키지 않아 발생한다.
- **태그 비교는 `CompareTag()`.** `gameObject.tag == "Zombie"`는 GC alloc이 발생한다.
- **`Camera.main`을 Update에서 호출하지 않는다.** 내부적으로 태그 검색이므로 캐싱한다.
- **`#region`을 남발하지 않는다.** 접어야 할 만큼 길다면 클래스를 나눌 시점이다.

---

## 6. 클래스 책임

- **파일 이름만 보고 그 클래스의 책임을 한 문장으로 말할 수 없으면 잘못된 이름이다.**
- `GameManager.cs` 같은 이름을 만들지 않는다. 무슨 일을 하는지 이름에 담기지 않는다는 것은 책임이 아직 나뉘지 않았다는 뜻이며, 그런 파일은 며칠 안에 수백 줄이 된다.
- 검증 시점: D2 (스크립트 5~6개가 쌓였을 때 이름만으로 책임 확인)

---

## 7. 도구 설정

- `.editorconfig` 를 저장소 루트에 둔다. Rider / Visual Studio가 위 네이밍 규칙을 자동 검사한다.