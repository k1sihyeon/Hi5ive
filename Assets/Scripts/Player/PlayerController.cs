using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController instance;

    public bool ignoringInputs = false;

    public float moveSpeed = 7f;
    public float rotationSpeed = 100f;
    public float jumpForce = 6f; // 점프 힘
    private bool isGrounded = true; // 플레이어가 땅에 있는지 여부
    private bool is_first_jump = true;
    public float playerHeightOffset = 1.15f;
    public float distanceAhead = 1.0f;

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;
    private Renderer playerRenderer; // 플레이어의 렌더러 컴포넌트

    public Animator anim;
    [Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0, 1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;
    public float InputX;
    public float InputZ;
    public float Speed;
    public float desiredRotationSpeed = 0.1f;
    public float allowPlayerRotation = 0.1f;

    [SerializeField] private Vector3 endPoint = new Vector3(15, 3, -280);


    private void Awake() {
        if(PlayerController.instance == null ) {
            PlayerController.instance = this;
        }
    }


    private void Start()
    {

        
        //playerCamera = Camera.main;
        //playerCameraTransform = playerCamera.transform;

        // 플레이어의 렌더러 컴포넌트 찾기
        playerRenderer = GetComponent<Renderer>();

        anim = this.GetComponent<Animator>();

        if (IsLocalPlayer)
        {
            
            Camera mainCamera = Camera.main;
            transform.position = new Vector3(185f, -1f, 295f);
            Quaternion newRotation = Quaternion.Euler(0f, -141f, 0f);
            transform.rotation = newRotation;
            if (mainCamera != null)
            {
                mainCamera.gameObject.SetActive(false); // 씬에 있는 일반 메인 카메라 비활성화
            }

            playerCamera = GetComponentInChildren<Camera>();
            playerCamera.tag = "MainCamera";
        }
        else
        {
            // 로컬 플레이어가 아닌 경우 플레이어에 있는 카메라 비활성화
            Camera playerCamera = GetComponentInChildren<Camera>();
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (!ignoringInputs) { //입력 무시 상태가 아니면

            if (IsLocalPlayer) {

                //디버깅용 골인지점 이동
                if(Input.GetKeyDown(KeyCode.BackQuote)) {
                    this.gameObject.transform.position = endPoint;
                    PlayerEndPoint.instance.SetPositionServerRpc(endPoint);
                }

                playerRenderer = GetComponent<Renderer>();
                // Input 처리
                moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

                // 회전 입력을 적용
                Rotate(rotationInput);

                // 이동 입력을 회전에 맞게 수정
                moveInput = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput;

                // 클라이언트 또는 호스트 플레이어일 때, 메인 카메라를 플레이어에게 고정
                /* Vector3 playerTopPosition = transform.position + Vector3.up * playerHeightOffset;
                 playerTopPosition = playerTopPosition + Vector3.forward * distanceAhead;
                 playerCameraTransform.position = playerTopPosition;
                 playerCameraTransform.rotation = transform.rotation;*/

                // 점프 처리
                if (isGrounded && Input.GetKeyDown(KeyCode.Space)) {
                    Jump();
                    anim.SetTrigger("jump");

                }
                else if (!isGrounded && is_first_jump && Input.GetKeyDown(KeyCode.Space)) {
                    DoubleJump();
                    anim.SetTrigger("double jump");
                }

                InputMagnitude();
            }
            else {
                //Destroy(GetComponentInChildren<Camera>().gameObject);
            }

            // 서버로 이동 정보 보내기
            if (IsServer) {
                SendMovementDataServerRpc(moveInput);
            }

            // 클라이언트에서 이동 및 회전 적용
            if (!IsServer) {
                Move(moveInput);
            }

        }
        
    }

    void InputMagnitude()
    {
        //Calculate Input Vectors
        InputX = Input.GetAxis("Horizontal");
        InputZ = Input.GetAxis("Vertical");

        //anim.SetFloat ("InputZ", InputZ, VerticalAnimTime, Time.deltaTime * 2f);
        //anim.SetFloat ("InputX", InputX, HorizontalAnimSmoothTime, Time.deltaTime * 2f);

        //Calculate the Input Magnitude
        Speed = new Vector2(InputX, InputZ).sqrMagnitude;

        //Physically move player

        if (Speed > allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
        }
        else if (Speed < allowPlayerRotation)
        {
            anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
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

        // 점프 힘을 적용
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // 점프하면 플레이어가 땅에서 떨어집니다.
        StartCoroutine(jumpcooldown(0.3f));


    }
    private void DoubleJump()
    {
        if (is_first_jump)
        {
            // 서버에게 이단 점프 이벤트를 전달
            DoubleJumpServerRpc();
            is_first_jump = false;
        }
    }

    private IEnumerator JumpCooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        is_first_jump = true;
    }

    [ServerRpc]
    private void DoubleJumpServerRpc()
    {
        // 서버에서 클라이언트로 이단 점프 이벤트를 전달
        DoubleJumpClientRpc();
    }

    [ClientRpc]
    private void DoubleJumpClientRpc()
    {
        // 클라이언트에서 이단 점프 구현
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private IEnumerator jumpcooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3초 후에 클라이언트에 속도 변경을 알리기 위해 ClientRpc 호출
        is_first_jump = true;
        
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

    // OnCollisionEnter를 사용하여 플레이어가 땅에 닿았을 때 isGrounded를 true로 설정하도록 구현해야 합니다.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // "Ground"는 땅 GameObject의 태그로 변경하세요.
        {
            isGrounded = true;
            is_first_jump = false;

        }
    }

}