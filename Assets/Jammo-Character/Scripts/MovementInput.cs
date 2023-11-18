
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {

    public float moveSpeed = 5f;
    public float jumpForce = 7f; // 점프 힘
    public float doubleJumpForce = 5f; // 이중 점프 힘
    private bool isGrounded;
    private bool canDoubleJump = true;
    public Camera playerCamera; // 플레이어의 카메라
    private CharacterController controller;
    private Animator anim;

    private Vector3 playerVelocity;
    private float gravityValue = -9.81f; // 중력 값

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

    private void Update()
    {
        isGrounded = controller.isGrounded;
        if (isGrounded)
        {
            playerVelocity.y = 0f;
            canDoubleJump = true;
        }
        else
        {
            playerVelocity.y += gravityValue * Time.deltaTime; // 중력 적용
        }

        // 바닥에 닿았는지 확인합니다.
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // 카메라의 전방과 오른쪽 벡터를 기준으로 움직임 방향을 계산합니다.
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        controller.Move(moveDirection * Time.deltaTime * moveSpeed);

        // 플레이어가 움직이고 있으면 해당 방향을 바라보게 합니다.
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), desiredRotationSpeed);
        }

        // 점프 로직
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }
        else if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump)
        {
            playerVelocity.y = Mathf.Sqrt(doubleJumpForce * -3.0f * gravityValue);
            canDoubleJump = false;
        }

        // 중력과 점프를 적용합니다.
        playerVelocity.x = moveDirection.x * moveSpeed;
        playerVelocity.z = moveDirection.z * moveSpeed;
        controller.Move(playerVelocity * Time.deltaTime);

        // 마우스 입력에 따라 카메라 회전
        RotateView();
    }

    private void RotateView()
    {
        // 마우스 입력에 따라 플레이어를 회전시킵니다.
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0, horizontalRotation, 0);

        // 카메라 상하 회전 범위를 제한합니다.
        float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
        playerCamera.transform.Rotate(-verticalRotation, 0, 0);
        var angles = playerCamera.transform.localEulerAngles;
        angles.z = 0;
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;
        angles.x = Mathf.Clamp(angles.x, -30, 65);
        playerCamera.transform.localEulerAngles = angles;
    }


}
