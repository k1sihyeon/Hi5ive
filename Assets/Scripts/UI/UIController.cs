using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {

    [SerializeField] private Slider UltimateBarSlider;
    [SerializeField] private TMP_Text rankText;

    public static UIController instance;
    public float ultVal = 0f;

    private void Awake() {
        if (UIController.instance == null) {
            UIController.instance = this;
        }
    }

    void Start() {

    }

    void Update() {
        UltimateBarSlider.value = ultVal;
    }

    public void UpdateRankUI(int val) {
        
        rankText.text = $"{val}등 입니다!!";
        rankText.gameObject.SetActive(true);

    }

    void UpdateEnergyUI(float value) {
        ultVal = value;
    }
}
