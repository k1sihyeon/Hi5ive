using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class TimeManager : NetworkBehaviour {

    private float startCountdown = 30f;
    private bool isStart = false;

    void Start() {
        if (IsServer) {

        }

        else if (IsClient) {

        }

        UIController.instance.EnableCountdown();
    }

    private void FixedUpdate() {
        if (!isStart) {

            if (IsServer) {
                startCountdown -= Time.deltaTime;
                SendTimeClientRpc(startCountdown);

                if(startCountdown <= 0) {
                    UIController.instance.DisableCountdown();
                    isStart = true;
                }
            }

            UIController.instance.UpdateCountdown(startCountdown);

        }
    }

    void Update() {

    }

    [ClientRpc]
    private void SendTimeClientRpc(float time) {
        startCountdown = time;
    }
}
