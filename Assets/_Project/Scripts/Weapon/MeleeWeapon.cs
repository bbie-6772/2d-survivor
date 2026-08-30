using System.Collections;
using UnityEngine;

public class MeleeWeapon : MonoBehaviour
{   
    // 공격 범위 크기(radius)와 각도(angle)
    [SerializeField] private float _radius = 2f;
    [SerializeField] private float _angle = 90f;
    [SerializeField] private float _damage = 10f;
    // 자동 공격 주기 설정
    [SerializeField] private float _interval = 1f;
    // 공격 맞는 대상만을 지정하여 최적화
    [SerializeField] private LayerMask _targetLayer;
    // 추후 스탯변경에 의해 공격속도 증가 시, 캐싱 방식을 빼야할 수 있음
    private WaitForSeconds _wait;
    private float _threshold;
    // 플레이어 입력방향 캐싱
    private PlayerAim _playerAim;

    void Awake()
    {
        _wait = new WaitForSeconds(_interval);
        // 같은 오브젝트까지는 초기설정으로 취급하여 PlayerAim 캐싱진행
        _playerAim = GetComponent<PlayerAim>();  
        // 공격 최대 범위 계산(코사인 값)
        _threshold = Mathf.Cos(_angle / 2 * Mathf.Deg2Rad);
    }

    void Start()
    {
        
        StartCoroutine(AttackLoop());
    }

    void Update()
    {
        // 공격 범위 시각화
        Vector2 aim = _playerAim.AimDirection;
        Vector2 left  = Quaternion.Euler(0, 0,  _angle / 2) * aim;
        Vector2 right = Quaternion.Euler(0, 0, -_angle / 2) * aim;

        Debug.DrawRay(transform.position, aim   * _radius, Color.green);
        Debug.DrawRay(transform.position, left  * _radius, Color.yellow);
        Debug.DrawRay(transform.position, right * _radius, Color.yellow);
    }

    IEnumerator AttackLoop()
    {
        while(true)
        {
            // 범위 내(원형) 콜라이더 확인
            Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position,_radius, _targetLayer);

            foreach (Collider2D hit in hits)
            {
                // 타겟 방향 벡터 추출
                Vector2 toTarget = hit.transform.position - transform.position;
                toTarget.Normalize();

                // 공격 방향 벡터와 내적하여 코사인 값(방향) 추출
                float dot = Vector2.Dot(_playerAim.AimDirection, toTarget);
                
                // 공격 범위 판정 외의 경우 무시
                if ( dot < _threshold )
                {
                    continue;
                }
                // 공격 가능객체 확인 후 데미지 적용
                IDamageable enemy = hit.GetComponent<IDamageable>();
                if (enemy != null)
                {
                    enemy.TakeDamage(_damage);
                    continue;
                }

                Debug.LogWarning("닿은 대상이 공격가능한 상태가 아닙니다.", hit);
            }

            yield return _wait;
        }
    }




    
}
