using System.Collections;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class SpeedUpHandler : NetworkBehaviour
{
    private PlayerController player;
    private PlayerCollision player_collision_info;
    private Volume volume;
    private ChromaticAberration chromaticAberration;
    private int randombox_result;

    public AudioSource PlusSound;//�����������
    public AudioClip PlusSoundSource;//��������

    private void Start()
    {
        player = GetComponent<PlayerController>();

        if (IsLocalPlayer) {
            PlusSound.clip = PlusSoundSource;
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
            if(player.moveSpeed<12)
            {
                player.moveSpeed = 10f;
                player.speed_up_count++;
                // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
                StartCoroutine(ResetSpeedAfterDelay(2f));
            }
            
        }
        if(other.CompareTag("Super_SpeedUp"))
        {
            Debug.Log("���ǵ������");
            //chromaticAberration.intensity.value = 0.7f;
            UpdateChromaticAberrationClientRpc(0.7f);
            // Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
            player.moveSpeed = 12f;
            player.speed_up_count++;
            // 3�� �Ŀ� �ٽ� �ӵ��� 5�� �����ϴ� �ڷ�ƾ ����
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
        if (other.gameObject.CompareTag("randombox"))
        {
            if(IsServer)
            {
                randombox_result = Random.Range(0, 2);
                if (randombox_result == 0)
                {
                    randombox_result = -4;
                }
                else if (randombox_result == 1)
                {
                    PlusSound.Play();
                    randombox_result = 4;
                }
                RandomEffect boxeffect = other.gameObject.GetComponent<RandomEffect>();
                boxeffect.effect(randombox_result);
                player.moveSpeed = randombox_result + 5;
                player.speed_up_count++;
                UpdatePlayerSpeedClientRpc(randombox_result + 5);
                StartCoroutine(ResetSpeedAfterDelayRpc(3f));
                NetworkDestroy_Trigger(other);
            }
            
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Obstacle"))
        {
            //player.CanMove = false;
            //StartCoroutine(ResetCanMove(1f));
        }
    }



    [ClientRpc]
    private void UpdatePlayerSpeedClientRpc(float speed)
    {
        player.moveSpeed = speed;
    }

    private void NetworkDestroy_Trigger(Collider collision)
    {
        ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        Destroy(collision.gameObject);
        CollisionClientRpc(enemyNetworkId);
    }

    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId)
    {

        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
        Destroy(enemyNetworkObject.gameObject);

    }

    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
        //chromaticAberration.intensity.value = 0.0f;
        player.speed_up_count--;
        if(player.speed_up_count==0)
        {
            UpdateChromaticAberrationClientRpc(0f);
            player.moveSpeed = 7f;
        }
        
    }

    private IEnumerator ResetSpeedAfterDelayRpc(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
        //chromaticAberration.intensity.value = 0.0f;
        player.speed_up_count--;
        if(player.speed_up_count==0)
        {
            UpdateChromaticAberrationClientRpc(0f);
            player.moveSpeed = 7f;
            UpdatePlayerSpeedClientRpc(player.moveSpeed);
        }
        
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
