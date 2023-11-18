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
    private PlayerEndPoint playerEndPoint;

    // Start is called before the first frame update
    void Start() {
        //player = GetComponent<PlayerController>();
        playerEndPoint = GetComponent<PlayerEndPoint>();
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

            //��� ���
            //player ��� �Ҵ�
            //player ui�� ǥ��
            rank += 1;

            if (IsLocalPlayer) {
                playerEndPoint.rank = rank;
                playerEndPoint.ActivateRankUI();
            }




        }
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
