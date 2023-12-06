using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class EndPointManager : NetworkBehaviour {

    [SerializeField] private int rank = 0;
    private PlayerEndPoint playerEndPoint;

    void Start() {
        //player = GetComponent<PlayerController>();
        playerEndPoint = GetComponent<PlayerEndPoint>();

        if(playerEndPoint == null) {
            Debug.Log("player end point is NULL!!");
            //생성 순서 때문에  null  임
        }
    }


    private void OnCollisionEnter(Collision collision) {
        if (!IsServer)
            return;

        if (collision.gameObject.CompareTag("Player")) {

            rank += 1;

            UpdateRankClientRpc(rank);
        }
    }

    private void UpdateRankUI(int rank) {
        UIController.instance.UpdateRankUI(rank);
    }

    [ClientRpc]
    private void UpdateRankClientRpc(int rank) {
        if (IsLocalPlayer) {
            Debug.Log("update rank client rpc");
            playerEndPoint = GetComponent<PlayerEndPoint>();
            playerEndPoint.rank = rank;
            UIController.instance.UpdateRankUI(rank);

            UpdateRankUI(rank);
        }
    }



}
