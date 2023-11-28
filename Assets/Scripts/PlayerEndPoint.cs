using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEndPoint : NetworkBehaviour {

    [SerializeField] private Vector3 observePoint = new Vector3(15, 25, -207);
    [SerializeField] private Vector3 startPoint = new Vector3(185, -2, 290);
    public int rank = 0;

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("EndPoint")) {

            if(IsLocalPlayer) {
                Debug.Log("IS PLAYER!! collision");
                SetPositionServerRpc(observePoint);
            }

            Debug.Log("Player End Point");
            this.gameObject.transform.position = observePoint;
        }

        if(collision.gameObject.CompareTag("EndOfWorld")) {
            if (IsLocalPlayer) {
                Debug.Log("IS PLAYER!! collision");
                SetPositionServerRpc(startPoint);
            }

            Debug.Log("Player End Point");
            this.gameObject.transform.position = startPoint;
        }
    }

    [ServerRpc]
    private void SetPositionServerRpc(Vector3 point) {
        Debug.Log("PLAYER!! position server rpc");
        SetPositionClientRpc(point);
    }

    [ClientRpc]
    private void SetPositionClientRpc(Vector3 point) {
        Debug.Log("PLAYER!! position client rpc");

        this.gameObject.transform.position = point;
    }

}
