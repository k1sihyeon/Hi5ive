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

    public bool real_cooldown = false;
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

    //public Image attacked;

    public AudioSource PlusSound;//사운드저장공간
    public AudioClip PlusSoundSource;//실제사운드

    public GameObject player1;
    public AudioSource AudioSource;
    public AudioClip AudioClip;
    public Transform cameraTransform;
    private Vector3 originalPosition;
    private bool isShaking = false;
    float duration = 1;
    float intensity = 1;

    private void Start() {
        if (IsServer) {
            currentEnergy = 0f;
        }
        else {
            UpdateEnergyClientRpc(currentEnergy);
        }

        if (IsLocalPlayer) {
            PlusSound.clip = PlusSoundSource;
            AudioSource.clip = AudioClip;
            player = GetComponent<PlayerController>();
            playerCamera = GetComponentInChildren<Camera>();

            if (playerCamera != null) {
                volume = playerCamera.GetComponent<Volume>();

                if (volume != null) {
                    volume.profile.TryGet(out aberration);
                }
                else {
                    Debug.LogError("Volume 컴포넌트를 찾을 수 없음");
                }
            }
            else {
                Debug.LogError("플레이어 자식 카메라를 찾을 수 없음");
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

    [ServerRpc]
    private void SendCoolDownServerRpc()
    {
        real_cooldown = true;
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
        gameObject.layer = Layer.PLAYER_ULTIMATE_LAYER; //충돌처리 꺼진 레이어로 변경

        ultimateClientRpc();
        
        /*PlayerController.instance.moveSpeed = 15f;
        UpdatePlayerSpeedClientRpc(15f);

        UpdateChromaticAberrationClientRpc(0.8f);*/

        Invoke("OffUltimate", 3);
    }

    void OffUltimate() {
        Debug.Log("Off Ultimate");
        gameObject.layer = Layer.PLAYER_LAYER; //원래 레이러로 복구

        ultimateOffClientRpc();
        /*PlayerController.instance.moveSpeed = 7f;
        UpdatePlayerSpeedClientRpc(7f);*/

        /*UpdateChromaticAberrationClientRpc(0f);*/

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

    private void NetworkDestroy_Trigger(Collider collision)
    {
        ulong enemyNetworkId = collision.gameObject.GetComponent<NetworkObject>().NetworkObjectId;
        Destroy(collision.gameObject);
        CollisionClientRpc(enemyNetworkId);
    }

    private void NetworkChildDestroy(Collision collision)
    {
        // collision에서 NetworkObject 컴포넌트 가져오기
        NetworkObject networkObject = collision.gameObject.GetComponent<NetworkObject>();

        // NetworkObject가 null이 아니면 계속 진행
        if (networkObject != null)
        {
            // 부모 오브젝트의 Transform 가져오기
            Transform parentTransform = networkObject.transform;

            // 부모의 자식 오브젝트 중에서 "Cube"를 가진 오브젝트 찾기
            Transform childTransform = parentTransform.Find("Cube");

            // 찾은 자식 오브젝트가 있다면 비활성화
            if (childTransform != null)
            {
                childTransform.gameObject.SetActive(false);

                // 여기서 원하는 작업 수행
                // ...

                // RPC 호출
                CollisionClientRpc(networkObject.NetworkObjectId);
            }
            else
            {
                Debug.LogWarning("Cube 자식 오브젝트를 찾을 수 없습니다.");
            }
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer) return;

        if (!ignoringCollisions) {

            energyIncreaseRate = 0f;

            if (!real_cooldown)
            {
                /*if (collision.gameObject.CompareTag("Enemy"))
                {
                    player.speed_up_count++;
                    player.attack_cooldown = true;
                    StartCoroutine(Reset_attack_cooldown(3f));
                    ShakeCameraClientRpc();
                    energyIncreaseRate = 50f;
                    NetworkDestroy(collision);
                }

                if (collision.gameObject.CompareTag("BallObstacle"))
                {
                    player.speed_up_count++;
                    player.attack_cooldown = true;
                    StartCoroutine(Reset_attack_cooldown(3f));
                    ShakeCameraClientRpc();
                    energyIncreaseRate = 100f;
                    NetworkDestroy(collision);
                }*/

                if (collision.gameObject.CompareTag("Obstacle"))
                {
                    real_cooldown = true;
                    StartCoroutine(Resetrealcooldown(3f));
                    abcClientRpc();
                    ShakeCameraClientRpc();
                    energyIncreaseRate = 15f;
                    if (currentEnergy < 100)
                    {
                        aabcClientRpc();
                    }


                }
            }
            

            

            currentEnergy += energyIncreaseRate;
            UpdateUltSlider();
            UpdateEnergyClientRpc(currentEnergy);

            //Ultimate
            if (currentEnergy >= maxEnergy) {
                aaabcClientRpc();
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
    private void ultimateClientRpc()
    {
        if (IsLocalPlayer)
        {
            player.CanMove = true;
            player.moveSpeed = 15f;
            UpdateChromaticAberrationClientRpc(0.8f);
        }
    }
    [ClientRpc]
    private void ultimateOffClientRpc()
    {
        if (IsLocalPlayer)
        {
            player.CanMove = true;
            player.moveSpeed = 7f;
            UpdateChromaticAberrationClientRpc(0f);
        }
    }

    [ClientRpc]
    private void abcClientRpc()
    {
        if(IsLocalPlayer)
        {
            player.speed_up_count++;
            player.attack_cooldown = true;
            StartCoroutine(Reset_attack_cooldown(3f));
        }
        
    }

    [ClientRpc]
    private void aabcClientRpc()
    {
        if (IsLocalPlayer)
        {
            player.moveSpeed = 0;
            player.CanMove = false;
            StartCoroutine(DelayedResetSpeedRpc(1f));
            StartCoroutine(ResetCanMove(1f));
        }
        
    }

    [ClientRpc]
    private void aaabcClientRpc()
    {
        if(IsLocalPlayer)
        {
            player.speed_up_count++;
            StartCoroutine(speed_up_count_minus(3f));
            player.CanMove = true;
        }
        
    }

    private IEnumerator speed_up_count_minus(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.speed_up_count--;
    }
    /*private void OnTriggerEnter(Collider collision)
    {
        randombox_result = Random.Range(0, 2);
        if(IsLocalPlayer)
        {
            if (randombox_result > 0)
            {
                PlusSound.Play();
            }
            
        }
        if (!ignoringCollisions)
            {
                if (collision.gameObject.CompareTag("randombox"))
                {

                    if (randombox_result == 0)
                    {
                        randombox_result = -4;
                    }
                    else if (randombox_result == 1)
                    {
                        randombox_result = 4;
                    }
                    RandomEffect boxeffect = collision.gameObject.GetComponent<RandomEffect>();
                    boxeffect.effect(randombox_result);
                    PlayerController.instance.moveSpeed = randombox_result + 5;
                    //UpdatePlayerSpeedClientRpc(randombox_result + 5);
                    Invoke("resetspeed", 3);
                    NetworkDestroy_Trigger(collision);
                }
            }
        
        
           
    }*/


    private IEnumerator ResetCanMove(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.CanMove = true;
    }

    private IEnumerator Resetrealcooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        real_cooldown = false;
    }

    private IEnumerator Reset_attack_cooldown(float delay)
    {
        yield return new WaitForSeconds(delay);
        player.attack_cooldown = false;
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
        // 대기 후에 속도를 5로 변경
        PlayerController.instance.moveSpeed = 5f;
        player.moveSpeed = 7f;
    }


    private IEnumerator DelayedResetSpeedRpc(float delay)
    {
        yield return new WaitForSeconds(delay);
        // 대기 후에 속도를 5로 변경
        player.speed_up_count--;
        if(player.speed_up_count==0)
        {
            Debug.Log("리셋");
            player.moveSpeed = 7f;
        }

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

    [ClientRpc]
    private void ShakeCameraClientRpc()
    {
        if (IsLocalPlayer)
        {
            AudioSource.Play();
            StartCoroutine(ShakeEffect(duration, intensity));
        }
    }

    private IEnumerator ShakeEffect(float duration, float intensity)
    {
        isShaking = true;
        originalPosition = cameraTransform.localPosition;
        float elapsed = 0f;
        //attacked.enabled = true;
        


        while (elapsed < duration - 0.8)
        {
            float x = Random.Range(-0.1f, 0.1f) * intensity;
            float y = Random.Range(-0.1f, 0.1f) * intensity;

            cameraTransform.localPosition = new Vector3(originalPosition.x + x, originalPosition.y + y, originalPosition.z);

            elapsed += Time.deltaTime;
            yield return null;
        }
        //attacked.enabled = false;
        cameraTransform.localPosition = originalPosition;
        isShaking = false;

    }


}
