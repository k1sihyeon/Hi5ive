using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class SpeedUpHandler : NetworkBehaviour
{
    private PlayerController player;
    private Volume volume;
    private ChromaticAberration chromaticAberration;

    private void Start()
    {
        player = GetComponent<PlayerController>();

        // �÷��̾� ��ü���� �ڽ� �߿� ī�޶� ã��
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            // ã�� ī�޶󿡼� Volume ������Ʈ ��������
            volume = playerCamera.GetComponent<Volume>();

            // Volume�� Chromatic Aberration ���� ��������
            if (volume != null)
            {
                volume.profile.TryGet(out chromaticAberration);
            }
            else
            {
                Debug.LogError("Volume ������Ʈ�� ã�� �� �����ϴ�.");
            }
        }
        else
        {
            Debug.LogError("�÷��̾� �ڽ� �߿� ī�޶� ã�� �� �����ϴ�.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("SpeedUp"))
        {
            Debug.Log("���ǹ���");
            chromaticAberration.intensity.value = 0.5f;
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            SetPlayerSpeedClientRpc(8.5f);

            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(2f));
        }
        if(other.CompareTag("Super_SpeedUp"))
        {
            Debug.Log("���ǵ������");
            chromaticAberration.intensity.value = 0.7f;
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            SetPlayerSpeedClientRpc(11f);

            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
        chromaticAberration.intensity.value = 0.0f;
        SetPlayerSpeedClientRpc(7f);
    }

    [ClientRpc]
    private void SetPlayerSpeedClientRpc(float speed)
    {
        // Ŭ���̾�Ʈ������ ȣ��Ǹ�, �÷��̾��� �ӵ��� ����
        player.moveSpeed = speed;
    }
}
