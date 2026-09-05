using UnityEngine;
using UnityEngine.UI;

public class DashCooldownBar : MonoBehaviour
{
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private Slider _slider;

    void Awake()
    {
        if (_movement == null)
        {
            Debug.LogError("PlayerMovement가 매핑되지 않았습니다.", this);
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
        _slider.maxValue = _movement.DashCooldown;
    }

    void Update()
    {
        _slider.value = _movement.DashCooldown - _movement.DashCooldownRemaining;
    }
}
