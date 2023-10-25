using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoingInput : MonoBehaviour
{
    
    [SerializeField] private TMP_InputField idInputField;
    [SerializeField] private TMP_InputField passwordInputField;
    [SerializeField] private GameObject loginBtn;
    [SerializeField] private GameObject signinBtn;
    private string id;
    private string password;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnClickLogin()
    {
        id = idInputField.text;
        password = passwordInputField.text;

        Login();
    }

    async void Login()
    {
        await Task.Run(() =>
        {
            
            BackendLogin.Instance.CustomLogin(id, password); // 뒤끝 로그인
            
            Debug.Log("UI 로그인 테스트 종료");
        });
    }
}
