using UnityEngine;

public class ContactDamage : MonoBehaviour
{
    [SerializeField] private float _damage = 10f;
    // 외부 참조 읽기전용 , 내부에서도 값을 변경할 수 없음
    public float Damage => _damage;
}
