using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Health))]
public class DamageFlash : MonoBehaviour
{
    // 피격 시 나타날 색 
    [SerializeField] private Color _flashColor = Color.red;
    [SerializeField] private float _duration = 0.1f;

    // 캐쉬용 프로퍼티들
    private SpriteRenderer _renderer;
    private Health _health;
    private Color _originalColor;
    private Coroutine _flashRoutine;
    private WaitForSeconds _wait;

    void Awake()
    {
        _wait = new WaitForSeconds(_duration);
        _renderer = GetComponent<SpriteRenderer>();
        _health = GetComponent<Health>();
        _originalColor = _renderer.color;
    }

    // 생성(재사용) 및 제거(대기) 일 때 피격효과 할당 및 제거
    void OnEnable()
    {
        _health.OnDamaged += Flash;
    }
    void OnDisable()
    {
        _health.OnDamaged -= Flash;
        // 사망 시 색 기존 색 보존 해주기
        _renderer.color = _originalColor;
    }

    void Flash()
    {
        if(_flashRoutine != null)
        {
            return;
        }   

        _flashRoutine = StartCoroutine(FlashRoutine());
    }

    IEnumerator FlashRoutine()
    {
        _renderer.color = _flashColor;
        // 일정시간 유지 
        yield return _wait;
        _renderer.color = _originalColor;

        _flashRoutine = null;
    }
}
