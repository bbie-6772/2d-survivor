using TMPro;
using UnityEngine;

public class GameRemainingText : MonoBehaviour
{
    [SerializeField] private GameTimer _timer;
    [SerializeField] private TMP_Text _remainingTime;
    private int _prevRemainingSecond;

    void Awake()
    {
        if (_timer == null)
        {
            Debug.LogError("타이머가 연결되지 않았습니다", this);
            enabled = false;
            return;
        }

        if (_remainingTime == null)
        {
            Debug.LogError("남은 시간을 나타낼 요소가 연결되지 않았습니다", this);
            enabled = false;
            return;
        }
    }

    void Start()
    {
        _prevRemainingSecond = Mathf.CeilToInt(_timer.Remaining);
        SetTimerText(_prevRemainingSecond);
    }

    void Update()
    {
        int currentRemainingSecond = Mathf.CeilToInt(_timer.Remaining);
        if(_prevRemainingSecond == currentRemainingSecond)
        {
            return;
        }   

        _prevRemainingSecond = currentRemainingSecond;
        SetTimerText(_prevRemainingSecond);
    }

    void SetTimerText(int currentSeconds)
    {
        // 남은시간 분, 초로 계산
        int minutes = currentSeconds / 60;
        int seconds = currentSeconds % 60;
        // UI 요소에 적용
        _remainingTime.text = $"{minutes}:{seconds:00}";
    }

}
