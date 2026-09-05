using System;
using UnityEngine;

public class Heart : MonoBehaviour
{
    private Action<GameObject> _release;
    // 경험치량 (컨셉 상은 강화 세포)
    [SerializeField] private int _value = 1;

    public void SetReleaseCallback(Action<GameObject> release)
    {
        _release = release;
    }

    public int Collect()
    {
        if (_release == null)
        {
            Debug.LogWarning("release가 비어있어 심장이 제거되지 못했습니다.", this);
            return 0;
        }
            
        _release(this.gameObject);

        return _value;
    }
}
