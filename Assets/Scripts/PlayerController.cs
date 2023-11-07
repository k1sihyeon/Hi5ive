using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
        playerCameraTransform = playerCamera.transform;
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (IsLocalPlayer)
        {
            // Input 처리
            moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            // 회전 입력을 적용
            Rotate(rotationInput);

            // 이동 입력을 회전에 맞게 수정
            moveInput = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput;

            // 클라이언트 또는 호스트 플레이어일 때, 메인 카메라를 플레이어에게 고정
            playerCameraTransform.position = transform.position;
            playerCameraTransform.rotation = transform.rotation;
        }

        // 서버로 이동 정보 보내기
        if (IsServer)
        {
            SendMovementDataServerRpc(moveInput);
        }

        // 클라이언트에서 이동 및 회전 적용
        if (!IsServer)
        {
            Move(moveInput);
        }
    }

    private void Move(Vector3 moveDirection)
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Rotate(float rotationAmount)
    {
        transform.Rotate(Vector3.up, rotationAmount);
    }

    [ServerRpc(RequireOwnership = false)]
    private void SendMovementDataServerRpc(Vector3 moveDirection)
    {
        Move(moveDirection);
        MovementDataClientRpc(moveDirection);
    }

    [ClientRpc]
    private void MovementDataClientRpc(Vector3 moveDirection)
    {
        if (!IsLocalPlayer)
        {
            Move(moveDirection);
        }
    }

}
