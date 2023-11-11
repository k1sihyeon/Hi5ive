using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // 이 스크립트는 Rigidbody 컴포넌트가 필요함을 보장합니다.
public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -100f; // 땅 밑으로 떨어진 것으로 간주하는 높이
    private Vector3 respawnPoint; // 리스폰 지점
    private Rigidbody rb; // 플레이어의 Rigidbody 참조

    void Start()
    {
        // Rigidbody 컴포넌트를 가져옵니다.
        rb = GetComponent<Rigidbody>();

        // 땅의 Transform 정보를 기반으로 중앙 위치 계산
        // 땅의 위치는 땅의 가장자리를 기준으로 하므로, 스케일의 절반 값을 사용하여 중앙 위치를 찾습니다.
        float groundWidth = 20f; // 땅의 X축 스케일
        float groundDepth = 80f; // 땅의 Z축 스케일

        // 땅의 중앙 위치 계산
        Vector3 groundPosition = new Vector3(50.6987f, -53.7984f, -225.320f); // 땅의 현재 위치
        respawnPoint = new Vector3(groundPosition.x, groundPosition.y + 1f, groundPosition.z); // Y축은 땅 위로 약간 올립니다.
    }

    void Update()
    {
        // 플레이어의 현재 높이를 로그로 출력
        Debug.Log("Player Height: " + transform.position.y);

        // 플레이어가 일정 높이 이하로 떨어졌는지 확인
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Player fell below the threshold. Respawning...");
            RespawnPlayer();
        }
    }


    void RespawnPlayer()
    {
        void RespawnPlayer()
        {
            Debug.Log("Respawning to: " + respawnPoint);

            // 플레이어의 모든 물리 효과를 중단
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Rigidbody를 깨웁니다.
            rb.WakeUp();

            // 플레이어를 리스폰 지점으로 이동
            rb.MovePosition(respawnPoint);

            // 필요하다면, Rigidbody의 회전도 리셋할 수 있습니다.
            // rb.rotation = Quaternion.identity;

            // MovePosition이 작동하지 않는 경우를 위한 대체 방법
            // transform.position = respawnPoint;
        }

    }
}
