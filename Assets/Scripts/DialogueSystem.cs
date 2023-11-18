using UnityEngine;
using TMPro; // TextMeshPro 사용을 위한 네임스페이스
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false; // 대화 완료 플래그
    public TextMeshProUGUI dialogueText; // 대화창에 텍스트를 표시할 TextMeshPro UI 요소
    public string[] dialogueLines; // 대화 내용을 저장할 배열
    public GameObject dialoguePanel; // 대화창 패널

    private int currentLine = 0; // 현재 표시하고 있는 대사의 인덱스
    private int spacePressCount = 0; // 스페이스바 누른 횟수를 추적하는 변수
    private const int maxSpacePressCount = 5; // 최대 스페이스바 누른 횟수

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // 스페이스바가 눌리면
        {
            spacePressCount++; // 스페이스바 누른 횟수 증가

            if (spacePressCount > maxSpacePressCount)
            {
                return; // 스페이스바를 5번 초과로 눌렀다면 아무 작업도 하지 않음
            }

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
    void Start()
    {
        dialoguePanel.SetActive(false); // 게임 시작 시 대화창을 숨깁니다.
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

        if (currentLine >= dialogueLines.Length)
        {
            dialoguePanel.SetActive(false);
            dialogueCompleted = true; // 대화가 완료되었다고 표시
        }
    }
}