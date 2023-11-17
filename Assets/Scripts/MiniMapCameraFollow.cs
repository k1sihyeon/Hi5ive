using UnityEngine;

public class MiniMapCameraFollow : MonoBehaviour
{
    public Transform playerTransform; // 플레이어의 Transform 참조
    public float height = 10.0f; // 카메라가 플레이어 위에 있을 높이

    void LateUpdate()
    {
        // 플레이어가 null이 아닐 때만 작동
        if (playerTransform != null)
        {
            // 카메라의 위치를 플레이어의 위치에 맞추되, y값은 지정된 높이로 설정
            transform.position = new Vector3(playerTransform.position.x, height, playerTransform.position.z);
        }
    }
}
