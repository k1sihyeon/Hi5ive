using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class PlayerEndPoint : NetworkBehaviour {

    [SerializeField] private TMP_Text rankText;

    private Vector3 observePoint = new Vector3(15, 25, -207);
    public int rank = 0;

    private void OnCollisionEnter(Collision collision) {
        if (!IsServer)
            return;

        if (collision.gameObject.CompareTag("EndPoint")) {
            Debug.Log("End Point");
            SetPosition(observePoint);

            //��� ���
            //player ��� �Ҵ�
            //player ui�� ǥ��
        }
    }

    public void ActivateRankUI() {
        rankText.text = $"{rank}�� �Դϴ�!!";
        rankText.gameObject.SetActive(true);
    } 

    private void SetPosition(Vector3 point) {

        this.gameObject.transform.position = point;

        SetPositionClientRpc(point);

    }

    [ClientRpc]
    private void SetPositionClientRpc(Vector3 point) {
        Debug.Log("position client rpc");

        this.gameObject.transform.position = point;
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
