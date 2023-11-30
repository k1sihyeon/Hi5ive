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

        // 플레이어 객체에서 자식 중에 카메라 찾기
        Camera playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera != null)
        {
            // 찾은 카메라에서 Volume 컴포넌트 가져오기
            volume = playerCamera.GetComponent<Volume>();

            // Volume의 Chromatic Aberration 설정 가져오기
            if (volume != null)
            {
                volume.profile.TryGet(out chromaticAberration);
            }
            else
            {
                Debug.LogError("Volume 컴포넌트를 찾을 수 없습니다.");
            }
        }
        else
        {
            Debug.LogError("플레이어 자식 중에 카메라를 찾을 수 없습니다.");
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return;

        if (other.CompareTag("SpeedUp"))
        {
            Debug.Log("발판밟음");
            chromaticAberration.intensity.value = 0.5f;
            // 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
            SetPlayerSpeedClientRpc(8.5f);

            // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
            StartCoroutine(ResetSpeedAfterDelay(2f));
        }
        if(other.CompareTag("Super_SpeedUp"))
        {
            Debug.Log("스피드업밟음");
            chromaticAberration.intensity.value = 0.7f;
            // 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
            SetPlayerSpeedClientRpc(11f);

            // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
            StartCoroutine(ResetSpeedAfterDelay(3f));
        }
    }
    
    private IEnumerator ResetSpeedAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3초 후에 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
        chromaticAberration.intensity.value = 0.0f;
        SetPlayerSpeedClientRpc(7f);
    }

    [ClientRpc]
    private void SetPlayerSpeedClientRpc(float speed)
    {
        // 클라이언트에서만 호출되며, 플레이어의 속도를 변경
        player.moveSpeed = speed;
    }
}
