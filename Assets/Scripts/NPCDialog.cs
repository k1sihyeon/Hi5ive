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

    private int currentLine = 0;
    private bool isNearNPC = false;

    void Update()
    {
        if (isNearNPC && Input.GetKeyDown(KeyCode.Space))
        {
            if (!dialoguePanel.activeSelf)
            {
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
        Debug.Log("Trigger entered: " + other.gameObject.name); // 디버깅
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = true;
            AdjustCameraAngle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("NPC"))
        {
            isNearNPC = false;
        }
    }

    private void StartDialogue()
    {
        currentLine = 4; // Element 4부터 시작 (인덱스는 0부터 시작하므로 3으로 설정)
        dialoguePanel.SetActive(true);
        dialogueText.text = dialogueLines[currentLine];
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
            EndDialogue();
        }
    }

    private void EndDialogue()
    {
        dialoguePanel.SetActive(false);
        currentLine = 4; // 대화를 다시 시작할 때 Element 4부터 시작하도록 설정
    }

    private void AdjustCameraAngle()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles += new Vector3(-10f, 0f, 0f);
        }
    }
}
