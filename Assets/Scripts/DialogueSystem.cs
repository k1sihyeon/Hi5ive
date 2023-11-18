using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;

    private int currentLine = 0;
    private const int startLineAtFinal = 4; // "Final" 태그를 가진 땅에 도착했을 때 시작할 대화 줄

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
            dialogueCompleted = true; // 대화가 완료되었다고 표시
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Final"))
        {
            currentLine = startLineAtFinal; // 대화를 4번 요소부터 시작
            dialogueCompleted = false; // 대화를 다시 활성화
            StartDialogue(); // 대화 시작
        }
    }
}
