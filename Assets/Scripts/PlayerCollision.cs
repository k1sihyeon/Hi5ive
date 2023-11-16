using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;



public class PlayerCollision : NetworkBehaviour {

    public static PlayerCollision instance;

    static class Layer {
        public const short ENEMY_LAYER = 6;
        public const short PLAYER_LAYER = 8;
        public const short PLAYER_ULTIMATE_LAYER = 7;
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
        UpdateEnergyClientRpc(currentEnergy);
        UpdateUltSlider();

        ignoringCollisions = true;
        gameObject.layer = Layer.PLAYER_ULTIMATE_LAYER; //충돌처리 꺼진 레이어로 변경
        //gameObject.GetComponent<Collider>().enabled = false;

        PlayerController.instance.moveSpeed = 15f;
        UpdatePlayerSpeedClientRpc(15f);
        //instance문 멀티에서 동작 x => controller, collision 통합 필요

        Invoke("OffUltimate", 3);
    }

    void OffUltimate() {
        Debug.Log("Off Ultimate");
        gameObject.layer = Layer.PLAYER_LAYER; //원래 레이러로 복구
        //gameObject.GetComponent<Collider>().enabled = true;

        PlayerController.instance.moveSpeed = 5f;
        UpdatePlayerSpeedClientRpc(5f);

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

            if(collision.gameObject.CompareTag("Obstacle")) {
                energyIncreaseRate = 30f;
            }
            if (collision.gameObject.layer == LayerMask.NameToLayer("SpeedUp"))
            {
                if (IsLocalPlayer)
                {
                    Debug.Log("스피드업");
                    // 일단 속도를 10으로 변경
                    PlayerController.instance.moveSpeed = 7f;

                    // 3초 후에 다시 속도를 5로 변경하는 코루틴 시작
                    StartCoroutine(ResetSpeedAfterDelay(3f));

                    
                }
            }

            currentEnergy += energyIncreaseRate;
            UpdateUltSlider();
            UpdateEnergyClientRpc(currentEnergy);

            //Ultimate
            if (currentEnergy >= maxEnergy) {

                OnUltimate();

            }
        }
        else {
            if(collision.gameObject.layer == Layer.ENEMY_LAYER) {
                Debug.Log("returning");
                return;
            }
        }

    }

    IEnumerator ResetSpeedAfterDelay(float delay)
    {
        // delay 초 동안 대기
        yield return new WaitForSeconds(delay);
        // 대기 후에 속도를 5로 변경
        PlayerController.instance.moveSpeed = 5f;


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
        UpdateUltSlider();
    }

    [ClientRpc]
    private void UpdatePlayerSpeedClientRpc(float speed) {
        PlayerController.instance.moveSpeed = speed;
    }


}
