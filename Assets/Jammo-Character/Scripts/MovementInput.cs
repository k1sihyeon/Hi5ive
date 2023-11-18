
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {

    public float moveSpeed = 5f;
    public float jumpForce = 7f; // ���� ��
    public float doubleJumpForce = 5f; // ���� ���� ��
    private bool isGrounded;
    private bool canDoubleJump = true;
    public Camera playerCamera; // �÷��̾��� ī�޶�
    private CharacterController controller;
    private Animator anim;

    private Vector3 playerVelocity;
    private float gravityValue = -9.81f; // �߷� ��

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
            playerVelocity.y += gravityValue * Time.deltaTime; // �߷� ����
        }

        // �ٴڿ� ��Ҵ��� Ȯ���մϴ�.
        if (isGrounded && playerVelocity.y < 0)
        {
            playerVelocity.y = 0f;
        }

        // ī�޶��� ����� ������ ���͸� �������� ������ ������ ����մϴ�.
        Vector3 forward = cam.transform.forward;
        Vector3 right = cam.transform.right;
        forward.y = 0;
        right.y = 0;
        forward.Normalize();
        right.Normalize();

        Vector3 moveDirection = forward * Input.GetAxis("Vertical") + right * Input.GetAxis("Horizontal");
        controller.Move(moveDirection * Time.deltaTime * moveSpeed);

        // �÷��̾ �����̰� ������ �ش� ������ �ٶ󺸰� �մϴ�.
        if (moveDirection != Vector3.zero)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), desiredRotationSpeed);
        }

        // ���� ����
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            playerVelocity.y += Mathf.Sqrt(jumpForce * -3.0f * gravityValue);
        }
        else if (Input.GetButtonDown("Jump") && !isGrounded && canDoubleJump)
        {
            playerVelocity.y = Mathf.Sqrt(doubleJumpForce * -3.0f * gravityValue);
            canDoubleJump = false;
        }

        // �߷°� ������ �����մϴ�.
        playerVelocity.x = moveDirection.x * moveSpeed;
        playerVelocity.z = moveDirection.z * moveSpeed;
        controller.Move(playerVelocity * Time.deltaTime);

        // ���콺 �Է¿� ���� ī�޶� ȸ��
        RotateView();
    }

    private void RotateView()
    {
        // ���콺 �Է¿� ���� �÷��̾ ȸ����ŵ�ϴ�.
        float horizontalRotation = Input.GetAxis("Mouse X") * rotationSpeed;
        transform.Rotate(0, horizontalRotation, 0);

        // ī�޶� ���� ȸ�� ������ �����մϴ�.
        float verticalRotation = Input.GetAxis("Mouse Y") * rotationSpeed;
        playerCamera.transform.Rotate(-verticalRotation, 0, 0);
        var angles = playerCamera.transform.localEulerAngles;
        angles.z = 0;
        angles.x = (angles.x > 180) ? angles.x - 360 : angles.x;
        angles.x = Mathf.Clamp(angles.x, -30, 65);
        playerCamera.transform.localEulerAngles = angles;
    }


}
