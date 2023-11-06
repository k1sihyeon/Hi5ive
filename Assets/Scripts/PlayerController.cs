using UnityEngine;
using Unity.Netcode;

public class PlayerController : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 100f;

    private Vector3 moveInput;
    private float rotationInput;

    private Transform playerCameraTransform;
    private Camera playerCamera;

    private void Start()
    {
        playerCamera = Camera.main;
        playerCameraTransform = playerCamera.transform;
    }

    private void Update()
    {
        if (!IsClient)
            return;

        if (IsLocalPlayer)
        {
            // Input ó��
            moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
            rotationInput = Input.GetAxis("Mouse X") * rotationSpeed * Time.deltaTime;

            // ȸ�� �Է��� ����
            Rotate(rotationInput);

            // �̵� �Է��� ȸ���� �°� ����
            moveInput = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * moveInput;

            // Ŭ���̾�Ʈ �Ǵ� ȣ��Ʈ �÷��̾��� ��, ���� ī�޶� �÷��̾�� ����
            playerCameraTransform.position = transform.position;
            playerCameraTransform.rotation = transform.rotation;
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

    private void Move(Vector3 moveDirection)
    {
        transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
    }

    private void Rotate(float rotationAmount)
    {
        transform.Rotate(Vector3.up, rotationAmount);
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

    // Collision //
    private void OnCollisionEnter(Collision collision) {

        if(IsLocalPlayer) {

            if (collision.gameObject.CompareTag("Enemy")) {
                Debug.Log("Collison Detected");

                ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
                CollisionServerRpc(enemyNetworkId);
            }
        }

    }

    [ServerRpc]
    private void CollisionServerRpc(ulong enemyNetworkId) {
        Debug.Log("Collision Server �Լ�");

        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
        //GameObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId].gameObject;
        if(enemyNetworkObject != null) {
            enemyNetworkObject.Despawn();
        }
        
        CollisionClientRpc(enemyNetworkId);
    }

    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId) {
        Debug.Log("Collision Client �Լ�");

        if(!IsLocalPlayer) {
            NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
            //GameObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId].gameObject;
            if (enemyNetworkObject != null) {
                enemyNetworkObject.Despawn();
            }
        }
    }
}
