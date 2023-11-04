using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class BackendManager : MonoBehaviour
{
    //��ȣȭ �ʿ�
    private string id;
    private string password;
    private string username;

    private void BackendSetup()
    {
        var bro = Backend.Initialize(true); // �ڳ� �ʱ�ȭ

        // �ڳ� �ʱ�ȭ�� ���� ���䰪
        if (bro.IsSuccess())
        {
            Debug.Log("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 204 Success
        }
        else
        {
            Debug.LogError("�ʱ�ȭ ���� : " + bro); // ������ ��� statusCode 400�� ���� �߻� 
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

    //���� �Լ��� �񵿱⿡�� ȣ���ϰ� ���ִ� �Լ� (����Ƽ UI ���� �Ұ�)
    //���� �Լ� ȣ���� Ȯ���ϰ� ����Ǿ�� �ϴ� ��ɿ��� ���
    //�ΰ��� ȣ���� �񵿱��
    async void Test()
    {
        await Task.Run(() =>
        {
            //BackendLogin.Instance.CustomSignUp("user1", "1234"); // ȸ������ �Լ�
            BackendLogin.Instance.CustomLogin("user1", "1234"); // �ڳ� �α���
            BackendLogin.Instance.UpdateNickname("Hi5ive GM"); // �г��� ����
            Debug.Log("�׽�Ʈ ����");
        });
    }

    async void GameData() {
        await Task.Run(() => {
            BackendLogin.Instance.CustomLogin("user1", "1234");
            BackendGameData.Instance.GameDataInsert();

            Debug.Log("�׽�Ʈ ����");
        });
    }

}
