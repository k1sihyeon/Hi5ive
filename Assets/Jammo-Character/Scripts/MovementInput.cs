using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f; // ȸ�� �ӵ�
    public float jumpForce = 7f; // ���� ��
    public float doubleJumpForce = 5f; // ���� ���� ��
    public float desiredRotationSpeed = 0.1f; // ���ϴ� ȸ�� �ӵ�

    private bool isGrounded;
    private bool canDoubleJump = true;
    public Camera playerCamera; // �÷��̾��� ī�޶�
    private CharacterController controller;
    private Animator anim;

    private Vector3 playerVelocity;
    private float gravityValue = -9.81f; // �߷� ��
    private DialogueSystem dialogueSystem; // ��ȭ �ý��� ������Ʈ �߰�

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        // ī�޶� �÷��̾��� �ڽ����� �����մϴ�.
        playerCamera.transform.SetParent(transform);
        // ī�޶� �Ӹ� ��ġ�� �����մϴ�.
        playerCamera.transform.localPosition = new Vector3(0, 1.6f, 0);
        playerCamera.transform.localRotation = Quaternion.identity;
    }
    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>(); // ��ȭ �ý��� ������Ʈ�� ã���ϴ�.
    }
    private void Update()
    {
        // ��ȭ �ý����� �Ϸ�Ǿ����� Ȯ��
        if (dialogueSystem && !dialogueSystem.dialogueCompleted) return;

        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            playerVelocity.y = 0f;
            canDoubleJump = true;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime;
        }

        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        Vector3 forward = playerCamera.transform.forward;
        Vector3 right = playerCamera.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        controller.Move(moveDirection * Time.deltaTime * moveSpeed);

        // ���� ����Ʈ Ű�� ���� �����մϴ�.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("����!");
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !isGrounded && canDoubleJump)
        {
            playerVelocity.y = Mathf.Sqrt(doubleJumpForce * -3.0f * gravityValue);
            canDoubleJump = false;
        }

        playerVelocity.x = moveDirection.x * moveSpeed;
        playerVelocity.z = moveDirection.z * moveSpeed;
        controller.Move(playerVelocity * Time.deltaTime);

        RotateView();
    }



    private void RotateView()
    {
        // ���콺 �Է¿� ���� �÷��̾ �������� ȸ����ŵ�ϴ�.
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, horizontalRotation, 0);

        // ī�޶� ���� ȸ�� ������ �����մϴ�.
        float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        playerCamera.transform.Rotate(-verticalRotation, 0, 0);

        // ī�޶� ���� ȸ�� ������ �����ϰ�, 'z' ȸ��
    }
}