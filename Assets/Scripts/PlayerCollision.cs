using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCollision : NetworkBehaviour {
    
    private int playerEnergy = 0;

    private void Ultimate() {
        Debug.Log("Ultimate");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    private void Start() {
        
    }

    private void Update() {
        if (!IsClient) return;

        if(IsLocalPlayer) {
            if (playerEnergy >= 100) {
                playerEnergy = 0;

                Ultimate();
            }
        }

        

    }

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer) return;

        if (collision.gameObject.CompareTag("Enemy")) {
            Debug.Log("Collision Enterd");

            playerEnergy += 50;
            Debug.Log("Player Energy: " + playerEnergy);

            ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            Destroy(collision.gameObject);
            CollisionClientRpc(enemyNetworkId);
        }

        if (collision.gameObject.CompareTag("BallObstacle")) {
            Debug.Log("Collision Enterd");

            playerEnergy += 100;
            Debug.Log("Player Energy: " + playerEnergy);

            ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
            Destroy(collision.gameObject);
            CollisionClientRpc(enemyNetworkId);
        }

    }


    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId) {
        //if(!IsLocalPlayer) {
            NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
            Destroy(enemyNetworkObject.gameObject);
        //}
    }
}
