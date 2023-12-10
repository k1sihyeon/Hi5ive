using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class earlyDestroy : MonoBehaviour
{
    public GameObject box;
    public GameObject guess;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        
        box.SetActive(false);
        guess.SetActive(false);
    }
}
