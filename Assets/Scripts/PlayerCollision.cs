using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCollision : NetworkBehaviour {
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    private void OnCollisionEnter(Collision collision) {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Enemy")) {
            Debug.Log("Collision Enterd");

            ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            Destroy(collision.gameObject);
            CollisionClientRpc(enemyNetworkId);
        }

    }

    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.CompareTag("Enenmy")) {
            Destroy(other.gameObject);
        }
    }

    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId) {
        if(!IsLocalPlayer) {
            NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
            Destroy(enemyNetworkObject.gameObject);
        }
    }
}
