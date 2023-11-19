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

    // �ν����Ϳ��� �Ҵ��� �� �ִ� NPC ������Ʈ �迭
    public GameObject[] npcObjects;

    private int currentLine = 0;
    private bool isNearNPC = false;

    private void Start()
    {
        dialoguePanel.SetActive(false);
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
        // npcObjects �迭�� �ִ��� Ȯ��
        if (System.Array.Exists(npcObjects, npc => npc == other.gameObject))
        {
            isNearNPC = true;
            AdjustCameraAngle();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (System.Array.Exists(npcObjects, npc => npc == other.gameObject))
        {
            isNearNPC = false;
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
    }

    private void AdjustCameraAngle()
    {
        if (cameraTransform != null)
        {
            cameraTransform.localEulerAngles += new Vector3(-10f, 0f, 0f);
        }
    }
}
