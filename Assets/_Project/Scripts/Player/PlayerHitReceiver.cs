using UnityEngine;

//중복되는 속성이 존재하지 않도록 강제하기(2개 이상 들어가면 데미지가 한번에 배수로 들어옴)
[DisallowMultipleComponent]
// 에디터에서 Health가 무조건 존재하도록 강제하는 방식
[RequireComponent(typeof(Health))]
public class PlayerHitReceiver : MonoBehaviour
{
    // 피격에 관한 무적시간용 필드 단위는 Second
    [SerializeField] private float _invincibleDuration = 0.5f;
    private float _lastHitTime;
    
    private Health _health;

    void Awake()
    {
        // Player 에 포함된 health 캐싱
        _health = GetComponent<Health>();
    }

    // 접촉만이 아닌, 지속적으로 닿고 있는 경우에도 받도록 Enter 대신 Stay 사용 
    void OnCollisionStay2D(Collision2D collision)
    {
        // 겹친 오브젝트에 데미지를 주는 컴포넌트 존재여부 확인 + 무적 시간 판정
        ContactDamage damageObject = collision.gameObject.GetComponent<ContactDamage>();
        if (damageObject == null || _lastHitTime + _invincibleDuration > Time.time)
        {
            return;
        }

        _health.TakeDamage(damageObject.Damage);
        _lastHitTime = Time.time;
    }
}
