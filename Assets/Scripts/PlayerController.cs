using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public static PlayerController instance;

    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;
    public float jumpForce = 10f; // ���� ��
    private bool isGrounded = true; // �÷��̾ ���� �ִ��� ����
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

        if (IsLocalPlayer)
        {
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
            if (isGrounded && Input.GetKeyDown(KeyCode.Space))
            {
                Jump();

            }

            InputMagnitude();
        }
        else
        {
            //Destroy(GetComponentInChildren<Camera>().gameObject);
        }

        // ������ �̵� ���� ������
        if (IsServer)
        {            
            SendMovementDataServerRpc(moveInput);
        }

        // Ŭ���̾�Ʈ���� �̵� �� ȸ�� ����
        if (!IsServer)
        {
            Move(moveInput);
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
        }
    }
}