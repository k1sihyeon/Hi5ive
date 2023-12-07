using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class EndPointManager : NetworkBehaviour {

    [SerializeField] private int rank = 0;
    private PlayerEndPoint playerEndPoint;
    private bool coolDown = true;

    void Start() {

    }


    private void OnCollisionEnter(Collision collision) {

        if (collision.gameObject.CompareTag("Player")) {

            rank += 1;
            //Debug.Log("[EndPoint] Rank: " + rank.Value);
            Debug.Log("[EndPoint] Rank: " + rank);


            UpdateRankServerRpc(rank);
            Debug.Log("[EndPoint] Rank: " + rank);
            UpdateRankUI(rank);

        }
    }

    private IEnumerator ResetCoolDown(float delay) {
        yield return new WaitForSeconds(delay);
        coolDown = true;
    }

    private void UpdateRankUI(int rank) {
        UIController.instance.UpdateRankUI(rank);
    }

    [ServerRpc]
    private void UpdateRankServerRpc(int val) {
        Debug.Log("[Server Rpc] Rank: " + rank);
        Debug.Log("[Server Rpc] val: " + val);
        rank = val;
        Debug.Log("[Server Rpc] rank: " + rank);

        UpdateRankClientRpc(val);
    }

    [ClientRpc]
    private void UpdateRankClientRpc(int val) {
        Debug.Log("[Client Rpc] Rank: " + rank);
        Debug.Log("[Client Rpc] val: " + val);
        rank = val;
        Debug.Log("[Client Rpc] Rank: " + rank);
    }

}
