using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPlayerController : MonoBehaviour {

    public Transform[] waypoints;  
    public float moveSpeed = 4f;
    public float jumpForce = 6f;

    private int currentWaypointIndex = 0;
    private bool isJumping = false;

    private float probability;

    private Animator animator;

    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

    void Start() {
        animator = GetComponent<Animator>();
        currentWaypointIndex = Random.Range(0, waypoints.Length);
    }

    void Update() {
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        probability = Random.value;
        if (probability < 0.001f) {
            Jump();
        }

        // ���� ���� ��ġ�� ��ǥ ��ġ�� ���� ���ٸ� ���� ��ġ�� �̵�
        if (Vector3.Distance(transform.position, targetPosition) < 0.2f) {
            currentWaypointIndex = Random.Range(0, waypoints.Length);
        }

        // �ִϸ��̼� ������Ʈ
        UpdateAnimator();
    }

    void Jump() {
        if (!isJumping) {
            GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
            animator.SetTrigger("jump");
        }
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Ground")) {
            isJumping = false;
        }
    }

    void UpdateAnimator() {
        // �̵� �ִϸ��̼� ������Ʈ
        animator.SetFloat("Blend", 1f, StartAnimTime, Time.deltaTime);

        Vector3 targetDirection = (waypoints[currentWaypointIndex].position - transform.position).normalized;
        transform.LookAt(transform.position + new Vector3(targetDirection.x, 0f, targetDirection.z), Vector3.up);
    }
}
