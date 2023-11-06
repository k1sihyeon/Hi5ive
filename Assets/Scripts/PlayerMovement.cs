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

    public float jumpForce = 5f; // ���� ��
    private bool isJumping = false;

    public ulong ownerClientId; // OwnerClientId ���� ������ ����
    private static PlayerMovement playerWithOwnerClientId; // OwnerClientId�� ������ �÷��̾ ������ ����


    public override void OnNetworkSpawn()
    {
        if (!IsOwner) { return; }

        SendNickNameServerRpc($"Player {OwnerClientId}");
        ownerClientId = OwnerClientId; // OwnerClientId�� ����
        playerWithOwnerClientId = this; // �ش� OwnerClientId�� ������ �÷��̾ ������ ����

        player_camera = Camera.main;

    }

    public static PlayerMovement GetPlayerWithOwnerClientId(ulong targetOwnerId)
    {
        if (playerWithOwnerClientId != null && playerWithOwnerClientId.ownerClientId == targetOwnerId)
        {
            return playerWithOwnerClientId; // OwnerClientId�� ������ �÷��̾ ��ȯ�մϴ�.
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
            // Ŭ���̾�Ʈ���� ������ ���� �Է��� �����ϴ�.
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

        // ������ ȸ�� ������ ���Ϸ� ������ ��ȯ�մϴ�.
        Vector3 currentEulerAngles = currentRotation.eulerAngles;

        // q Ű�� ���� �� �������� ������ �����ϴ�.
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

        // e Ű�� ���� �� ���������� ������ �����ϴ�.
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

        // ���Ϸ� ������ �ٽ� ���ʹϾ����� ��ȯ�մϴ�.
        Quaternion newRotation = Quaternion.Euler(currentEulerAngles);



        // �÷��̾��� ȸ���� ������Ʈ�մϴ�.
        transform.rotation = newRotation;
        Vector3 input = Vector3.zero;
        if (w)
        {
            input = newRotation * Vector3.forward; // ������ ȸ���� ����� ���� ����
        }
        if (s)
        {
            input = newRotation * Vector3.back; // ������ ȸ���� ����� ���� ����
        }
        if (a)
        {
            input = newRotation * Vector3.left; // ������ ȸ���� ����� ���� ����
        }
        if (d)
        {
            input = newRotation * Vector3.right; // ������ ȸ���� ����� ������ ����
        }

        transform.Translate(input * moveSpeed * Time.deltaTime, Space.World);
    }

    [ServerRpc]
    public void SendNickNameServerRpc(string s)
    {
        nickName.Value = s;
        Debug.Log(nickName.Value);
        Debug.Log("����ƾ�");
        
        Debug.Log("�÷��̾�� " + playerWithOwnerClientId + "�Դϴ�.");

    }




    [ServerRpc]
    public void SendJumpInputServerRpc()
    {
        if (IsGrounded() && !isJumping)
        {
            // �������� ��� Ŭ���̾�Ʈ���� ���� �Է��� �����մϴ�.
            JumpServerRpc();
        }
    }

    [ServerRpc]
    public void JumpServerRpc()
    {
        // �������� ��� Ŭ���̾�Ʈ���� ���� �Է��� �����մϴ�.
        JumpClientRpc();
    }

    [ClientRpc]
    public void JumpClientRpc()
    {
        // Ŭ���̾�Ʈ���� �÷��̾ ������ŵ�ϴ�.
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
        // �÷��̾ �ٴڿ� ��� �ִ����� üũ�ϴ� �ڵ�
        // ���⿡ �ش� ������ �����ؾ� �մϴ�.
        // ������Ʈ�� �°� �ݸ��� ���̾�, �ݸ��� ��� ���ο� ���� ������ �����ϼ���.

        // �Ʒ��� ������ �����Դϴ�.
        Ray ray = new Ray(transform.position, Vector3.down);
        float rayDistance = 0.6f; // ���� ����

        if (Physics.Raycast(ray, rayDistance))
        {
            isJumping = false; // ���� ��� ������ ���� ���¸� ����
            return true;
        }

        return false; // ���� ��� ���� ����
    }
}