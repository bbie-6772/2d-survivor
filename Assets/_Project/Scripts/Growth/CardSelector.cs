using System;
using System.Collections.Generic;
using UnityEngine;

public class CardSelector : MonoBehaviour
{
    [SerializeField] private PlayerExperience _experience;
    [SerializeField] private PlayerUpgradeApplier _applier;
    [SerializeField] private UpgradeCard[] _pool;
    [SerializeField] private int _choiceCount = 3;

    private int _pendingLevelUps;

    // UI가 읽는 용도로 IReadOnly 속성의 List 사용
    public event Action<IReadOnlyList<UpgradeCard>> OnSelectionOpened;

    // // ===== 임시 검증용 (UI 연결 후 제거) =====
    // private IReadOnlyList<UpgradeCard> _debugCards;
    // void HandleSelectionOpenedDebug(IReadOnlyList<UpgradeCard> cards)
    // {
    //     // TODO: cards를 _debugCards에 보관하고, 카드 이름 3개를 로그로 출력
    //     _debugCards = cards;

    //     for (int i = 0; i < _debugCards.Count; ++i)
    //     {
    //         Debug.Log($"{i+1}번째 카드 {_debugCards[i].Title}");
    //     }
    // }
    // void Update()
    // {
    //     if (_debugCards == null || _debugCards.Count < 1)
    //         return;
    // // 개똑똑한 enum 산술 ㄷㄷ
    //     for (int i = 0; i < _debugCards.Count; ++i)
    //     {
    //         if (Input.GetKeyDown(KeyCode.Alpha1 + i)) { Select(_debugCards[i]); }
    //     }
    // 
    // // ========================================

    void Awake()
    {
        if (_experience == null)
        {
            Debug.LogError("PlayerExperience가 지정되지 않았습니다!", this);
            enabled = false;
        }

        if (_applier == null)
        {
            Debug.LogError("PlayerUpgradeApplier가 지정되지 않았습니다!", this);
            enabled = false;
        }

        if (_pool == null || _pool.Length < _choiceCount)
        {
            Debug.LogError($"최소 필요카드 {_choiceCount}장이 입력되지 않았습니다!", this);
            enabled = false;
        }

        for (int i = 0; i < _pool.Length; ++i)
        {
            if (_pool[i] == null)
            {
                Debug.LogError($"{i+1} 번째 카드가 입력되지 않았습니다!", this);
                enabled = false;
            }
        }

        OnSelectionOpened += HandleSelectionOpenedDebug;
    }

    void Start()
    {
        _experience.OnLevelUp += HandleLevelUp;
    }

    void HandleLevelUp()
    {
        ++_pendingLevelUps;
        // 이미 진행중인 작업이 있다면 무시
        if (_pendingLevelUps > 1)
        {
            return;
        }

        OpenSelection();
    }

    void OpenSelection()
    {
        // 게임 일시정지
        Time.timeScale = 0f;

        List<UpgradeCard> tempCards = new(_pool);
        List<UpgradeCard> selectCards = new();

        // 요소 중 랜덤 카드 선택
        while (selectCards.Count < _choiceCount)
        {
            int randomIndex = UnityEngine.Random.Range(0, tempCards.Count);
            selectCards.Add(tempCards[randomIndex]);
            tempCards.RemoveAt(randomIndex);
        }
        
        // 구독된 함수 실행
        OnSelectionOpened?.Invoke(selectCards);
    }

    public void Select(UpgradeCard card)
    {
        // 비정상적 접근 방지
        if (_pendingLevelUps <= 0)
        {
            return;
        }

        // 선택 카드 스탯 적용 
        _applier.Apply(card);
        // 대기 중인 레벨업 수 감소
        --_pendingLevelUps;

        // 대기 중인 레벨업이 남아있을 시, 추가 진행
        if (_pendingLevelUps > 0)
        {
            OpenSelection();
        // 전부 진행 완료 시, 일시정지 해제
        } else
        {
            Time.timeScale = 1f;
        }
    }

}
