using System.Collections;
using System.Collections.Generic;
using System.Net;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private TMP_InputField localIpInputField;
    [SerializeField] private Button localJoinBtn;
    [SerializeField] private TMP_Dropdown dropdown;

    [SerializeField] private Button hostBtn;

    [SerializeField] private TMP_Text popupText;
    [SerializeField] private GameObject popup;

    private void SetPopUp(string msg) {
        popupText.text = msg;
        popup.SetActive(true);
    }

    private bool checkInField() {
        IPAddress ipAddr;

        if (localIpInputField.text == string.Empty) {
            return false;
        }

        if (IPAddress.TryParse(localIpInputField.text, out ipAddr)) {
            if (IsInternalIP(ipAddr)) {
                return true;
            }
            else {
                SetPopUp("입력된 IP는 외부 IP입니다.");
                return false;
            }
        }
        else {
            SetPopUp("올바르지 않은 IP 주소 형식입니다.");
            return false;
        }

        return false;
    }

    bool IsInternalIP(IPAddress ipAddress) {
        byte[] addressBytes = ipAddress.GetAddressBytes();

        // 내부 아이피 범위 확인
        if ((addressBytes[0] == 10) ||
            (addressBytes[0] == 172 && (addressBytes[1] >= 16 && addressBytes[1] <= 31)) ||
            (addressBytes[0] == 192 && addressBytes[1] == 168)) {
            return true;
        }

        return false;
    }

    private void Awake() {
        localJoinBtn.onClick.AddListener(() => {
            if(checkInField()) {
                //join
                PlayerPrefs.SetString("PlayerType", "Client");
                PlayerPrefs.SetString("SceneName", dropdown.itemText.ToString());
                PlayerPrefs.SetString("IP", localIpInputField.text);


                int ddIdx = dropdown.value;
                string ddText = dropdown.options[ddIdx].text;
                SceneManager.LoadScene(ddText);
                //SceneManager.LoadScene("Stage1TestingScene", LoadSceneMode.Single);


            }
        });

        hostBtn.onClick.AddListener(() => {
            PlayerPrefs.SetString("PlayerType", "Host");
            PlayerPrefs.SetString("SceneName", dropdown.itemText.ToString());

            int ddIdx = dropdown.value;
            string ddText = dropdown.options[ddIdx].text;
            SceneManager.LoadScene(ddText);
            //SceneManager.LoadScene("Stage1TestingScene", LoadSceneMode.Single);
        });

    }


    // Start is called before the first frame update
    void Start()
    {
        popup.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
