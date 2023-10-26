using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class LoginInput : MonoBehaviour
{
    
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject signinBtn;

    public bool isLoginSuccess;
    public bool isSigninSuccess;

    private string id;
    private string password;

    private static BackendReturnObject bro;

    // Start is called before the first frame update
    void Start()
    {
      
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void LoadScene(string sceneName = "practice Scene") {
        SceneManager.LoadScene(sceneName);

    }

    public void OnClickSignin()
    {
        id = idInputField.text;
        password = passwordInputField.text;

        Signin();

        idInputField.text = "";
        passwordInputField.text = "";
    }

    public void OnClickLogin()
    {
        id = idInputField.text;
        password = passwordInputField.text;

        Login();

        idInputField.text = "";
        passwordInputField.text = "";

        //if (bro.IsSuccess()) {
        //    //�α��� ����
        //    //������ ��ȯ

        //    LoadScene();
        //}
        //else {
        //    //�α��� ����
        //    //���� �˾�â

        //    Debug.Log("!!!!!!!!!!! �α��� �ȵ� !!!!!!!!!!!!!");
        //}
    }

    async void Login()
    {
        await Task.Run(() =>
        {
            
            bro = BackendLogin.Instance.CustomLogin(id, password); // �ڳ� �α���

            Debug.Log("UI �α��� �Լ� ����");
        });
    }

    async void Signin()
    {
        await Task.Run(() =>
        {
            BackendLogin.Instance.CustomSignUp(id, password);

            Debug.Log("UI ȸ������ �Լ� ����");
        });
    }
}
