using UnityEngine;

public class WaveDirector : MonoBehaviour
{
    [SerializeField] private GameTimer _timer;
    [SerializeField] private EnemySpawner _spawner;
    [SerializeField] private WaveTable _table;

    //현재 몇 웨이브에 있는가 
    private int _currentWaveIndex = -1;

    void Awake()
    {
        if (_timer == null)
        {
            Debug.LogError("타이머가 연결되지 않았습니다!", this);
            enabled = false;
        }
        
        if (_spawner == null)
        {
            Debug.LogError("Spawner가 연결되지 않았습니다!", this);
            enabled = false;
        }
        
        if (_table == null || _table.Waves == null || _table.Waves.Count == 0)
        {
            Debug.LogError("유효한 Wave 테이블이 아닙니다!", this);
            enabled = false;
        }
    }

    void Start()
    {
        ApplyNextWave();
    }

    void Update()
    {
        int nextWaveIndex = _currentWaveIndex + 1;
        // Out of Range + 마지막 웨이브 이후 작동 중지
        if (nextWaveIndex >= _table.Waves.Count)
        {
            enabled = false;
            return;
        }

        var nextWave = _table.Waves[nextWaveIndex];
        // 다음 웨이브 시작 시간을 넘겼을 경우
        if (_timer.Elapsed < nextWave.StartTime)
        {
            return;
        }

        ApplyNextWave();
    }

    private void ApplyNextWave()
    {
        // 인덱스를 기준으로 wave 꺼내오기 + 증가시켜주기
        var wave = _table.Waves[++_currentWaveIndex];
        // 적용
        _spawner.SetSpawnParams(wave.Interval, wave.CountPerSpawn);
    }
}
