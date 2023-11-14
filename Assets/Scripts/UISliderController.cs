using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISliderController : MonoBehaviour {

    private Slider UltimateBarSlider;

    public static UISliderController instance;
    public float val = 0f;

    private void Awake() {
        if (UISliderController.instance == null) {
            UISliderController.instance = this;
        }
    }

    // Start is called before the first frame update
    void Start() {
        UltimateBarSlider = GetComponent<Slider>();
    }

    // Update is called once per frame
    void Update() {
        UltimateBarSlider.value = val;
    }
}
