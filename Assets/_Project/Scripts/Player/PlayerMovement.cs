using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 1.0f;

    [SerializeField] private float _dashDistance = 5.0f;
    private Vector2 _movementInput; // 이동 입력 저장, Update에서 프레임별 입력 FixedUpdate에서 일정 주기 이동 적용
    private Rigidbody2D _rigidbody; // 장착된 rigidbody 참조하기


    [SerializeField] private LayerMask _wallLayer;
    [SerializeField] private float _wallOffset = 0.5f;
    [SerializeField] private float _dashCooldown = 1.0f;
    private float _dashCooldownRemaining = 0.0f;
    private bool _dashInput = false;
    
    // 읽기용 필드 (UI 접근용)
    public float MoveSpeed => _moveSpeed;

    // 초기화에 사용되는 메서드
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // 방향키 입력 하드코딩 > 나중에 리매핑하기 어려운 점 이해하기 (Input패키지가 최근방식임)
        _movementInput.x = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        _movementInput.y = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        // 대쉬 입력
        _dashInput = _dashInput || Input.GetKeyDown(KeyCode.Space);
   
        // 대각선 방향으로 이동 시에 빨라지는 부분 수정, 입력값을 아날로그로 받으면 정규화가 강제로 1로 키울 수 있음
        _movementInput.Normalize();
    }

    // 엔진이 프레임 간격을 재고, 0.02초마다 업데이트 하는 메서드
    void FixedUpdate()
    {
        // 이동 업데이트
        _rigidbody.linearVelocity = _movementInput * _moveSpeed;

        // 대쉬 쿨타임 적용
        if (_dashCooldownRemaining >= 0.0f)
        {
            _dashCooldownRemaining -= Time.fixedDeltaTime;
        }
        
        // 대쉬 사용
        if (_dashInput)
        {
            if (_dashCooldownRemaining <= 0.0f)
            {
                RaycastHit2D hit = Physics2D.Raycast(_rigidbody.position, _movementInput, _dashDistance, _wallLayer);
                Vector2 position = _rigidbody.position;
                if (hit.collider != null)
                {
                    position = hit.point + (hit.normal * _wallOffset);
                }
                else
                {
                    position = _rigidbody.position + (_movementInput * _dashDistance);
                }
                
                _rigidbody.position = position;
                _dashCooldownRemaining = _dashCooldown;
            }
            _dashInput = false;
        }
    }

    public void AddMoveSpeed(float amount)
    {
        if (amount <= 0f)
        {
            Debug.LogWarning("이동속도 증가량이 음수로 들어옴!!");
            return;
        }

        _moveSpeed += amount;
    }
}
