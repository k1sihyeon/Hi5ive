using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialPlayerController : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 10f; // ���� ��
    private bool isGrounded = true; // �÷��̾ ���� �ִ��� ����
    public Camera playerCamera; // �÷��̾ ���� ī�޶�

    private Rigidbody rb; // Rigidbody ������Ʈ ������ ���� ����
    private DialogueSystem dialogueSystem; // ��ȭ �ý��� ����

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        dialogueSystem = FindObjectOfType<DialogueSystem>(); // ��ȭ �ý��� ������Ʈ ã��
        playerCamera.transform.SetParent(null); // ī�޶� �÷��̾��� �ڽĿ��� �и��մϴ�.
    }

    private void Update()
    {
        if (dialogueSystem != null && !dialogueSystem.dialogueCompleted)
        {
            return; // ��ȭ�� �Ϸ���� �ʾ����� �Է��� ���� �ʽ��ϴ�.
        }

        // �̵� �Է� ó��
        Vector3 moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        Vector3 moveVelocity = moveInput.normalized * moveSpeed;

        // ���� �Է� ó��
        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }

        // ī�޶� ��ġ�� ȸ���� �÷��̾ ����ϴ�.
        playerCamera.transform.position = transform.position;
        playerCamera.transform.rotation = transform.rotation;

        // Rigidbody�� ����Ͽ� �÷��̾� �̵� ����
        rb.MovePosition(rb.position + moveVelocity * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // ���� ��Ҵ��� Ȯ��
        if (collision.contacts[0].normal.y > 0.5f)
        {
            isGrounded = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // ���߿� ������
        isGrounded = false;
    }
}
