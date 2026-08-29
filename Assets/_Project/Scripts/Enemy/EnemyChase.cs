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

    void Start()
    {
        // 타입 종속성이 생기기에 확장성은 아쉬우나, 프로젝트 규모상 OK 판단
        _target = Object.FindAnyObjectByType<PlayerMovement>().transform;
    }

    void FixedUpdate()
    {
        Vector2 targetDirection = (Vector2)_target.position - _rigidbody.position;
        targetDirection.Normalize();

        _rigidbody.linearVelocity = targetDirection * _moveSpeed;
    }
}
