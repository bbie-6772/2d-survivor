using UnityEngine;

public class PlayerAim : MonoBehaviour
{
    // 초기값 하단 공격
    public Vector2 AimDirection { get; private set; } = new Vector2(0.0f, -1.0f);

    void Update()
    {
        Vector2 attackInput;
        attackInput.x = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) - (Input.GetKey(KeyCode.LeftArrow) ? 1 : 0);
        attackInput.y = (Input.GetKey(KeyCode.UpArrow) ? 1 : 0) - (Input.GetKey(KeyCode.DownArrow) ? 1 : 0);
        attackInput.Normalize();

        // 입력값이 존재할 때만 마지막 값 수정
        if (attackInput != Vector2.zero)
        {
            AimDirection = attackInput;
        }
    }

}
