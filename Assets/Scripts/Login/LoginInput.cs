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
            SetMessage("ID와 Password를 입력하세요");
            return true;
        }
        else if(idInputField.text == "") {
            SetMessage("ID를 입력하세요");
            return true;
        }
        else if(passwordInputField.text == "") {
            SetMessage("Password를 입력하세요");
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

            // 로그인 성공
            if (callback.IsSuccess()) {
                Debug.Log("로그인이 성공했습니다. : " + callback);
                SetMessage($"{id}님 환영합니다.");

                // 씬으로 이동
                LoadScene();
            }
            else {
                Debug.LogError("로그인이 실패했습니다. : " + callback);
                
                loginBtn.interactable = true;

                string message = string.Empty;

                switch (int.Parse(callback.GetStatusCode())) {
                    case 401:   // 존재하지 않는 아이디, 잘못된 비밀번호
                        message = callback.GetMessage().Contains("customId") ? "존재하지 않는 아이디입니다." : "잘못된 비밀번호 입니다.";
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
            SetMessage($"로그인 중입니다... {time:F1}");

            yield return null;
        }
    }

}
