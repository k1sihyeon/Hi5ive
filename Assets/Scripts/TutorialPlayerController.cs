using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f; // 점프 힘
    private bool isGrounded = true; // 플레이어가 땅에 있는지 여부
    public Camera playerCamera; // 플레이어가 가진 카메라

    private Rigidbody rb; // Rigidbody 컴포넌트 참조를 위한 변수
    private DialogueSystem dialogueSystem; // 대화 시스템 참조

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        dialogueSystem = FindObjectOfType<DialogueSystem>(); // 대화 시스템 컴포넌트 찾기
        playerCamera.transform.SetParent(null); // 카메라를 플레이어의 자식에서 분리합니다.
    }

    private void Update()
    {
        if (dialogueSystem != null && !dialogueSystem.dialogueCompleted)
        {
            return; // 대화가 완료되지 않았으면 입력을 받지 않습니다.
        }

        // 이동 입력 처리
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        // 점프 입력 처리
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // 카메라 위치와 회전을 플레이어에 맞춥니다.
        playerCamera.transform.position = transform.position;
        playerCamera.transform.rotation = transform.rotation;

        // Rigidbody를 사용하여 플레이어 이동 적용
        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 땅에 닿았는지 확인
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 공중에 떠있음
        isGrounded = false;
    }
}
