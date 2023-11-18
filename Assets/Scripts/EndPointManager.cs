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

            UpdateRank();

            if (IsLocalPlayer) {
                playerEndPoint.rank = rank;
            }
        }
    }

    private void UpdateRank() {
        UIController.instance.UpdateRankUI(rank);
    }

}
