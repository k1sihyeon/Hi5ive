using System.Collections;
using UnityEngine;
using Unity.Netcode;

public class SpeedUpHandler : NetworkBehaviour
{
    private PlayerController player;

    private void Start()
    {
        player = GetComponent<PlayerController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("SpeedUp"))
        {
            Debug.Log("발판밟음");
            // 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
            SetPlayerSpeedClientRpc(10f);

            // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3초 후에 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
        SetPlayerSpeedClientRpc(5f);
    }

    [ClientRpc]
    private void SetPlayerSpeedClientRpc(float speed)
    {
        // 클라이언트에서만 호출되며, 플레이어의 속도를 변경
        player.moveSpeed = speed;
    }
}
