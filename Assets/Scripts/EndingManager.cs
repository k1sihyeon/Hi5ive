using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndingManager : MonoBehaviour {

    [SerializeField] private TMP_Text rankText;
    private int rank;

    void Start() {
        rank = PlayerPrefs.GetInt("Rank");

        if (rank <= 0) {
            rankText.text = "완주 실패..";
        }
        else if (rank > 0) {
            rankText.text = $"{rank}등 입니다!!";
        }
       
        rankText.gameObject.SetActive(true);
    }

    void Update() {

    }
}
