using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f; // 회전 속도
    public float jumpForce = 7f; // 점프 힘
    public float doubleJumpForce = 5f; // 이중 점프 힘
    public float desiredRotationSpeed = 0.1f; // 원하는 회전 속도

    private bool isGrounded;
    private bool canDoubleJump = true;
    public Camera playerCamera; // 플레이어의 카메라
    private CharacterController controller;
    private Animator anim;

    private Vector3 playerVelocity;
    private float gravityValue = -9.81f; // 중력 값
    private DialogueSystem dialogueSystem; // 대화 시스템 컴포넌트 추가

    private void Start()
    {
        controller = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        // 카메라를 플레이어의 자식으로 설정합니다.
        playerCamera.transform.SetParent(transform);
        // 카메라를 머리 위치로 설정합니다.
        playerCamera.transform.localPosition = new Vector3(0, 1.6f, 0);
        playerCamera.transform.localRotation = Quaternion.identity;
    }
    private void Awake()
    {
        dialogueSystem = FindObjectOfType<DialogueSystem>(); // 대화 시스템 컴포넌트를 찾습니다.
    }
    private void Update()
    {
        // 대화 시스템이 완료되었는지 확인
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

        // 왼쪽 쉬프트 키를 눌러 점프합니다.
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            Debug.Log("점프!");
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
        // 마우스 입력에 따라 플레이어를 수평으로 회전시킵니다.
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;
        transform.Rotate(0, horizontalRotation, 0);

        // 카메라 상하 회전 범위를 제한합니다.
        float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed * Time.deltaTime;
        playerCamera.transform.Rotate(-verticalRotation, 0, 0);

        // 카메라 상하 회전 범위를 제한하고, 'z' 회전
    }
}