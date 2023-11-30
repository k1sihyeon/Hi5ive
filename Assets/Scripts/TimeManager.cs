using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TimeManager : NetworkBehaviour {

    [SerializeField] private float startCountdown = 10f;
    private bool isStart = false;

    void Start() {
        UIController.instance.EnableCountdown();
        //PlayerController.instance.ignoringInputs = true;
        //�÷��̾ �� �ʰ� �����ǹǷ� ���� x
    }

    private void FixedUpdate() {
        if (!isStart) {
            PlayerController.instance.UpdateIgnoringInputs(true);

            if (IsServer) {
                startCountdown -= Time.deltaTime;
                SendTimeClientRpc(startCountdown);
            }

            if (startCountdown <= 0) {
                UIController.instance.DisableCountdown();
                isStart = true;
                PlayerController.instance.UpdateIgnoringInputs(false);
            }

            UIController.instance.UpdateCountdown(startCountdown);
        }
    }

    [ClientRpc]
    private void SendTimeClientRpc(float time) {
        startCountdown = time;
    }
}
