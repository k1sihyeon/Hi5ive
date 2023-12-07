using UnityEngine;
using Unity.Netcode;
using System.Collections;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController instance;

    public bool ignoringInputs = false;

    public float moveSpeed = 7f;
    public float rotationSpeed = 100f;
    public float jumpForce = 6f; // ���� ��
    private bool isGrounded = true; // �÷��̾ ���� �ִ��� ����
    private bool is_first_jump = true;
    public float playerHeightOffset = 1.15f;
    public float distanceAhead = 1.0f;

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;
    private Renderer playerRenderer; // �÷��̾��� ������ ������Ʈ

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

        // �÷��̾��� ������ ������Ʈ ã��
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
                mainCamera.gameObject.SetActive(false); // ���� �ִ� �Ϲ� ���� ī�޶� ��Ȱ��ȭ
            }

            playerCamera = GetComponentInChildren<Camera>();
            playerCamera.tag = "MainCamera";
        }
        else
        {
            // ���� �÷��̾ �ƴ� ��� �÷��̾ �ִ� ī�޶� ��Ȱ��ȭ
            Camera playerCamera = GetComponentInChildren<Camera>();
            playerCamera.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (!ignoringInputs) { //�Է� ���� ���°� �ƴϸ�

            if (IsLocalPlayer) {

                //������ �������� �̵�
                if(Input.GetKeyDown(KeyCode.BackQuote)) {
                    this.gameObject.transform.position = endPoint;
                    PlayerEndPoint.instance.SetPositionServerRpc(endPoint);
                }

                playerRenderer = GetComponent<Renderer>();
                // Input ó��
                moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
                rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

                // ȸ�� �Է��� ����
                Rotate(rotationInput);

                // �̵� �Է��� ȸ���� �°� ����
                moveInput = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput;

                // Ŭ���̾�Ʈ �Ǵ� ȣ��Ʈ �÷��̾��� ��, ���� ī�޶� �÷��̾�� ����
                /* Vector3 playerTopPosition = transform.position + Vector3.up * playerHeightOffset;
                 playerTopPosition = playerTopPosition + Vector3.forward * distanceAhead;
                 playerCameraTransform.position = playerTopPosition;
                 playerCameraTransform.rotation = transform.rotation;*/

                // ���� ó��
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

            // ������ �̵� ���� ������
            if (IsServer) {
                SendMovementDataServerRpc(moveInput);
            }

            // Ŭ���̾�Ʈ���� �̵� �� ȸ�� ����
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

        // ���� ���� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false; // �����ϸ� �÷��̾ ������ �������ϴ�.
        StartCoroutine(jumpcooldown(0.3f));


    }
    private void DoubleJump()
    {
        if (is_first_jump)
        {
            // �������� �̴� ���� �̺�Ʈ�� ����
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
        // �������� Ŭ���̾�Ʈ�� �̴� ���� �̺�Ʈ�� ����
        DoubleJumpClientRpc();
    }

    [ClientRpc]
    private void DoubleJumpClientRpc()
    {
        // Ŭ���̾�Ʈ���� �̴� ���� ����
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        isGrounded = false;
    }

    private IEnumerator jumpcooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 3�� �Ŀ� Ŭ���̾�Ʈ�� �ӵ� ������ �˸��� ���� ClientRpc ȣ��
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

    // OnCollisionEnter�� ����Ͽ� �÷��̾ ���� ����� �� isGrounded�� true�� �����ϵ��� �����ؾ� �մϴ�.
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ground")) // "Ground"�� �� GameObject�� �±׷� �����ϼ���.
        {
            isGrounded = true;
            is_first_jump = false;

        }
    }

}