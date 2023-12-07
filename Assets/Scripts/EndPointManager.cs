using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class EndPointManager : NetworkBehaviour {

    [SerializeField] private int rank = 0;
    private List<ulong> ranks = new List<ulong>();
    private PlayerEndPoint playerEndPoint;
    private bool coolDown = true;

    void Start() {
        //player = GetComponent<PlayerController>();
        playerEndPoint = GetComponent<PlayerEndPoint>();

        if(playerEndPoint == null) {
            Debug.Log("player end point is NULL!!");
            //생성 순서 때문에  null  임
        }
    }


    private void OnCollisionEnter(Collision collision) {
        //if (!IsServer)
        //    return;


        if (collision.gameObject.CompareTag("Player")) {
            NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

            if (!ranks.Exists(x => x == networkObject.OwnerClientId)) {
                ranks.Add(networkObject.OwnerClientId);
            }

            //UpdateRanksServerRpc(ranks);

            rank = ranks.IndexOf(networkObject.OwnerClientId) + 1;

            collision.gameObject.GetComponent<PlayerEndPoint>().rank = rank;
            UpdateRankUI(rank);
        }


        //if (collision.gameObject.CompareTag("Player")) {

        //    if (coolDown) {
        //        coolDown = false;

        //        rank += 1;

        //        Debug.Log("world Rank: " + rank);

        //        UpdateRankUI(rank);

        //        ResetCoolDown(1f);
        //    }

        //}
    }

    private IEnumerator ResetCoolDown(float delay) {
        yield return new WaitForSeconds(delay);
        coolDown = true;
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
            //UIController.instance.UpdateRankUI(rank);

            UpdateRankUI(rank);
        }
    }

    //[ServerRpc]
    //private void UpdateRanksServerRpc(List<ulong> ranks) {
    //    this.ranks = ranks;
    //    UpdateRanksClientRpc(ranks);
    //}

    //[ClientRpc]
    //private void UpdateRanksClientRpc(List<ulong> ranks) {
    //    this.ranks = ranks;
    //}


}
