using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private float _smoothTime = 0.15f;
    
    private float _z;
    private Vector2 _velocity;

    void Awake()
    {
        if (!_target)
        {
            Debug.LogError("카메라에 추적대상이 없습니다", this);
            enabled = false;
            return;
        }
        // 2D 특성상 z(화면과의 거리)값은 고정이므로 미리 캐싱
        _z = transform.position.z;
    }

    // FixedUpdate(물리주기) > Update(프레임) > 코루틴 > LateUpdate -> 렌더링 순서
    // 렌더링 직전에 카메라를 움직여 자연스럽게 설계
    void LateUpdate()
    {
        Vector2 currentPosition = Vector2.SmoothDamp((Vector2)transform.position, (Vector2)_target.position, ref _velocity, _smoothTime);
        
        transform.position= new Vector3(currentPosition.x, currentPosition.y, _z);
    }

}
