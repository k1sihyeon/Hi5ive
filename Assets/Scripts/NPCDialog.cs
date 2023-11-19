using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class NPCDialog : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;
    public Transform cameraTransform;

    // 인스펙터에서 할당할 수 있는 NPC 오브젝트 배열
    public GameObject[] npcObjects;

    private int currentLine = 0;
    private bool isNearNPC = false;
    private MovementInput playerMovement; // 플레이어 움직임 스크립트의 참조


    private void Start()
    {
        dialoguePanel.SetActive(false);
        playerMovement = FindObjectOfType<MovementInput>(); // 플레이어 움직임 스크립트 찾기
    }
    void Update()
    {
        if (isNearNPC && Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialoguePanel.activeSelf)
            {
                dialoguePanel.SetActive(true);
                StartDialogue();
            }
            else
            {
                ShowNextLine();
            }
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (System.Array.Exists(npcObjects, npc => npc == other.gameObject))
        {
            Debug.Log("npc 주위에 있음");
            isNearNPC = true;
            AdjustCameraAngle();
            if (playerMovement != null) playerMovement.canMove = false; // 움직임 비활성화
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (System.Array.Exists(npcObjects, npc => npc == other.gameObject))
        {
            isNearNPC = false;
            if (playerMovement != null) playerMovement.canMove = true; // 움직임 활성화
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentLine = 0;
        if (playerMovement != null) playerMovement.canMove = true; // 대화가 끝나면 움직임 활성화
    }

    private void StartDialogue()
    {
        currentLine = 0;
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLines[currentLine];
    }

    private void ShowNextLine()
    {
        currentLine++;
        if (currentLine < dialogueLines.Length)
        {
            dialogueText.text = dialogueLines[currentLine];
        }
        else
        {
            EndDialogue();
        }
    }

    private void AdjustCameraAngle()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles += new Vector3(-10f, 0f, 0f);
        }
    }
}
