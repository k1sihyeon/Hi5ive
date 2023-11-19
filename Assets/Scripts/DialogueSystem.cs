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
}
