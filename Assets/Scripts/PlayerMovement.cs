using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;

public class PlayerMovement : NetworkBehaviour
{
    public NetworkVariable<FixedString32Bytes> nickName = new NetworkVariable<FixedString32Bytes>();
    private bool w;

    private bool a;

    private bool s;

    private bool d;

    private bool q;

    private bool e;

    private Camera player_camera;
    public float smoothSpeed = 30.0f;

    private float rotationSpeed = 70f;

    private float moveSpeed = 6f;

    public float jumpForce = 5f; // 점프 힘
    private bool isJumping = false;

    public ulong ownerClientId; // OwnerClientId 값을 저장할 변수
    private static PlayerMovement playerWithOwnerClientId; // OwnerClientId를 가지는 플레이어를 저장할 변수


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        SendNickNameServerRpc($"Player {OwnerClientId}");
        ownerClientId = OwnerClientId; // OwnerClientId를 설정
        playerWithOwnerClientId = this; // 해당 OwnerClientId를 가지는 플레이어를 변수에 저장

        player_camera = Camera.main;

    }

    public static PlayerMovement GetPlayerWithOwnerClientId(ulong targetOwnerId)
    {
        if (playerWithOwnerClientId != null && playerWithOwnerClientId.ownerClientId == targetOwnerId)
        {
            return playerWithOwnerClientId; // OwnerClientId를 가지는 플레이어를 반환합니다.
        }
        return null;
    }

    private void Update()
    {

        if (!IsOwner) { return; }

        w = Input.GetKey(KeyCode.W);
        s = Input.GetKey(KeyCode.S);
        a = Input.GetKey(KeyCode.A);
        d = Input.GetKey(KeyCode.D);
        q = Input.GetKey(KeyCode.Q);
        e = Input.GetKey(KeyCode.E);                
        if (Input.GetKey(KeyCode.LeftControl) && IsGrounded() && !isJumping)
        {
            // 클라이언트에서 서버로 점프 입력을 보냅니다.
            SendJumpInputServerRpc();
        }

        SendInputServerRpc(w, s, a, d, playerWithOwnerClientId.transform.rotation, q, e);
        
        Vector3 smoothPosition = Vector3.Lerp(transform.position, playerWithOwnerClientId.transform.position, smoothSpeed * Time.deltaTime);
        Quaternion smoothedRotation = Quaternion.Slerp(transform.rotation, playerWithOwnerClientId.transform.rotation, smoothSpeed * Time.deltaTime);
        //player_camera.transform.position = playerWithOwnerClientId.transform.position;
        player_camera.transform.position = smoothPosition;
        player_camera.transform.rotation = smoothedRotation;




    }


    [ServerRpc]
    public void SendInputServerRpc(bool w, bool s, bool a, bool d, Quaternion currentRotation, bool q, bool e)
    {

        Debug.Log(currentRotation);

        // 현재의 회전 각도를 오일러 각도로 변환합니다.
        Vector3 currentEulerAngles = currentRotation.eulerAngles;

        // q 키를 누를 때 왼쪽으로 방향을 돌립니다.
        if (q)
        {
            if (OwnerClientId != 0)
            {
                currentEulerAngles.y -= rotationSpeed * 40.0f * Time.deltaTime;
            }
            else
            {
                currentEulerAngles.y -= rotationSpeed * Time.deltaTime;
            }

        }

        // e 키를 누를 때 오른쪽으로 방향을 돌립니다.
        if (e)
        {
            if (OwnerClientId != 0)
            {
                currentEulerAngles.y += rotationSpeed * 40.0f * Time.deltaTime;
            }
            else
            {
                currentEulerAngles.y += rotationSpeed * Time.deltaTime;
            }
        }

        // 오일러 각도를 다시 쿼터니언으로 변환합니다.
        Quaternion newRotation = Quaternion.Euler(currentEulerAngles);



        // 플레이어의 회전을 업데이트합니다.
        transform.rotation = newRotation;
        Vector3 input = Vector3.zero;
        if (w)
        {
            input = newRotation * Vector3.forward; // 현재의 회전을 고려한 앞쪽 방향
        }
        if (s)
        {
            input = newRotation * Vector3.back; // 현재의 회전을 고려한 뒤쪽 방향
        }
        if (a)
        {
            input = newRotation * Vector3.left; // 현재의 회전을 고려한 왼쪽 방향
        }
        if (d)
        {
            input = newRotation * Vector3.right; // 현재의 회전을 고려한 오른쪽 방향
        }

        transform.Translate(input * moveSpeed * Time.deltaTime, Space.World);
    }

    [ServerRpc]
    public void SendNickNameServerRpc(string s)
    {
        nickName.Value = s;
        Debug.Log(nickName.Value);
        Debug.Log("실행됐어");
        
        Debug.Log("플레이어는 " + playerWithOwnerClientId + "입니다.");

    }




    [ServerRpc]
    public void SendJumpInputServerRpc()
    {
        if (IsGrounded() && !isJumping)
        {
            // 서버에서 모든 클라이언트에게 점프 입력을 전파합니다.
            JumpServerRpc();
        }
    }

    [ServerRpc]
    public void JumpServerRpc()
    {
        // 서버에서 모든 클라이언트에게 점프 입력을 전파합니다.
        JumpClientRpc();
    }

    [ClientRpc]
    public void JumpClientRpc()
    {
        // 클라이언트에서 플레이어를 점프시킵니다.
        Jump();
    }
    private void Jump()
    {
        isJumping = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        // 플레이어가 바닥에 닿아 있는지를 체크하는 코드
        // 여기에 해당 로직을 구현해야 합니다.
        // 프로젝트에 맞게 콜리전 레이어, 콜리더 사용 여부에 따라 로직을 구현하세요.

        // 아래는 간단한 예시입니다.
        Ray ray = new Ray(transform.position, Vector3.down);
        float rayDistance = 0.6f; // 레이 길이

        if (Physics.Raycast(ray, rayDistance))
        {
            isJumping = false; // 땅에 닿아 있으면 점프 상태를 해제
            return true;
        }

        return false; // 땅에 닿아 있지 않음
    }
}