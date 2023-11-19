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
    private bool isNearNPC = false; // NPC 근처에 있는지 여부

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

        // NPC와 상호작용 시 대화 재개
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
            dialogueCompleted = true; // 대화 완료 플래그 설정
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = true; // NPC 근처에 있다고 표시
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = false; // NPC 근처에서 벗어났다고 표시
        }
    }
}
