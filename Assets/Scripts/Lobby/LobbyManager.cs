using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{

    [SerializeField] private TMP_InputField localIpInputField;
    [SerializeField] private Button localJoinBtn;
    [SerializeField] private TMP_Dropdown localDD;
    [SerializeField] private TMP_Dropdown hostDD;

    [SerializeField] private TMP_Text ipText;
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
                SetPopUp("�Էµ� IP�� �ܺ� IP�Դϴ�.");
                return false;
            }
        }
        else {
            SetPopUp("�ùٸ��� ���� IP �ּ� �����Դϴ�.");
            return false;
        }

        return false;
    }

    bool IsInternalIP(IPAddress ipAddress) {
        byte[] addressBytes = ipAddress.GetAddressBytes();

        // ���� ������ ���� Ȯ��
        if ((addressBytes[0] == 10) ||
            (addressBytes[0] == 172 && (addressBytes[1] >= 16 && addressBytes[1] <= 31)) ||
            (addressBytes[0] == 192 && addressBytes[1] == 168) ||
            (addressBytes[0] == 127 && addressBytes[1] == 0)) {
            return true;
        }

        return false;
    }

    private void Awake() {
        localJoinBtn.onClick.AddListener(() => {
            if(checkInField()) {
                //join
                PlayerPrefs.SetString("PlayerType", "Client");
                PlayerPrefs.SetString("SceneName", localDD.itemText.ToString());
                PlayerPrefs.SetString("IP", localIpInputField.text);


                int ddIdx = localDD.value;
                string ddText = localDD.options[ddIdx].text;
                SceneManager.LoadScene(ddText);
            }
        });

        hostBtn.onClick.AddListener(() => {
            PlayerPrefs.SetString("PlayerType", "Host");
            PlayerPrefs.SetString("SceneName", hostDD.itemText.ToString());

            int ddIdx = hostDD.value;
            string ddText = hostDD.options[ddIdx].text;
            SceneManager.LoadScene(ddText);
        });

    }


    void Start() {
        popup.SetActive(false);
        ipText.text = "";

        var host = Dns.GetHostEntry(Dns.GetHostName());
        foreach (var ip in host.AddressList) {

            if (ip.AddressFamily == AddressFamily.InterNetwork) {
                ipText.text += (ip.ToString() + "\n");
                Debug.Log("IP Address = " + ip.ToString());
            }

        }
    }
    void Update()
    {
        
    }
}
