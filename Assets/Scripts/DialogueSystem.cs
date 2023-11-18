using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // 대화창에 텍스트를 표시할 TextMeshPro UI 요소
    public string[] dialogueLines; // 대화 내용을 저장할 배열
    public GameObject dialoguePanel; // 대화창 패널

    private int currentLine = 0; // 현재 표시하고 있는 대사의 인덱스

    void Start()
    {
        dialoguePanel.SetActive(false); // 게임 시작 시 대화창을 숨깁니다.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바가 눌리면
        {
            if (!dialoguePanel.activeInHierarchy)
            {
                StartDialogue();
            }
            else
            {
                ShowNextLine();
            }
        }
    }

    void StartDialogue()
    {
        dialoguePanel.SetActive(true); // 대화창 패널을 활성화합니다.
        currentLine = 0; // 대화 인덱스를 초기화합니다.
        dialogueText.text = dialogueLines[currentLine]; // 첫 번째 대사를 표시합니다.
    }

    public void ShowNextLine()
    {
        currentLine++; // 다음 대사의 인덱스로 이동합니다.

        if (currentLine < dialogueLines.Length) // 대화 내용이 더 있다면
        {
            dialogueText.text = dialogueLines[currentLine]; // 다음 대사를 표시합니다.
        }
        else
        {
            dialoguePanel.SetActive(false); // 대화 내용이 더 이상 없으면 대화창을 숨깁니다.
        }
    }
}