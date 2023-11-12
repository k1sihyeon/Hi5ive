using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerCollision : NetworkBehaviour {

    private float maxEnergy = 100f;
    private float energyIncreaseRate = 10f;
    private float energyResetThreshold = 50f;
    private float currentEnergy = 0f;

    private float collisionIgnoreTime = 5f;
    private bool ignoringCollisions = false;

    private void Ultimate() {
        Debug.Log("Ultimate");
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.AddForce(Vector3.up * 10f, ForceMode.Impulse);
    }

    private void Start() {
        if (IsServer) {
            currentEnergy = 0f;
            ignoringCollisions = false;
        }
        else {
            // 에너지 및 충돌 무시 상태를 클라이언트로부터 동기화
            UpdateEnergyClientRpc(currentEnergy);
            UpdateCollisionIgnoreStatusClientRpc(ignoringCollisions);
        }
    }


    private void NetworkDestroy(Collision collision) {
        ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        Destroy(collision.gameObject);
        CollisionClientRpc(enemyNetworkId);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer) return;

        if (!ignoringCollisions) {

            if (collision.gameObject.CompareTag("Enemy")) {
                energyIncreaseRate = 50f;
                NetworkDestroy(collision);
            }

            if (collision.gameObject.CompareTag("BallObstacle")) {
                energyIncreaseRate = 100f;
                NetworkDestroy(collision);
            }

            currentEnergy += energyIncreaseRate;

            //Ultimate
            if (currentEnergy >= energyResetThreshold) {

                Debug.Log("Ultimate");

                ignoringCollisions = true;
                StartCoroutine(ResetCollisionIgnore());
                currentEnergy = 0f;
            }

        }

    }

    private IEnumerator ResetCollisionIgnore() {
        yield return new WaitForSeconds(collisionIgnoreTime);
        ignoringCollisions = false;
    }


    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId) {
        //if(!IsLocalPlayer) {
            NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
            Destroy(enemyNetworkObject.gameObject);
        //}
    }

    [ClientRpc]
    private void UpdateEnergyClientRpc(float energy) {
        currentEnergy = energy;
    }

    [ClientRpc]
    private void UpdateCollisionIgnoreStatusClientRpc(bool ignoreStatus) {
        ignoringCollisions = ignoreStatus;
    }
}
