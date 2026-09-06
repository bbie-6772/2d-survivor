using System;
using System.Collections;
using UnityEngine;

public class EnemySpawner : SpawnerBase
{
    // 스폰 지점이 되는 원의 반지름
    [SerializeField] private float _spawnRadius = 20f;

    // 인스펙터 지정으로 실행 전 연결 보장
    [SerializeField] private Transform _target;
    [SerializeField] private HeartSpawner _heartSpawner;
    [SerializeField] private Vector2 _mapMin = new Vector2(-30f, -30f);
    [SerializeField] private Vector2 _mapMax = new Vector2(30f, 30f);
    // 2D 물리 겹침의 부하를 측정하여 적절한 값을 도출하였음 (문서 0030 확인)
    [SerializeField] private int _limitAliveCount = 600;
    // 스폰주기 초기 값
    private float _interval = 5f;
    // 스폰량 초기 값
    private int _countPerSpawn = 5;

    // 루프마다 새로만들지 않도록 객체 캐싱 GC(가비지컬렉터) 최적화
    private WaitForSeconds _wait;
    // UI 및 확장성을 위한 함수
    public int AliveCount { get; private set; }
    public event Action OnSpawn;
    public event Action OnDespawn;
    

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
            // 1회 생성량에 맞춰 반복
            for (int i = 0; i < _countPerSpawn; ++i)
            {
                Spawn();
                // 최대 생성 수 제한
                if(AliveCount >= _limitAliveCount)
                {
                    break;
                }

                ++AliveCount;
                OnSpawn?.Invoke();
            }
            yield return _wait;
        }
    }

    // SpanwerBase의 Awake가 무시되지 않고 실행되도록 만든 Awake 훅
    protected override void OnInit()
    {
        if (_target == null)
        {
            Debug.LogError("타겟을 지정하지 않았습니다.", this);
            enabled = false;
            return;
        } 

        if (_heartSpawner == null)
        {
            Debug.LogWarning("HeartSpanwer가 설정되지 않았습니다", this);
        }

        // wait 필드 캐싱 
        _wait = new WaitForSeconds(_interval);
    }

    protected override Vector2 GetSpawnPosition()
    {
        // 타겟의 지점에서 spawnRadius로 반지름를 갖는 원 위에 생성
        Vector2 spawnPoint = (Vector2)_target.position + (UnityEngine.Random.insideUnitCircle.normalized * _spawnRadius);
        // 맵 영역 밖에서 스폰되지 않도록 Clamp 적용 
        spawnPoint = Vector2.Min(Vector2.Max(spawnPoint, _mapMin), _mapMax);

        return spawnPoint;
    }  

    protected override void Setup(GameObject obj)
    {

        EnemyChase chase = obj.GetComponent<EnemyChase>();
        if (chase == null)
        {
            // 프리팹 미설정 경고 남기기
            Debug.LogError("Prefab에 EnemyChase가 없습니다.", obj);
            return;
        }

        EnemyDeath death = obj.GetComponent<EnemyDeath>();
        if (death == null)
        {
            // 프리팹 미설정 경고 남기기
            Debug.LogError("Prefab에 EnemyDeath가 없습니다.", obj);
            return;
        }

        chase.Init(_target);
        death.SetReleaseCallback(Despawn);
        if (_heartSpawner != null)
        {
            death.SetDropHeartCallback(_heartSpawner.SpawnHeart);
        }
    }

    // 웨이브 시스템을 적용 시켜주는 메서드
    public void SetSpawnParams(float interval, int countPerSpawn)
    {
        // 값 적용 
        _interval = interval;
        _countPerSpawn = countPerSpawn;
        // 스폰 지연 시간 재설정
        _wait = new WaitForSeconds(_interval);
    }

    protected override void Cleanup(GameObject obj)
    {
        --AliveCount;
        OnDespawn?.Invoke();
    }
}
