using System;
using UnityEngine;

public class GameEnder : MonoBehaviour
{
    // 게임 종료 조건 참조용 필드, 씬에서 삽입하도록 설계
    [SerializeField] private GameTimer _timer;
    [SerializeField] private Health _playerHealth;
    private GameOutcome _outcome;
    public event Action<GameOutcome> OnGameEnd;

    void Awake()
    {
        // 게임 "진행중" 상태로 초기화
        _outcome = GameOutcome.Pending;
        Time.timeScale = 1f;

        if (_timer == null)
        {
            Debug.LogError("Timer가 지정되지 않았습니다!", this);
            enabled = false;
        }

        if (_playerHealth == null)
        {
            Debug.LogError("플레이어의 체력이 매핑되지 않았습니다!!", this);
            enabled = false;
        }
    }

    void Start()
    {
        _timer.OnExpired += HandleWin;
        _playerHealth.OnDied += HandleLose;
    }

    void HandleWin()
    {
        HandleResult(GameOutcome.Win);
    }

    void HandleLose()
    {
        HandleResult(GameOutcome.Lose);
    }

    private void HandleResult(GameOutcome result)
    {
        // 게임 진행중 일 때만 1번 결과를 판정하도록 설계
        if (_outcome != GameOutcome.Pending)
        {
            return;
        }
        _outcome = result;

        // 게임 정지 
        Time.timeScale = 0f;
        // 기존 구독한 내역 정리
        _timer.OnExpired -= HandleWin;
        _playerHealth.OnDied -= HandleLose;
        // 추가 게임 종료 액션 진행
        OnGameEnd?.Invoke(_outcome);
    }
}
