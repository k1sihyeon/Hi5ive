using UnityEngine;
using TMPro; // TextMeshPro ����� ���� ���ӽ����̽�
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public TextMeshProUGUI dialogueText; // ��ȭâ�� �ؽ�Ʈ�� ǥ���� TextMeshPro UI ���
    public string[] dialogueLines; // ��ȭ ������ ������ �迭
    public GameObject dialoguePanel; // ��ȭâ �г�

    private int currentLine = 0; // ���� ǥ���ϰ� �ִ� ����� �ε���

    void Start()
    {
        dialoguePanel.SetActive(false); // ���� ���� �� ��ȭâ�� ����ϴ�.
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽��ٰ� ������
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
        dialoguePanel.SetActive(true); // ��ȭâ �г��� Ȱ��ȭ�մϴ�.
        currentLine = 0; // ��ȭ �ε����� �ʱ�ȭ�մϴ�.
        dialogueText.text = dialogueLines[currentLine]; // ù ��° ��縦 ǥ���մϴ�.
    }

    public void ShowNextLine()
    {
        currentLine++; // ���� ����� �ε����� �̵��մϴ�.

        if (currentLine < dialogueLines.Length) // ��ȭ ������ �� �ִٸ�
        {
            dialogueText.text = dialogueLines[currentLine]; // ���� ��縦 ǥ���մϴ�.
        }
        else
        {
            dialoguePanel.SetActive(false); // ��ȭ ������ �� �̻� ������ ��ȭâ�� ����ϴ�.
        }
    }
}