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

    public AudioSource PlusSound;//사운드저장공간
    public AudioClip PlusSoundSource;//실제사운드

    private void Start()
    {
        player = GetComponent<PlayerController>();

        if (IsLocalPlayer) {
            PlusSound.clip = PlusSoundSource;
            // 플레이어 객체에서 자식 중에 카메라 찾기
            Camera playerCamera = GetComponentInChildren<Camera>();
            if (playerCamera != null) {
                // 찾은 카메라에서 Volume 컴포넌트 가져오기
                volume = playerCamera.GetComponent<Volume>();

                // Volume의 Chromatic Aberration 설정 가져오기
                if (volume != null) {
                    volume.profile.TryGet(out chromaticAberration);
                }
                else {
                    Debug.LogError("Volume 컴포넌트를 찾을 수 없습니다.");
                }
            }
            else {
                Debug.LogError("플레이어 자식 중에 카메라를 찾을 수 없습니다.");
            }
        }

    }


    private void OnTriggerEnter(Collider other)
    { 
        if (other.CompareTag("SpeedUp"))
        {
            Debug.Log("발판밟음");
            //chromaticAberration.intensity.value = 0.5f;
            UpdateChromaticAberrationClientRpc(0.5f);
            // 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
            if(player.moveSpeed<12)
            {
                player.moveSpeed = 10f;
                player.speed_up_count++;
                // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
                StartCoroutine(ResetSpeedAfterDelay(2f));
            }
            
        }
        if(other.CompareTag("Super_SpeedUp"))
        {
            Debug.Log("스피드업밟음");
            //chromaticAberration.intensity.value = 0.7f;
            UpdateChromaticAberrationClientRpc(0.7f);
            // 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
            player.moveSpeed = 12f;
            player.speed_up_count++;
            // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
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
        // 3초 후에 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
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
        // 3초 후에 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
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
        // 클라이언트에서만 호출되며, 플레이어의 속도를 변경
        player.moveSpeed = speed;
    }*/
}
