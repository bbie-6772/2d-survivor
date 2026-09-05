using System.Collections;
using UnityEngine;

public class AttackFlash : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _flashSprite;
    [SerializeField] private float _duration = 0.1f;
    [SerializeField] private Transform _pivot;
    private MeleeWeapon _weapon;

    private PlayerAim _playerAim;
    private Coroutine _flashRoutine;
    private WaitForSeconds _wait;

    void Awake()
    {
        if(_flashSprite == null)
        {
            Debug.LogError("스프라이트가 연결되지 않았습니다", this);
            enabled = false;
            return;
        }

        if(_pivot == null)
        {
            Debug.LogError("pivot(중심점)이 연결되지 않았습니다", this);
            enabled = false;
            return;
        }

        // 같은 오브젝트 초기설정으로 취급하여 진행
        _weapon = GetComponent<MeleeWeapon>();
        _playerAim = GetComponent<PlayerAim>();  
        _wait = new WaitForSeconds(_duration);
    }

    void Start()
    {
        _weapon.OnAttacked += Flash;
    }

    void RotateSprite()
    {
        // 바꿀 각도 계산 아크탄젠트(라디안 값) 에서 Rad To Deg 를 곱하여 각도로 변환
        float angle = Mathf.Atan2(_playerAim.AimDirection.y, _playerAim.AimDirection.x) * Mathf.Rad2Deg;
        // 오일러 각도 변환 후 대입
        _pivot.transform.rotation = Quaternion.Euler(0f, 0f, angle);
    }

    void Flash()
    {
        // 공격속도가 높아져서 진행중이라면 멈추고 새롭게 재시작
        if(_flashRoutine != null)
        {
            StopCoroutine(_flashRoutine);
        }

        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        // 크기 및 범위 적용(사각형 기준)
        float range = _weapon.Range;
        _flashSprite.transform.localScale = new Vector3(range, range * 1.2f, 1f);
        _flashSprite.transform.localPosition = new Vector3(range/2, 0f);
        // 방향 회전
        RotateSprite();
        // 활성화
        _flashSprite.enabled = true;
        // 일정시간 유지 
        yield return _wait;
        // 비활성화
        _flashSprite.enabled = false;

        _flashRoutine = null;
    }
}
