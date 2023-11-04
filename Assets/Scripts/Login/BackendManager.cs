using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    //암호화 필요
    private string id;
    private string password;
    private string username;

    private void BackendSetup()
    {
        var bro = Backend.Initialize(true); // 뒤끝 초기화

        // 뒤끝 초기화에 대한 응답값
        if (bro.IsSuccess())
        {
            Debug.Log("초기화 성공 : " + bro); // 성공일 경우 statusCode 204 Success
        }
        else
        {
            Debug.LogError("초기화 실패 : " + bro); // 실패일 경우 statusCode 400대 에러 발생 
        }

        //Test();
        //GameData();
    }

    private void Awake() {
        DontDestroyOnLoad(gameObject);
        BackendSetup();
    }


    void Update()
    {
        if(Backend.IsInitialized) {
            Backend.AsyncPoll();
        }
    }

    //동기 함수를 비동기에서 호출하게 해주는 함수 (유니티 UI 접근 불가)
    //동기 함수 호출은 확실하게 진행되어야 하는 기능에만 사용
    //인게임 호출은 비동기로
    async void Test()
    {
        await Task.Run(() =>
        {
            //BackendLogin.Instance.CustomSignUp("user1", "1234"); // 회원가입 함수
            BackendLogin.Instance.CustomLogin("user1", "1234"); // 뒤끝 로그인
            BackendLogin.Instance.UpdateNickname("Hi5ive GM"); // 닉네임 변겅
            Debug.Log("테스트 종료");
        });
    }

    async void GameData() {
        await Task.Run(() => {
            BackendLogin.Instance.CustomLogin("user1", "1234");
            BackendGameData.Instance.GameDataInsert();

            Debug.Log("테스트 종료");
        });
    }

}
