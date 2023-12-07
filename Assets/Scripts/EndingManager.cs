using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingManager : MonoBehaviour {

    [SerializeField] private TMP_Text rankText;
    private int rank;

    void Start() {
        rank = PlayerPrefs.GetInt("Rank");
        rankText.text = $"{rank}�� �Դϴ�!!";
        rankText.gameObject.SetActive(true);
    }

    void Update() {

    }
}
