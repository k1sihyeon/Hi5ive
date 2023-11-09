using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 10f; // 점프 힘
    private bool isGrounded = true; // 플레이어가 땅에 있는지 여부

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;
    private Renderer playerRenderer; // 플레이어의 렌더러 컴포넌트

    private void Start()
    {
        playerCamera = Camera.main;
        playerCameraTransform = playerCamera.transform;

        // 플레이어의 렌더러 컴포넌트 찾기
        playerRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (IsLocalPlayer)
        {
            playerRenderer = GetComponent<Renderer>();
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

            // 점프 처리
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                playerRenderer.material.color = Color.red;
                Jump();

            }
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

    private void Jump()
    {

        // 점프 힘을 적용
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // 점프하면 플레이어가 땅에서 떨어집니다.

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

    // OnCollisionEnter를 사용하여 플레이어가 땅에 닿았을 때 isGrounded를 true로 설정하도록 구현해야 합니다.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // "Ground"는 땅 GameObject의 태그로 변경하세요.
        {
            isGrounded = true;
        }
    }
}