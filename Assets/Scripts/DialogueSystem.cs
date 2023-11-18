using UnityEngine;
using TMPro; // TextMeshPro ����� ���� ���ӽ����̽�
using System.Collections;

public class DialogueSystem : MonoBehaviour
{
    public bool dialogueCompleted = false; // ��ȭ �Ϸ� �÷���
    public TextMeshProUGUI dialogueText; // ��ȭâ�� �ؽ�Ʈ�� ǥ���� TextMeshPro UI ���
    public string[] dialogueLines; // ��ȭ ������ ������ �迭
    public GameObject dialoguePanel; // ��ȭâ �г�

    private int currentLine = 0; // ���� ǥ���ϰ� �ִ� ����� �ε���
    private int spacePressCount = 0; // �����̽��� ���� Ƚ���� �����ϴ� ����
    private const int maxSpacePressCount = 5; // �ִ� �����̽��� ���� Ƚ��

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) // �����̽��ٰ� ������
        {
            spacePressCount++; // �����̽��� ���� Ƚ�� ����

            if (spacePressCount > maxSpacePressCount)
            {
                return; // �����̽��ٸ� 5�� �ʰ��� �����ٸ� �ƹ� �۾��� ���� ����
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
        dialoguePanel.SetActive(false); // ���� ���� �� ��ȭâ�� ����ϴ�.
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

        if (currentLine >= dialogueLines.Length)
        {
            dialoguePanel.SetActive(false);
            dialogueCompleted = true; // ��ȭ�� �Ϸ�Ǿ��ٰ� ǥ��
        }
    }
}