using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;
    public ScreenFader screenFader; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            StartCoroutine(FadeAndRespawn());
        }
    }

    private IEnumerator FadeAndRespawn()
    {
        yield return screenFader.FadeOutIn(() =>
        {
            // Esto se ejecuta justo cuando la pantalla está negra
            CharacterController cc = GetComponent<CharacterController>();
            if (cc != null) cc.enabled = false;
            transform.position = respawnPoint.position;
            if (cc != null) cc.enabled = true;
        });
    }
}
