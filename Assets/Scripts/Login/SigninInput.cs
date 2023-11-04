using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class SigninInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField nicknameInputField;

    [SerializeField] private Button signinBtn;
    [SerializeField] private TMP_Text msgText;

    private void SetMessage(string msg) {
        msgText.text = msg;
        Debug.Log(msg);
    }

    private bool IsFieldEmpty() {
        if (idInputField.text == string.Empty && passwordInputField.text == string.Empty && nicknameInputField.text == string.Empty) {
            SetMessage("ID, Password, Nickname�� �Է��ϼ���");
            return true;
        }
        else if (idInputField.text == "") {
            SetMessage("ID�� �Է��ϼ���");
            return true;
        }
        else if (passwordInputField.text == "") {
            SetMessage("Password�� �Է��ϼ���");
            return true;
        }
        else if (nicknameInputField.text == "") {
            SetMessage("Nickname�� �Է��ϼ���");
            return true;
        }

        return false;
    }

    private void LoadScene(string sceneName = "LoginScene") {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickSignin() {
        SetMessage("");
        if (IsFieldEmpty()) return;

        string id = idInputField.text;
        string password = passwordInputField.text;
        string nickname = nicknameInputField.text;

        idInputField.text = "";
        passwordInputField.text = "";
        nicknameInputField.text = "";

        signinBtn.interactable = false;

        StartCoroutine(nameof(SigninProcess));
        Signin(id, password, nickname);
    }


    private void Signin(string id, string password, string nickname) {
        Backend.BMember.CustomSignUp(id, password, callback => {
            StopCoroutine(nameof(SigninProcess));

            if (callback.IsSuccess()) {
                Debug.Log("ȸ�����Կ� �����߽��ϴ�. : " + callback);
                //TODO: �г��� ���� �ʿ�!!!!!

                SetMessage("�α��� ȭ������ �̵��մϴ�.");
                LoadScene();
            }
            else {
                Debug.LogError("ȸ�����Կ� �����߽��ϴ�. : " + callback);

                signinBtn.interactable = true;

                string message = string.Empty;

                switch(int.Parse(callback.GetStatusCode()) ) {
                    case 409:
                        message = "�ߺ��� ID �Դϴ�.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                SetMessage(message);
            }
        });
    }


    private IEnumerator SigninProcess() {
        float time = 0;

        while (true) {
            time += Time.deltaTime;
            SetMessage($"ȸ������ ���Դϴ�... {time:F1}");

            yield return null;
        }
    }
}
