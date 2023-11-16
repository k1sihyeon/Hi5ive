using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using UnityEngine.SceneManagement;

public class Hi5iveNetworkManager : MonoBehaviour {
    public NetworkManager networkManager;
    public static Hi5iveNetworkManager Singleton;

    private string ipAddr;


    public void SetIp(string ip) {
        networkManager.GetComponent<UnityTransport>().ConnectionData.Address = ip;
    }


    private void HandleServerStarted() {
        networkManager.SceneManager.LoadScene("Stage1TestingScene", LoadSceneMode.Single);
    }

    private void Awake() {
        DontDestroyOnLoad(this);

        Singleton = this; 
    }

    void Start()
    {
        networkManager = NetworkManager.Singleton;

        string playerType = PlayerPrefs.GetString("PlayerType");
        
        if (playerType == "Client") {
            ipAddr = PlayerPrefs.GetString("IP");
            SetIp(ipAddr);
            networkManager.StartClient();
        }
        else if (playerType == "Host") {
            networkManager.StartHost();
        }
        else {
            //err
        }

        
        //networkManager.OnServerStarted += HandleServerStarted;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
