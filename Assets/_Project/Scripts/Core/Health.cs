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

    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float amount)
    {
        // 이상한 값 (힐이되는 버그) 걸러내기
        if (amount < 0 )
        {
            Debug.LogWarning("데미지가 음수로 들어옴!!");
            return;
        }
        // 음수 방지
        _currentHealth = Mathf.Max(_currentHealth - amount, 0f);

        if( _currentHealth <= 0f && !_alreadyDead )
        {
            // Null 오류 안나게 ?.Invoke() 사용 
            OnDied?.Invoke();
            _alreadyDead = true;
        }
    }
}
