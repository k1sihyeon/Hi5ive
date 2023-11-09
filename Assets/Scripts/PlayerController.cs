using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 10f; // ���� ��
    private bool isGrounded = true; // �÷��̾ ���� �ִ��� ����

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;
    private Renderer playerRenderer; // �÷��̾��� ������ ������Ʈ

    private void Start()
    {
        playerCamera = Camera.main;
        playerCameraTransform = playerCamera.transform;

        // �÷��̾��� ������ ������Ʈ ã��
        playerRenderer = GetComponent<Renderer>();
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (IsLocalPlayer)
        {
            playerRenderer = GetComponent<Renderer>();
            // Input ó��
            moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            // ȸ�� �Է��� ����
            Rotate(rotationInput);

            // �̵� �Է��� ȸ���� �°� ����
            moveInput = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput;

            // Ŭ���̾�Ʈ �Ǵ� ȣ��Ʈ �÷��̾��� ��, ���� ī�޶� �÷��̾�� ����
            playerCameraTransform.position = transform.position;
            playerCameraTransform.rotation = transform.rotation;

            // ���� ó��
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                playerRenderer.material.color = Color.red;
                Jump();

            }
        }

        // ������ �̵� ���� ������
        if (IsServer)
        {
            SendMovementDataServerRpc(moveInput);
        }

        // Ŭ���̾�Ʈ���� �̵� �� ȸ�� ����
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

        // ���� ���� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // �����ϸ� �÷��̾ ������ �������ϴ�.

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

    // OnCollisionEnter�� ����Ͽ� �÷��̾ ���� ����� �� isGrounded�� true�� �����ϵ��� �����ؾ� �մϴ�.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // "Ground"�� �� GameObject�� �±׷� �����ϼ���.
        {
            isGrounded = true;
        }
    }
}