using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;
    public Transform cameraTransform; // ī�޶� Transform

    private int currentLine = 0;
    private bool isNearNPC = false; // NPC ��ó�� �ִ��� ����

    void Start()
    {
        dialoguePanel.SetActive(true); // ���� �� ��ȭâ Ȱ��ȭ
        dialogueText.text = dialogueLines[currentLine]; // ù ��ȭ �ؽ�Ʈ ����
    }

    void Update()
    {
        if (dialogueCompleted) return;

        // �⺻ ��ȭ ����
        if (Input.GetKeyDown(KeyCode.Space) && currentLine < 4)
        {
            ShowNextLine();
            if (currentLine == 4)
            {
                dialoguePanel.SetActive(false);
                dialogueCompleted = true;
            }
        }

        // NPC�� ��ȣ�ۿ� �� ��ȭ �簳
        if (isNearNPC && Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                StartDialogueFromFinal();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void StartDialogueFromFinal()
    {
        dialogueCompleted = false;
        currentLine = 4;
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLines[currentLine];
        AdjustCameraAngle();
    }

    void AdjustCameraAngle()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles += new Vector3(-10f, 0f, 0f);
        }
    }

    public void ShowNextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogueCompleted = true; // ��ȭ �Ϸ� �÷��� ����
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = true; // NPC ��ó�� �ִٰ� ǥ��
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = false; // NPC ��ó���� ����ٰ� ǥ��
        }
    }
}
