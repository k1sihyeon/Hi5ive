using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class PlayerSkinController : NetworkBehaviour {

    //Renderer[] characterMaterials;
    SkinnedMeshRenderer[] characterMaterials;

    public Texture2D[] albedoList;
    public Color[] eyeColors;
    private int rand;

    void Start() {

        if (IsLocalPlayer) {
            characterMaterials = GetComponentsInChildren<SkinnedMeshRenderer>();
            rand = Random.Range(0, 4);
            Debug.Log("rand: " + rand);
            ChangeMaterial(rand);

            ChangeMaterialServerRpc(rand);
        }
    }

    void ChangeMaterial(int index) {
        characterMaterials = GetComponentsInChildren<SkinnedMeshRenderer>();

        for (int i = 0; i < characterMaterials.Length; i++) {

            if (characterMaterials[i].transform.CompareTag("PlayerEyes")) {
                characterMaterials[i].material.SetColor("_EmissionColor", eyeColors[index]);
            }
            else {
                characterMaterials[i].material.SetTexture("_BaseMap", albedoList[index]);
            }
        }
    }

    [ServerRpc]
    void ChangeMaterialServerRpc(int index) {
        ChangeMaterial(index);
        ChangeMaterialClientRpc(index);
    }

    [ClientRpc]
    void ChangeMaterialClientRpc(int index) {
        ChangeMaterial(index);
    }

    void Update() {

    }
}
