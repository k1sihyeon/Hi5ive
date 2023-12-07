using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class RandomEffect : NetworkBehaviour
{
    public GameObject Explosion_effect;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void effect(int result)
    {
        if (!IsServer)
        {
            return;
        }
        if (result < 0)
        {
            Effect_On_ClientRpc();
            StartCoroutine(Effect_corutine(3f));
        }

    }

    [ClientRpc]
    private void Effect_On_ClientRpc()
    {
        Explosion_effect.SetActive(true);
    }
    [ClientRpc]
    private void Effect_Off_ClientRpc()
    {
        Explosion_effect.SetActive(false);
    }

    private IEnumerator Effect_corutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        Effect_Off_ClientRpc();
    }
}
