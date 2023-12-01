using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPlayerController : MonoBehaviour {

    public Transform[] waypoints;  // �̸� ������ �� ������ ����� ��ġ��
    public float moveSpeed = 8f;
    public float jumpForce = 6f;

    private int currentWaypointIndex = 0;
    private bool isJumping = false;

    private float probability;

    private Animator animator;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    void Start() {
        animator = GetComponent<Animator>();
    }

    void Update() {
        // ĳ���͸� �� ������ ��η� �̵�
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // ���� ���� ��ġ�� ��ǥ ��ġ�� ���� ���ٸ� ���� ��ġ�� �̵�
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
            probability = Random.value;
            if (probability < 0.2f)  // Ȧ����° ��ġ�� �� ����
            {
                Jump();
            }

            currentWaypointIndex++;

            // �迭�� ���� �����ϸ� ó�� ��ġ�� �̵�
            if (currentWaypointIndex >= waypoints.Length) {
                currentWaypointIndex = 0;
            }
        }

        // �ִϸ��̼� ������Ʈ
        UpdateAnimator();
    }

    void Jump() {
        if (!isJumping) {
            // ���� ����
            GetComponent<Rigidbody>().AddForce(new Vector2(0f, jumpForce), ForceMode.Impulse);
            isJumping = true;
            animator.SetTrigger("jump");
        }
    }

    void OnCollisionEnter(Collision collision) {
        // ���� ����� ��
        if (collision.gameObject.CompareTag("Ground")) {
            isJumping = false;
        }
    }

    void UpdateAnimator() {
        // �̵� �ִϸ��̼� ������Ʈ
        float horizontalInput = (currentWaypointIndex % 2 == 0) ? 0f : 1f;  // Ȧ�� ��ġ������ �̵�
        animator.SetFloat("Blend", horizontalInput, StartAnimTime, Time.deltaTime);
    }
}
