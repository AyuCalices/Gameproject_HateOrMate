using System;
using System.Collections;
using Photon.Pun;
using UnityEngine;

public class SlashDamageAnimationBehaviour : MonoBehaviour, IPunInstantiateMagicCallback
{
    private bool _test;
    
    public SlashDamageAnimationBehaviour InstantiateDamageAnimation(Vector3 startPosition)
    {
        return PhotonNetwork.Instantiate("Slash", startPosition, Quaternion.identity).GetComponent<SlashDamageAnimationBehaviour>();
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return new WaitForSeconds(2f);

        PhotonNetwork.Destroy(gameObject);
    }
}
