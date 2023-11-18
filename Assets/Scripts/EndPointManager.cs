using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class EndPointManager : NetworkBehaviour {

    private Vector3 observePoint = new Vector3(15, 25, -207);
    private int rank = 0;
    private PlayerController player;
    [SerializeField] private PlayerEndPoint playerEndPoint;

    // Start is called before the first frame update
    void Start() {
        //player = GetComponent<PlayerController>();
        playerEndPoint = GetComponent<PlayerEndPoint>();

        if(playerEndPoint == null) {
            Debug.Log("player end point is NULL!!");
            //생성 순서 때문에  null  임
        }
    }


    // Update is called once per frame
    void Update() {

    }

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer)
            return;

        if (collision.gameObject.CompareTag("Player")) {
            Debug.Log("End Point");
            SetPosition(collision.gameObject, observePoint);

            //등수 계산
            //player 등수 할당
            //player ui에 표시
            rank += 1;

            UpdateRank();

            if (IsLocalPlayer) {



                playerEndPoint.rank = rank;
                playerEndPoint.ActivateRankUI();
            }




        }
    }

    private void UpdateRank() {
        //if(IsLocalPlayer) {

            UIController.instance.UpdateRankUI(rank);
            
        //}
    }

    private void SetPosition(GameObject obj, Vector3 point) {
        Debug.Log("set position");

        ulong objNetworkId = obj.GetComponent<NetworkObject>().NetworkObjectId;
        obj.transform.position = point;
        //player.gameObject.transform.position = point;

        SetPositionServerRpc(objNetworkId, point);

    }

    [ServerRpc]
    private void SetPositionServerRpc(ulong objNetworkdId, Vector3 point) {
        Debug.Log("position server rpc");

        SetPositionClientRpc(objNetworkdId, point);
    }

    [ClientRpc]
    private void SetPositionClientRpc(ulong objNetworkdId, Vector3 point) {
        Debug.Log("position client rpc");

        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objNetworkdId];
        networkObject.gameObject.transform.position = point;
        //player.gameObject.transform.position = point;

    }
}
