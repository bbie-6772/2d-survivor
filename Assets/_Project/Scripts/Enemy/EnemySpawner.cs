using System.Collections;
using UnityEngine;

public class EnemySpawner : SpawnerBase
{
    // 스폰 지점이 되는 원의 반지름
    [SerializeField] private float _spawnRadius = 20f;

    // 인스펙터 지정으로 실행 전 연결 보장
    [SerializeField] private Transform _target;
    // 스폰 주기
    [SerializeField] private float _interval = 5f;

    // 루프마다 새로만들지 않도록 객체 캐싱 GC(가비지컬렉터) 최적화
    private WaitForSeconds _wait;

    void Start()
    {
        // SpanwLoop 에서 Spanw()(외부참조)를 하기에 Start에서 진행
        StartCoroutine(SpawnLoop()); 
    }
    
    // 코루틴 타이머로 스폰주기 설정
    IEnumerator SpawnLoop()
    {
        while (true)
        {
            Spawn();
            yield return _wait;
        }
    }

    // SpanwerBase의 Awake가 무시되지 않고 실행되도록 만든 Awake 훅
    protected override void OnInit()
    {
        if (_target)
        {
            // wait 프로퍼티 캐싱 
            _wait = new WaitForSeconds(_interval);
            return;
        }

        Debug.LogError("타겟을 지정하지 않았습니다.", this);
        enabled = false;
    }

    protected override Vector2 GetSpawnPosition()
    {
        // 타겟의 지점에서 spawnRadius로 반지름를 갖는 원 위에 생성
        Vector2 spawnPoint = (Vector2)_target.position + (Random.insideUnitCircle.normalized * _spawnRadius);

        return spawnPoint;
    }  

    protected override void Setup(GameObject obj)
    {
        EnemyChase enemy = obj.GetComponent<EnemyChase>();
        if (enemy)
        {
            enemy.Init(_target);
            return;
        }

        // 프리팹 미설정 경고 남기기
        Debug.LogError("Prefab에 EnemyChase가 없습니다.", obj);
    }
}
