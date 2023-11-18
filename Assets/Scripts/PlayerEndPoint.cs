using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEndPoint : NetworkBehaviour {

    private Vector3 observePoint = new Vector3(15, 25, -207);
    public int rank = 0;

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("EndPoint")) {

            if(IsLocalPlayer) {
                Debug.Log("IS PLAYER!! collision");
                SetPositionServerRpc();
            }

            Debug.Log("Player End Point");
            this.gameObject.transform.position = observePoint;
        }
    }

    [ServerRpc]
    private void SetPositionServerRpc() {
        Debug.Log("PLAYER!! position server rpc");
        SetPositionClientRpc(observePoint);
    }

    [ClientRpc]
    private void SetPositionClientRpc(Vector3 point) {
        Debug.Log("PLAYER!! position client rpc");

        this.gameObject.transform.position = observePoint;
    }

}
