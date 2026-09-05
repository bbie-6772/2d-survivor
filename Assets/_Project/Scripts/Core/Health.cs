using System;
using UnityEngine;

public class Health : MonoBehaviour, IDamageable
{
    [SerializeField] private float _maxHealth = 100f;
    private float _currentHealth;
    // 죽을 때 동시에 여러번 발동되지 않도록 도와주는 플래그
    private bool _alreadyDead;

    // event 를 통해 외부에서 함수를 더하거나 빼기만 가능하도록 제한
    // Action = 인자, 반환값 없는 함수
    public event Action OnDied;
    // 피격판정 함수
    public event Action OnDamaged;
    // UI 업데이트용 함수 
    public event Action OnHealthChanged;

    // 읽기용 필드 (UI 접근용)
    public float MaxHealth => _maxHealth;
    public float CurrentHealth => _currentHealth;


    void Awake()
    {
        if (_maxHealth <= 0f)
        {
            Debug.LogError("잘못된 최대체력 값입니다", this);
            enabled = false;
            return;
        }

        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // 죽을 경우 계산 최소화
        if (_alreadyDead)
        {
            return;
        }

        // 이상한 값 (힐이되는 버그) 걸러내기
        if (amount < 0 )
        {
            Debug.LogWarning("데미지가 음수로 들어옴!!");
            return;
        }
        
        // 음수 방지
        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);
        OnDamaged?.Invoke();
        OnHealthChanged?.Invoke();

        if( _currentHealth <= 0f )
        {   
            // 입구 먼저막기 순서 조정
            _alreadyDead = true;
            // Null 오류 안나게 ?.Invoke() 사용 
            OnDied?.Invoke();
        }
    }

    public void AddMaxHealth(float amount)
    {
        if (amount <= 0f)
        {
            Debug.LogWarning("체력 증가량이 음수로 들어옴!!");
            return;
        }

        // 기존 최대체력 대비 현재체력 비율을 저장 
        float ratio = _currentHealth / _maxHealth;
        // 최대체력 증가
        _maxHealth += amount;
        // 증가 비율에 맞춰 현재체력 계산
        _currentHealth = _maxHealth * ratio;
        // UI 적용
        OnHealthChanged?.Invoke();
    }
}
