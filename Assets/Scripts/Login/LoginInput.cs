using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.Diagnostics;

public class LoginInput : MonoBehaviour
{
    
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private Button loginBtn;
    [SerializeField] private Button signupBtn;
    [SerializeField] private TMP_Text msgText;

    private void SetMessage(string msg) {
        msgText.text = msg;
        Debug.Log(msg);
    }

    private bool IsFieldEmpty() {
        if(idInputField.text == string.Empty && passwordInputField.text == string.Empty) {
            SetMessage("ID�� Password�� �Է��ϼ���");
            return true;
        }
        else if(idInputField.text == "") {
            SetMessage("ID�� �Է��ϼ���");
            return true;
        }
        else if(passwordInputField.text == "") {
            SetMessage("Password�� �Է��ϼ���");
            return true;
        }

        return false;
    }

    private void LoadScene(string sceneName = "LobbyScene") {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickSignup() {
        LoadScene("SignUpScene");
    }

    public void OnClickLogin() {
        SetMessage("");
        if (IsFieldEmpty()) return;

        string id = idInputField.text;
        string password = passwordInputField.text;

        idInputField.text = "";
        passwordInputField.text = "";

        loginBtn.interactable = false;

        StartCoroutine(nameof(LoginProcess));
        Login(id, password);
    }

    private void Login(string id, string password) {
        Backend.BMember.CustomLogin(id, password, callback => {
            StopCoroutine(nameof(LoginProcess));

            // �α��� ����
            if (callback.IsSuccess()) {
                Debug.Log("�α����� �����߽��ϴ�. : " + callback);
                SetMessage($"{id}�� ȯ���մϴ�.");

                // ������ �̵�
                LoadScene();
            }
            else {
                Debug.LogError("�α����� �����߽��ϴ�. : " + callback);
                
                loginBtn.interactable = true;

                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode())) {
                    case 401:   // �������� �ʴ� ���̵�, �߸��� ��й�ȣ
                        message = callback.GetMessage().Contains("customId") ? "�������� �ʴ� ���̵��Դϴ�." : "�߸��� ��й�ȣ �Դϴ�.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                SetMessage(message);
            }
        });
    }

    private IEnumerator LoginProcess() {
        float time = 0;

        while (true) {
            time += Time.deltaTime;
            SetMessage($"�α��� ���Դϴ�... {time:F1}");

            yield return null;
        }
    }

}
