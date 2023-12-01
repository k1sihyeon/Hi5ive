using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoginPlayerController : MonoBehaviour {

    public Transform[] waypoints;  // 미리 정해진 원 형태의 경로의 위치들
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
        // 캐릭터를 원 형태의 경로로 이동
        Vector3 targetPosition = waypoints[currentWaypointIndex].position;
        transform.position = Vector3.Lerp(transform.position, targetPosition, moveSpeed * Time.deltaTime);

        // 만약 현재 위치와 목표 위치가 거의 같다면 다음 위치로 이동
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f) {
            probability = Random.value;
            if (probability < 0.2f)  // 홀수번째 위치일 때 점프
            {
                Jump();
            }

            currentWaypointIndex++;

            // 배열의 끝에 도달하면 처음 위치로 이동
            if (currentWaypointIndex >= waypoints.Length) {
                currentWaypointIndex = 0;
            }
        }

        // 애니메이션 업데이트
        UpdateAnimator();
    }

    void Jump() {
        if (!isJumping) {
            // 점프 로직
            GetComponent<Rigidbody>().AddForce(new Vector2(0f, jumpForce), ForceMode.Impulse);
            isJumping = true;
            animator.SetTrigger("jump");
        }
    }

    void OnCollisionEnter(Collision collision) {
        // 땅에 닿았을 때
        if (collision.gameObject.CompareTag("Ground")) {
            isJumping = false;
        }
    }

    void UpdateAnimator() {
        // 이동 애니메이션 업데이트
        float horizontalInput = (currentWaypointIndex % 2 == 0) ? 0f : 1f;  // 홀수 위치에서만 이동
        animator.SetFloat("Blend", horizontalInput, StartAnimTime, Time.deltaTime);
    }
}
