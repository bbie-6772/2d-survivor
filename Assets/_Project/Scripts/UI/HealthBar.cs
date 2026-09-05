using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Health _health;
    [SerializeField] private Slider _slider;

    void Awake()
    {
        if (_health == null)
        {
            Debug.LogError("Health가 매핑되지 않았습니다.", this);
            enabled = false;
        }

        if (_slider == null)
        {
            Debug.LogError("Slider가 매핑되지 않았습니다.", this);
            enabled = false;
        }
    }

    void Start()
    {
        // 매핑
        _health.OnHealthChanged += HandleHealthChanged;
        // 초기 1회 동기화 
        HandleHealthChanged();
    }

    void HandleHealthChanged()
    {
        float maxHealth = _health.MaxHealth;
        float currentHealth = _health.CurrentHealth;

        // 디버깅 용이 현재값, 최대값 직접 조정
        _slider.maxValue = maxHealth;
        _slider.value = currentHealth;
    }
}
