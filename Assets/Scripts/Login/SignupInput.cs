using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using BackEnd;

public class SignupInput : MonoBehaviour
{
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private TMP_InputField nicknameInputField;

    [SerializeField] private Button signupBtn;
    [SerializeField] private TMP_Text msgText;

    private void SetMessage(string msg) {
        msgText.text = msg;
        Debug.Log(msg);
    }

    private bool IsFieldEmpty() {
        if (idInputField.text == string.Empty && passwordInputField.text == string.Empty && nicknameInputField.text == string.Empty) {
            SetMessage("ID, Password, Nickname을 입력하세요");
            return true;
        }
        else if (idInputField.text == "") {
            SetMessage("ID를 입력하세요");
            return true;
        }
        else if (passwordInputField.text == "") {
            SetMessage("Password를 입력하세요");
            return true;
        }
        else if (nicknameInputField.text == "") {
            SetMessage("Nickname을 입력하세요");
            return true;
        }

        return false;
    }

    private void LoadScene(string sceneName = "LoginScene") {
        SceneManager.LoadScene(sceneName);
    }

    public void OnClickToPrevious() {
        LoadScene();
    }

    public void OnClickSignup() {
        SetMessage("");
        if (IsFieldEmpty()) return;

        string id = idInputField.text;
        string password = passwordInputField.text;
        string nickname = nicknameInputField.text;

        idInputField.text = "";
        passwordInputField.text = "";
        nicknameInputField.text = "";

        signupBtn.interactable = false;

        StartCoroutine(nameof(SignupProcess));
        Signup(id, password, nickname);
    }


    private void Signup(string id, string password, string nickname) {
        Backend.BMember.CustomSignUp(id, password, callback => {
            StopCoroutine(nameof(SignupProcess));

            if (callback.IsSuccess()) {
                Debug.Log("회원가입에 성공했습니다. : " + callback);
                //TODO: 닉네임 설정 필요!!!!!

                SetMessage("로그인 화면으로 이동합니다.");
                LoadScene();
            }
            else {
                Debug.LogError("회원가입에 실패했습니다. : " + callback);

                signupBtn.interactable = true;

                string message = string.Empty;

                switch(int.Parse(callback.GetStatusCode()) ) {
                    case 409:
                        message = "중복된 ID 입니다.";
                        break;
                    default:
                        message = callback.GetMessage();
                        break;
                }

                SetMessage(message);
            }
        });
    }


    private IEnumerator SignupProcess() {
        float time = 0;

        while (true) {
            time += Time.deltaTime;
            SetMessage($"회원가입 중입니다... {time:F1}");

            yield return null;
        }
    }
}
