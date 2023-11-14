using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class PlayerCollision : NetworkBehaviour {

    public static PlayerCollision instance;

    static class define {
        public const short ENEMY_LAYER = 6;
        public const short PLAYER_LAYER = 7;
        public const short PLAYER_ULTIMATE_LAYER = 8;
    }

    

    private float maxEnergy = 100f;
    private float energyIncreaseRate = 10f;
    private float currentEnergy = 0f;

    private float collisionIgnoreTime = 5f;
    private bool ignoringCollisions = false;

    private void Awake() {
        if (PlayerCollision.instance == null) {
            PlayerCollision.instance = this;
        }
    }

    private void Start() {
        if (IsServer) {
            currentEnergy = 0f;

        }
        else {
            
            UpdateEnergyClientRpc(currentEnergy);
            
        }
    }

    void UpdateUltSlider() {

        UISliderController.instance.val = currentEnergy / maxEnergy;

    }

    void OnUltimate() {
        Debug.Log("On Ultimate");
        currentEnergy = 0f;
        UpdateUltSlider();

        ignoringCollisions = true;
        gameObject.layer = define.PLAYER_ULTIMATE_LAYER; //충돌처리 꺼진 레이어로 변경
        //gameObject.GetComponent<Collider>().enabled = false;

        PlayerController.instance.moveSpeed = 15f;
        //instance문 멀티에서 동작 x => controller, collision 통합 필요

        Invoke("OffUltimate", 3);
    }

    void OffUltimate() {
        Debug.Log("Off Ultimate");
        gameObject.layer = define.PLAYER_LAYER; //원래 레이러로 복구
        //gameObject.GetComponent<Collider>().enabled = true;

        PlayerController.instance.moveSpeed = 5f;

        ignoringCollisions = false;
    }


    private void NetworkDestroy(Collision collision) {
        ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        Destroy(collision.gameObject);
        CollisionClientRpc(enemyNetworkId);
    }

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer) return;

        if (!ignoringCollisions) {

            energyIncreaseRate = 0f;

            if (collision.gameObject.CompareTag("Enemy")) {
                energyIncreaseRate = 50f;
                NetworkDestroy(collision);
            }

            if (collision.gameObject.CompareTag("BallObstacle")) {
                energyIncreaseRate = 100f;
                NetworkDestroy(collision);
            }

            currentEnergy += energyIncreaseRate;
            UpdateUltSlider();

            //Ultimate
            if (currentEnergy >= maxEnergy) {

                OnUltimate();

            }
        }
        else {
            if(collision.gameObject.layer == define.ENEMY_LAYER) {
                Debug.Log("returning");
                return;
            }
        }

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


}
