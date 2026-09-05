using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class HeartCollector : MonoBehaviour
{
    private PlayerExperience _experience;

    void Awake()
    {
        // 저장할 플레이어 경험치 객체와 매핑 
        _experience = GetComponentInParent<PlayerExperience>();
        if (_experience == null)
        {
            Debug.LogError("값을 저장할 PlayerExperience 가 매핑되지 않았습니다.", this);
            enabled = false;
        }

        // 디버깅용 
        _experience.OnLevelUp += () => Debug.Log("레벨업 성공!");
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Layer Coliision Matrix를 통해 Pickup만 판정함
        // Heart 소유 여부 확인 후 out 인자로 선언
        if (!other.TryGetComponent<Heart>(out Heart heart))
        {
            return;
        }
        // 경험치량 반환 + 자체 소멸
        int value = heart.Collect();
        // 경험치 적용
        _experience.Add(value);
    }
}
