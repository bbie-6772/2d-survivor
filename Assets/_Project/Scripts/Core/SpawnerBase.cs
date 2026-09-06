using UnityEngine;

// abstract로 스폰위치 및 기본 설정을 오버라이드 강제 
// 자식은 Awake 메서드 대신 OnInit 을 사용해야함
public abstract class SpawnerBase : MonoBehaviour
{
    // 소환할 프리팹 설정(인스펙터)
    [SerializeField] protected GameObject _prefab; 

    // 프리팹 존재여부 확인 기능
    protected void Awake()
    {
        // 자식의 Awake 초기화 실행
        OnInit();

        if (_prefab)
        {
            return;
        }

        // 프리팹 미설정 경고 남기기
        Debug.LogError("Prefab을 선택하지 않았습니다.", this);
        // 컴포넌트에서 제외
        enabled = false;
    }

    protected GameObject Spawn()
    {   
        // 초기설정 오류 시 발동 안돼도록 가드
        if (!enabled) return null;
        //Instantiate 메서드를 이용해 _prefab 생성, 인자로는 위치와 회전율(2D이기에 회전없음) 제공
        GameObject obj = Instantiate(_prefab, GetSpawnPosition(), Quaternion.identity);
        // 오버라이드한 메서드 발동 + 성공 여부 확인
        bool isReady = Setup(obj);
        if (!isReady)
        {
            // 실패 시 제거 후 null 반환
            Destroy(obj);
            return null;
        }
        // 생성한 객체 반환
        return obj;
    }

    protected void Despawn(GameObject obj)
    {
        Cleanup(obj);
        Destroy(obj);
    }
    // 스폰위치 구현 강제
    protected abstract Vector2 GetSpawnPosition();
    // 초기화 구현 강제
    protected abstract bool Setup(GameObject obj);
    // 자식의 Awake 대체 메서드 ( 부모의 Awake가 무시되는 상황 방지 )
    protected virtual void OnInit() {}
    // 소멸 이벤트 확장성
    protected virtual void Cleanup(GameObject obj) {}
}
