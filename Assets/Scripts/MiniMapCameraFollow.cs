using UnityEngine;

public class MiniMapCameraFollow : MonoBehaviour
{
    public Transform playerTransform; // �÷��̾��� Transform ����
    public float height = 10.0f; // ī�޶� �÷��̾� ���� ���� ����

    void LateUpdate()
    {
        // �÷��̾ null�� �ƴ� ���� �۵�
        if (playerTransform != null)
        {
            // ī�޶��� ��ġ�� �÷��̾��� ��ġ�� ���ߵ�, y���� ������ ���̷� ����
            transform.position = new Vector3(playerTransform.position.x, height, playerTransform.position.z);
        }
    }
}
