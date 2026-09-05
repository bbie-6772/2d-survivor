using System;
using UnityEngine;

public class PlayerUpgradeApplier : MonoBehaviour
{
    [SerializeField] private MeleeWeapon _weapon;
    [SerializeField] private PlayerMovement _movement;
    [SerializeField] private Health _health;
    [SerializeField] private UpgradeCard _testCard;

    void Awake()
    {
        if (_weapon == null)
        {
            Debug.LogError("강화에 사용될 Weapon이 지정되지 않았습니다!", this);
            enabled = false;
        }

        if (_movement == null)
        {
            Debug.LogError("강화에 사용될 Movement가 지정되지 않았습니다!", this);
            enabled = false;
        }

        if (_health == null)
        {
            Debug.LogError("강화에 사용될 health가 지정되지 않았습니다!", this);
            enabled = false;
        }
    }    

    public void Apply(UpgradeCard card)
    {
        // 카드 종류별 적용 방식 분리
        switch(card.Stat)
        {
            case StatType.AttackRange:
                _weapon.AddRange(card.Amount);
                break;
            case StatType.AttackSpeed:
                _weapon.AddAttackSpeed(card.Amount);
                break;
            case StatType.Damage:
                _weapon.AddDamage(card.Amount);
                break;
            case StatType.MaxHealth:
                _health.AddMaxHealth(card.Amount);
                break;
            case StatType.MoveSpeed:
                _movement.AddMoveSpeed(card.Amount);
                break;
            default:
                Debug.LogError("스탯 적용에 비상적인 입력 오류 발생!", this);
                break;
        }
    }
}
