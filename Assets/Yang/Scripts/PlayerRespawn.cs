using UnityEngine;

[RequireComponent(typeof(Rigidbody))] // �� ��ũ��Ʈ�� Rigidbody ������Ʈ�� �ʿ����� �����մϴ�.
public class PlayerRespawn : MonoBehaviour
{
    public float fallThreshold = -100f; // �� ������ ������ ������ �����ϴ� ����
    private Vector3 respawnPoint; // ������ ����
    private Rigidbody rb; // �÷��̾��� Rigidbody ����

    void Start()
    {
        // Rigidbody ������Ʈ�� �����ɴϴ�.
        rb = GetComponent<Rigidbody>();

        // ���� Transform ������ ������� �߾� ��ġ ���
        // ���� ��ġ�� ���� �����ڸ��� �������� �ϹǷ�, �������� ���� ���� ����Ͽ� �߾� ��ġ�� ã���ϴ�.
        float groundWidth = 20f; // ���� X�� ������
        float groundDepth = 80f; // ���� Z�� ������

        // ���� �߾� ��ġ ���
        Vector3 groundPosition = new Vector3(50.6987f, -53.7984f, -225.320f); // ���� ���� ��ġ
        respawnPoint = new Vector3(groundPosition.x, groundPosition.y + 1f, groundPosition.z); // Y���� �� ���� �ణ �ø��ϴ�.
    }

    void Update()
    {
        // �÷��̾��� ���� ���̸� �α׷� ���
        Debug.Log("Player Height: " + transform.position.y);

        // �÷��̾ ���� ���� ���Ϸ� ���������� Ȯ��
        if (transform.position.y < fallThreshold)
        {
            Debug.Log("Player fell below the threshold. Respawning...");
            RespawnPlayer();
        }
    }


    void RespawnPlayer()
    {
        void RespawnPlayer()
        {
            Debug.Log("Respawning to: " + respawnPoint);

            // �÷��̾��� ��� ���� ȿ���� �ߴ�
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;

            // Rigidbody�� ����ϴ�.
            rb.WakeUp();

            // �÷��̾ ������ �������� �̵�
            rb.MovePosition(respawnPoint);

            // �ʿ��ϴٸ�, Rigidbody�� ȸ���� ������ �� �ֽ��ϴ�.
            // rb.rotation = Quaternion.identity;

            // MovePosition�� �۵����� �ʴ� ��츦 ���� ��ü ���
            // transform.position = respawnPoint;
        }

    }
}
