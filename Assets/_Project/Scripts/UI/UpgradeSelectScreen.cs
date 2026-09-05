using System.Collections.Generic;
using UnityEngine;

public class UpgradeSelectScreen : MonoBehaviour
{
    [SerializeField] private CardSelector _selector;
    [SerializeField] private GameObject _panel;
    [SerializeField] private UpgradeCardButton[] _buttons;

    void Awake()
    {
        // 늘 먹던 필드 검증
        if (_selector == null)
        {
            Debug.LogError("카드 선택기가 매핑되지 않았습니다.", this);
            enabled = false;
        }

        if (_panel == null)
        {
            Debug.LogError("패널이 매핑되지 않았습니다.", this);
            enabled = false;
        }
    }

    void Start()
    {
        // 외부 값을 읽어오는 작업이 필요하기에 Start에서 진행
        if (_buttons == null || _buttons.Length < _selector.ChoiceCount)
        {
            Debug.LogError("버튼이 최소 요구치를 넘지 못했습니다.", this);
            enabled = false;
            return;
        }

        for (int i = 0; i < _buttons.Length; ++i)
        {
            if (_buttons[i] == null)
            {
                Debug.LogError($"{i+1} 번째 버튼이 올바르지 않습니다!", this);
                enabled = false;
            }
        }


        // 버튼 활성화 + 매핑 선택 진행
        _selector.OnSelectionOpened += HandleSelectionOpened;
        // 마무리 시 화면 끄도록 매핑 
        _selector.OnSelectionClosed += HandleSelectionClosed;
        // 버튼 클릭 시 로직 실행하도록 콜백 넣어주기
        foreach(var button in _buttons)
        {
            button.SetCallback(HandleCardClicked);
        }
    }

    void HandleSelectionOpened(IReadOnlyList<UpgradeCard> cards)
    {
        // 순서대로 받은 카드들 매핑
        for (int i = 0; i < cards.Count; ++i)
        {
            _buttons[i].Bind(cards[i]);
        }
        // 패널 활성화
        _panel.SetActive(true);
    }

    void HandleCardClicked(UpgradeCard card)
    {
        // 고른 카드 확정 및 적용
        _selector.Select(card);
    }

    void HandleSelectionClosed()
    {
        // 화면 내리기
        _panel.SetActive(false);
    }
}
