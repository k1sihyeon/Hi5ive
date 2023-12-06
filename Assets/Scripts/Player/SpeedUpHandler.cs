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

        if (IsLocalPlayer) {
            // �÷��̾� ��ü���� �ڽ� �߿� ī�޶� ã��
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null) {
                // ã�� ī�޶󿡼� Volume ������Ʈ ��������
                volume = playerCamera.GetComponent<Volume>();

                // Volume�� Chromatic Aberration ���� ��������
                if (volume != null) {
                    volume.profile.TryGet(out chromaticAberration);
                }
                else {
                    Debug.LogError("Volume ������Ʈ�� ã�� �� �����ϴ�.");
                }
            }
            else {
                Debug.LogError("�÷��̾� �ڽ� �߿� ī�޶� ã�� �� �����ϴ�.");
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("SpeedUp"))
        {
            Debug.Log("���ǹ���");
            //chromaticAberration.intensity.value = 0.5f;
            UpdateChromaticAberrationClientRpc(0.5f);
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            player.moveSpeed = 10f;

            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(2f));
        }
        if(other.CompareTag("Super_SpeedUp"))
        {
            Debug.Log("���ǵ������");
            //chromaticAberration.intensity.value = 0.7f;
            UpdateChromaticAberrationClientRpc(0.7f);
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            player.moveSpeed = 15f;

            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
        //chromaticAberration.intensity.value = 0.0f;
        UpdateChromaticAberrationClientRpc(0f);
        player.moveSpeed = 7f;
    }

    [ClientRpc]
    private void UpdateChromaticAberrationClientRpc(float value) {

        if (IsLocalPlayer) {
            chromaticAberration.intensity.value = value;
        }

    }



    /*[ClientRpc]
    private void SetPlayerSpeedClientRpc(float speed)
    {
        // Ŭ���̾�Ʈ������ ȣ��Ǹ�, �÷��̾��� �ӵ��� ����
        player.moveSpeed = speed;
    }*/
}