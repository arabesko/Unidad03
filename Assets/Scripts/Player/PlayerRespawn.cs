using UnityEngine;
using System.Collections;
using Cinemachine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;         // Punto donde reaparece el jugador
    public Animator fadeAnimator;          // Animator con las animaciones FadeIn y FadeOut
    public float fadeToBlackTime = 1f;     // Tiempo hasta que la pantalla esté completamente negra
    public float blackScreenDuration = 0.5f; // Tiempo que permanece negra antes de FadeOut
    public float fadeFromBlackTime = 1f;   // Tiempo que dura el FadeOut
   

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            StartCoroutine(FadeAndRespawn());
    
        }
    }

    private IEnumerator FadeAndRespawn()
    {
        // Iniciar FadeIn (oscurecer pantalla)
        fadeAnimator.Play("FadeIn");

        // Esperar a que la pantalla se oscurezca completamente
        yield return new WaitForSeconds(fadeToBlackTime);

        // Teletransportar al jugador cuando la pantalla ya está completamente negra
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = respawnPoint.position;

        if (cc != null) cc.enabled = true;

        // Esperar un momento en negro (opcional)
        yield return new WaitForSeconds(blackScreenDuration);

        // Iniciar FadeOut (volver a mostrar la pantalla)
        fadeAnimator.Play("FadeOut");

        // Esperar a que se complete el FadeOut (opcional)
        yield return new WaitForSeconds(fadeFromBlackTime);
    }
}
