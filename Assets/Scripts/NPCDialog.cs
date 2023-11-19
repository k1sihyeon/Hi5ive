using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // 추가된 네임스페이스

public class NPCDialog : MonoBehaviour
{
    public TextMeshProUGUI dialogueText;
    public string[] dialogueLines;
    public GameObject dialoguePanel;
    public Transform cameraTransform;

    public GameObject[] npcObjects;

    private int currentLine = 0;
    private bool isNearNPC = false;
    private MovementInput playerMovement;

    private void Start()
    {
        dialoguePanel.SetActive(false);
        playerMovement = FindObjectOfType<MovementInput>();
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
            if (playerMovement != null) playerMovement.canMove = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (System.Array.Exists(npcObjects, npc => npc == other.gameObject))
        {
            isNearNPC = false;
            if (playerMovement != null) playerMovement.canMove = true;
        }
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

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentLine = 0;
        if (playerMovement != null) playerMovement.canMove = true;

        // "LobbyScene"으로 씬 전환
        SceneManager.LoadScene("LobbyScene");
    }

    private void AdjustCameraAngle()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles += new Vector3(-10f, 0f, 0f);
        }
    }
}
