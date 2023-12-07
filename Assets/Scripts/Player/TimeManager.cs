using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class TimeManager : NetworkBehaviour {

    private PlayerController player;
    [SerializeField] private float startCountdown = 10f;
    [SerializeField] private int endCountdown = 10;
    private bool isStart = false;

    public static TimeManager instance;

    private void Awake() {
        if (TimeManager.instance == null) {
            TimeManager.instance = this;
        }
    }

    void Start() {
        UIController.instance.EnableCountdown();
        player = GetComponent<PlayerController>();
        //PlayerController.instance.ignoringInputs = true;
        //플레이어가 더 늦게 생성되므로 실행 x

        if(IsOwner) {
            SyncTimeServerRpc(startCountdown);
        }
    }

    private void FixedUpdate() {
        if (!isStart) {
            PlayerController.instance.ignoringInputs = true;
            UpdateIgnoringInputsClientRpc(true);

            if (IsServer) {
                startCountdown -= Time.deltaTime;
                SendTimeClientRpc(startCountdown);
            }

            if (startCountdown <= 0) {
                isStart = true;
                UIController.instance.DisableCountdown();
                PlayerController.instance.ignoringInputs = false;
                UpdateIgnoringInputsClientRpc(false);
            }

            UIController.instance.UpdateCountdown(startCountdown);
        }
    }

    public IEnumerator EndCountdown() {
        Debug.Log("[Time] end countdown");
        UIController.instance.EnableCountdown();

        for (int i = endCountdown; i >= 0; i--) {
            UIController.instance.UpdateCountdownInt(i);
            yield return new WaitForSeconds(1);
        }

        UIController.instance.DisableCountdown();

        //Scene Load

    }


    [ClientRpc]
    private void SendTimeClientRpc(float time) {
        startCountdown = time;
    }

    [ClientRpc]
    private void UpdateIgnoringInputsClientRpc(bool value) {
        player = GetComponent<PlayerController>();
        player.ignoringInputs = value;
    }

    [ServerRpc]
    private void SyncTimeServerRpc(float time) {
        startCountdown = time;
        SendTimeClientRpc(time);
    }

}
