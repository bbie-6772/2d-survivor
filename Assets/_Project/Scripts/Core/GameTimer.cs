using System;
using UnityEngine;

public class GameTimer : MonoBehaviour
{
    // 타이머 총 시간을 조절하는 필드 
    [SerializeField] private float _duration = 300f;
    private bool _alreadyExpired;
    // 남은 시간 읽기만을 위한 필드 구조
    public float Remaining { get; private set;}
    // field-like event 외부에서 add, remove 만 가능하도록 event로 제한
    // 구독 순서대로 작동함
    public event Action OnExpired;

    void Awake()
    {
        // 타이머 시간 초기화
        Remaining = _duration;
    }

    void Update()
    {
        // 계산 최소화
        if (_alreadyExpired)
        {
            return;
        }

        // 소요 시간에 맞춰 Remaining 감소 
        // 물리요소 아님 + 이론상 비용이 적어질 수 있기에 Update 에서 진행
        Remaining = Mathf.Max( Remaining - Time.deltaTime, 0f);

        // 중복 호출을 막도록 설계 + 조건 순서에 따라 early escape
        if (!_alreadyExpired && Remaining <= 0f)
        {
            _alreadyExpired = true;
            OnExpired?.Invoke();
        }
    }
}
