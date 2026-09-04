using System;
using UnityEngine;

[RequireComponent(typeof(Health))]
public class EnemyDeath : MonoBehaviour
{
    private Health _health;
    // 콜백 함수를 받아 Despawn 하기 위함 ( 의존성 0 )
    private Action<GameObject> _release;
    private Action<Vector2> _dropHeart;

    void Awake()
    {
        _health = GetComponent<Health>();
    }

    void OnEnable()
    {
        _health.OnDied += HandleDeath;
    }

    void OnDisable()
    {
        _health.OnDied -= HandleDeath;
    }

    public void SetReleaseCallback(Action<GameObject> release)
    {
        _release = release;
    }

    public void SetDropHeartCallback(Action<Vector2> dropHeart)
    {
        _dropHeart = dropHeart;
    }

    void HandleDeath()
    {
        if (_dropHeart == null)
        {
            Debug.LogWarning("dropHeart가 비어있어 심장이 드랍되지 못했습니다.", this);
        }
        else
        {
            _dropHeart(transform.position);
        }

        if (_release == null)
        {
            Debug.LogWarning("release가 비어있어 좀비가 제거되지 못했습니다.", this);
            return;
        }
            
        _release(this.gameObject);
    }
}
