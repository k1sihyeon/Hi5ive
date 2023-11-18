using System.Collections;
using System.Collections.Generic;
using Unity.Multiplayer.Samples.Utilities.ClientAuthority;
using Unity.Netcode;
using UnityEngine;

public class EndPointManager : NetworkBehaviour {

    private Vector3 observePoint = new Vector3(15, 25, -207);
    private int rank = 0;


    // Start is called before the first frame update
    void Start() {

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
        }
    }

    private void SetPosition(GameObject obj, Vector3 point) {

        ulong objNetworkId = obj.GetComponent<NetworkObject>().NetworkObjectId;
        obj.transform.position = point;

        SetPositionClientRpc(objNetworkId, point);

    }

    [ClientRpc]
    private void SetPositionClientRpc(ulong objNetworkdId, Vector3 point) {
        Debug.Log("position client rpc");

        NetworkObject networkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[objNetworkdId];
        networkObject.gameObject.transform.position = point;

    }
}
