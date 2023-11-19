using UnityEngine;
using TMPro;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false;
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;
    public Transform cameraTransform; // 카메라 Transform

    private int currentLine = 0;

    void Start()
    {
        dialoguePanel.SetActive(true); // 시작 시 대화창 활성화
        dialogueText.text = dialogueLines[currentLine]; // 첫 대화 텍스트 설정
    }

    void Update()
    {
        if (dialogueCompleted) return;

        // 기본 대화 진행
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
            dialogueCompleted = true; // 대화 완료 플래그 설정
        }
    }
}
