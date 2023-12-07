using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEndPoint : NetworkBehaviour {

    public static PlayerEndPoint instance;

    [SerializeField] private Vector3 observePoint = new Vector3(15, 25, -207);
    [SerializeField] private Vector3 startPoint = new Vector3(185, -2, 290);
    public int rank = -1;
    [SerializeField] private int endCountdown = 10;


    private void Awake() {
        if (PlayerEndPoint.instance == null) {
            PlayerEndPoint.instance = this;
        }
    }

    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("EndPoint")) {

            if(IsLocalPlayer) {
                SetPositionServerRpc(observePoint);
            }

            this.gameObject.transform.position = observePoint;
        }

        if(collision.gameObject.CompareTag("EndOfWorld")) {
            if (IsLocalPlayer) {
                SetPositionServerRpc(startPoint);
            }

            this.gameObject.transform.position = startPoint;
        }
    }

    public void UpdateRank(int val) {
        this.rank = val;
        Debug.Log("[Player] Rank: " + rank);
        PlayerPrefs.SetInt("Rank", rank);

        if(rank == 1) {
            Debug.Log("[Player] end countdown");

            StartCoroutine(
            this.gameObject.GetComponent<TimeManager>().EndCountdown()
            );
            StartEndCountdownServerRpc();
        }
    }

    [ServerRpc]
    public void SetPositionServerRpc(Vector3 point) {
        SetPositionClientRpc(point);
    }

    [ClientRpc]
    private void SetPositionClientRpc(Vector3 point) {
        this.gameObject.transform.position = point;
    }

    [ServerRpc]
    private void StartEndCountdownServerRpc() {
        Debug.Log("[Player serverRpc] end countdown");
        StartEndCountdownClientRpc();
        StartCoroutine(
            this.gameObject.GetComponent<TimeManager>().EndCountdown()
            );
    }

    [ClientRpc]
    private void StartEndCountdownClientRpc() {
        Debug.Log("[Player client rpc] end countdown");
        StartCoroutine(
            this.gameObject.GetComponent<TimeManager>().EndCountdown()
            );
    }


}
