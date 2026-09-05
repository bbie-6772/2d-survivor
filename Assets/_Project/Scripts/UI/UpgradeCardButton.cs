using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(Button))]
public class UpgradeCardButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _title;

    private Button _button;
    private UpgradeCard _card;
    private Action<UpgradeCard> _onClicked;

    void Awake()
    {
        if (_title == null)
        {
            Debug.LogError("텍스트 지정 안됐습니다", this);
            enabled = false;
            return;
        }

        // 버튼 캐싱
        _button = GetComponent<Button>();
        // 버튼 이벤트(누를 시) 에 HandleClick 매핑
        _button.onClick.AddListener(HandleClick);
    }

    public void SetCallback(Action<UpgradeCard> onClicked)
    {
        _onClicked = onClicked;
    }

    public void Bind(UpgradeCard card)
    {
        _card = card;
        _title.text = card.Title;
    }

    void HandleClick()
    {
        if (_button == null)
        {
            Debug.LogError("버튼이 매핑되지 않았습니다.", this);
            return;
        }

        if (_card == null)
        {
            Debug.LogWarning("card가 비어있어 제대로 동작하지 못했습니다.", this);
            return;
        }

        if(_onClicked == null)
        {
            Debug.LogWarning("onClicked 가 비어있어 제대로 동작하지 못했습니다.", this);
            return;
        }

        _onClicked.Invoke(_card);
    }

}
