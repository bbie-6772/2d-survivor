using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WaveTable", menuName = "Game/WaveTable")]
public class WaveTable : ScriptableObject
{
    [Serializable]
    public struct Wave
    {
        // 게임 시작 후 몇 초부터
        public float StartTime;
        // 스폰 간격을 조정하며 
        public float Interval;
        // 몇 마리씩 소환할 것인가
        public int CountPerSpawn;
    }

    [SerializeField] private Wave[] _waves;

    public IReadOnlyList<Wave> Waves => _waves;

    // 에디터 검증으로 값을 유효하게만 적용가능
    void OnValidate()
    {
        if (_waves == null)
        {
            return;
        }

        // 내부 wave 순회하며 검증
        for (int i = 0; i < _waves.Length; ++i)
        {
            if (_waves[i].Interval <= 0)
            {
                Debug.LogWarning($"{i+1}번째 Wave의 스폰 주기가 유효하지 않음 Value : {_waves[i].Interval}", this);
            }
            if (_waves[i].CountPerSpawn <= 0)
            {
                Debug.LogWarning($"{i+1}번째 Wave의 스폰량이 유효하지 않음 Value : {_waves[i].CountPerSpawn}", this);
            }
            // Out of Range 방지
            if (i > 0)
            {
                // 이전 웨이브보다 시작 시간이 빠를 떄
                if (_waves[i].StartTime <= _waves[i-1].StartTime)
                {
                    Debug.LogWarning($"{i+1}번째 Wave의 시작 시간이 이전 Wave보다 빠름", this);
                }
            }
        }
    }
}
