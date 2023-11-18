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
            Debug.Log("���ǹ���");
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            SetPlayerSpeedClientRpc(10f);

            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
        SetPlayerSpeedClientRpc(5f);
    }

    [ClientRpc]
    private void SetPlayerSpeedClientRpc(float speed)
    {
        // Ŭ���̾�Ʈ������ ȣ��Ǹ�, �÷��̾��� �ӵ��� ����
        player.moveSpeed = speed;
    }
}
