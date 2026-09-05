using System;
using UnityEngine;

public class PlayerExperience : MonoBehaviour
{
    // 기본 레벨업 요구치
    [SerializeField] private int _requiredExp = 5;
    // 경험치 요구량 증가 배율
    [SerializeField] private int _growthRate = 2;
    // 외부 읽기용 필드
    public int Current { get; private set;}
    public event Action OnLevelUp;

    public void Add(int amount)
    {
        Current += amount;

        // 중복 레벨업 계산
        while(Current >= _requiredExp)
        {
            // 레벨요구치 감소 적용
            Current -= _requiredExp;
            // 매핑된 함수 실행
            OnLevelUp?.Invoke();
            Debug.Log($"레벨업 완료! {Current}");
            // 다음 경험치 요구량 설정
            _requiredExp *= _growthRate;
        }
    }
}
