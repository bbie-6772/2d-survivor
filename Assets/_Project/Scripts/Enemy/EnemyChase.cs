using UnityEngine;

public class EnemyChase : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 2.0f;

    private Rigidbody2D _rigidbody;
    private Transform _target;

    void Awake()
    {
        // 적 내부 RigidBody2D 캐싱
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 targetDirection = (Vector2)_target.position - _rigidbody.position;
        targetDirection.Normalize();

        _rigidbody.linearVelocity = targetDirection * _moveSpeed;
    }

    // 스포너가 생기고 초기화해주던 Start 대신 Init 메서드 사용
    public void Init(Transform target)
    {
        _target = target;
    }
}
