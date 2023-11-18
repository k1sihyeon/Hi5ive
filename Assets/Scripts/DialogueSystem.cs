using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;

    private int currentLine = 0;
    private const int startLineAtFinal = 4; // "Final" �±׸� ���� ���� �������� �� ������ ��ȭ ��

    void Update()
    {
        if (dialogueCompleted) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialoguePanel.activeInHierarchy && currentLine < startLineAtFinal)
            {
                StartDialogue();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void Start()
    {
        dialoguePanel.SetActive(false);
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLines[currentLine];
    }

    public void ShowNextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length && (currentLine < startLineAtFinal || dialogueCompleted))
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            dialoguePanel.SetActive(false);
            dialogueCompleted = true; // ��ȭ�� �Ϸ�Ǿ��ٰ� ǥ��
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Final"))
        {
            currentLine = startLineAtFinal; // ��ȭ�� 4�� ��Һ��� ����
            dialogueCompleted = false; // ��ȭ�� �ٽ� Ȱ��ȭ
            StartDialogue(); // ��ȭ ����
        }
    }
}
