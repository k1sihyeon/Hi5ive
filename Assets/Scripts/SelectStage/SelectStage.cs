using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SelectStage : MonoBehaviour {
    [SerializeField] private Button stage1Btn;
    [SerializeField] private Button stage2Btn;
    [SerializeField] private Button stage3Btn;

    public void OnClickBtn1() {
        SceneManager.LoadScene("Netcode Testing");
    }

    public void OnClickBtn2() {
        SceneManager.LoadScene("Space_Map");
    }

    public void OnClickBtn3() {
        SceneManager.LoadScene("Columns");
    }

}
