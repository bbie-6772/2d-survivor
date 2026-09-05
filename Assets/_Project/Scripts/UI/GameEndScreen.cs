using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameEndScreen : MonoBehaviour
{
    // 게임 종료 참조
    [SerializeField] private GameEnder _ender;
    // UI 요소 매핑
    [SerializeField] private GameObject _panel;
    [SerializeField] private TMP_Text _message;

    void Awake()
    {
        if (_ender == null)
        {
            Debug.LogError("게임 종료가 연결되지 않았습니다", this);
            enabled = false;
        }

        if (_panel == null)
        {
            Debug.LogError("UI 요소가 매핑되지 않았습니다", this);
        }

        if (_message == null)
        {
            Debug.LogError("메시지 요소가 매핑되지 않았습니다", this);
        }
    }

    void Start()
    {
        // 게임 끝에 UI 요소 핸들러 구독
        _ender.OnGameEnd += HandleGameEnd;
    }

    void HandleGameEnd(GameOutcome outcome)
    {
        // 결과에 맞춰 UI 요소 수정
        switch (outcome)
        {
            case GameOutcome.Win:
                _message.text = "WIN!!";
                break;
            case GameOutcome.Lose:
                _message.text = "LOSE...";
                break;
            default:
                Debug.LogError("GAME END SCREEN ERROR", this);
                break;
        }

        // 매핑된 결과요소 UI 활성화
        _panel.SetActive(true);
    }

    public void Restart()
    {
        // 재시작 시 현재 씬을 다시 부르도록 설정
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}
