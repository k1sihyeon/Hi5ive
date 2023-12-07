using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;



public class PlayerCollision : NetworkBehaviour {

    static class Layer {
        public const short PLAYER_LAYER = 6;
        public const short PLAYER_ULTIMATE_LAYER = 7;
        public const short OBSTACLE_LAYER = 8;
    }

    private PlayerController player;
    private Volume volume;
    private ChromaticAberration aberration;
    private Camera playerCamera;

    private float maxEnergy = 100f;
    private float energyIncreaseRate = 10f;
    [SerializeField] private float currentEnergy = 0f;
    private float collisionIgnoreTime = 5f;
    private bool ignoringCollisions = false;
    public GameObject Ultimate_effect;
    private int randombox_result;

    private void Start() {
        if (IsServer) {
            currentEnergy = 0f;
        }
        else {
            UpdateEnergyClientRpc(currentEnergy);
        }

        if (IsLocalPlayer) {

            player = GetComponent<PlayerController>();
            playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera != null) {
                volume = playerCamera.GetComponent<Volume>();

                if (volume != null) {
                    volume.profile.TryGet(out aberration);
                }
                else {
                    Debug.LogError("Volume ������Ʈ�� ã�� �� ����");
                }
            }
            else {
                Debug.LogError("�÷��̾� �ڽ� ī�޶� ã�� �� ����");
            }

        }


    }

    private void Update()
    {
        if(IsLocalPlayer)
        {
            //UpdatePlayerSpeedHostRpc(player.moveSpeed);
            UpdatePlayerSpeedClientRpc(player.moveSpeed);
        }
        
    }

    void UpdateUltSlider() {

        if(IsLocalPlayer) {
            UIController.instance.ultVal = currentEnergy / maxEnergy;
        }
        
    }

    void OnUltimate() {
        Debug.Log("On Ultimate");
        currentEnergy = 0f;
        UpdateEnergyClientRpc(currentEnergy);
        UpdateUltSlider();

        ignoringCollisions = true;
        gameObject.layer = Layer.PLAYER_ULTIMATE_LAYER; //�浹ó�� ���� ���̾�� ����

        PlayerController.instance.moveSpeed = 15f;
        UpdatePlayerSpeedClientRpc(15f);

        UpdateChromaticAberrationClientRpc(0.8f);

        Invoke("OffUltimate", 3);
    }

    void OffUltimate() {
        Debug.Log("Off Ultimate");
        gameObject.layer = Layer.PLAYER_LAYER; //���� ���̷��� ����

        PlayerController.instance.moveSpeed = 5f;
        UpdatePlayerSpeedClientRpc(5f);

        UpdateChromaticAberrationClientRpc(0f);

        ignoringCollisions = false;
    }

    void resetspeed()
    {
        PlayerController.instance.moveSpeed = 5f;
        UpdatePlayerSpeedClientRpc(5f);

    }

    private void NetworkDestroy(Collision collision) {
        ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        Destroy(collision.gameObject);
        CollisionClientRpc(enemyNetworkId);
    }

    private void NetworkChildDestroy(Collision collision)
    {
        // collision���� NetworkObject ������Ʈ ��������
    NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

    // NetworkObject�� null�� �ƴϸ� ��� ����
    if (networkObject != null) {
        // �θ� ������Ʈ�� Transform ��������
        Transform parentTransform = networkObject.transform;

        // �θ��� �ڽ� ������Ʈ �߿��� "Cube"�� ���� ������Ʈ ã��
        Transform childTransform = parentTransform.Find("Cube");

        // ã�� �ڽ� ������Ʈ�� �ִٸ� ��Ȱ��ȭ
        if (childTransform != null) {
            childTransform.gameObject.SetActive(false);

            // ���⼭ ���ϴ� �۾� ����
            // ...

            // RPC ȣ��
            CollisionClientRpc(networkObject.NetworkObjectId);
        } else {
            Debug.LogWarning("Cube �ڽ� ������Ʈ�� ã�� �� �����ϴ�.");
        }
    }
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

            if (collision.gameObject.CompareTag("randombox"))
            {
                randombox_result = Random.Range(-5, 0);
                RandomEffect boxeffect = collision.gameObject.GetComponent<RandomEffect>();
                boxeffect.effect(randombox_result);
                PlayerController.instance.moveSpeed = randombox_result+7;
                UpdatePlayerSpeedClientRpc(randombox_result+7);
                Invoke("resetspeed", 3);
                NetworkDestroy(collision);
            }

            currentEnergy += energyIncreaseRate;
            UpdateUltSlider();
            UpdateEnergyClientRpc(currentEnergy);

            //Ultimate
            if (currentEnergy >= maxEnergy) {
                OnUltimate();
                Ultimate_effectOnClientRpc();
                StartCoroutine(Ultimate_corutine(3f));

                

            }
            
        }
        else {
            if(collision.gameObject.layer == Layer.OBSTACLE_LAYER) {
                Debug.Log("returning");
                return;
            }
        }

    }

    [ClientRpc]
    private void UpdateChromaticAberrationClientRpc(float value) {

        if(IsLocalPlayer) {
            aberration.intensity.value = value;
        }

    }


    /* [ServerRpc]
     private void Ultimate_effectOnServerRpc()
     {
         Ultimate_effect.SetActive(true);
         Ultimate_effectOnClientRpc();

     }*/

    /*[ServerRpc]
    private void Ultimate_effectOffServerRpc()
    {
        Ultimate_effect.SetActive(false);
        Ultimate_effectOffClientRpc();

    }*/

    [ClientRpc]
    private void Ultimate_effectOnClientRpc()
    {
        Ultimate_effect.SetActive(true);
    }
    [ClientRpc]
    private void Ultimate_effectOffClientRpc()
    {
        Ultimate_effect.SetActive(false);

    }
    private IEnumerator Ultimate_corutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Ultimate_effectOffClientRpc();

    }


    private IEnumerator DelayedResetSpeed(float delay)
    {
        yield return new WaitForSeconds(delay);
        // ��� �Ŀ� �ӵ��� 5�� ����
        PlayerController.instance.moveSpeed = 5f;
        player.moveSpeed = 5f;
    }

    [ClientRpc]
    private void CollisionClientRpc(ulong enemyNetworkId) {

        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
        Destroy(enemyNetworkObject.gameObject);

    }

    [ClientRpc]
    private void CollisionChildClientRpc(ulong enemyNetworkId)
    {

        NetworkObject enemyNetworkObject = NetworkManager.Singleton.SpawnManager.SpawnedObjects[enemyNetworkId];
        Transform childtransform = enemyNetworkObject.transform.Find("Cube");
        childtransform.gameObject.SetActive(false);

    }
    [ClientRpc]
    private void UpdateEnergyClientRpc(float energy) {
        currentEnergy = energy;
        UpdateUltSlider();
    }

    [ClientRpc]
    private void UpdatePlayerSpeedClientRpc(float speed) {
        player = GetComponent<PlayerController>();
        player.moveSpeed = speed;
    }
    

}
