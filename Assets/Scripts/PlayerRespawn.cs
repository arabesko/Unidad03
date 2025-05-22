using UnityEngine;
using System.Collections;

public class PlayerRespawn : MonoBehaviour
{
    public Transform respawnPoint;         // Punto donde reaparece el jugador
    public Animator fadeAnimator;          // Animator con las animaciones FadeIn y FadeOut
    public float fadeDuration = 1.5f;      // Duración total del fade (ajustalo a la duración real de tu animación)

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("DeathZone"))
        {
            StartCoroutine(FadeAndRespawn());
        }
    }

    private IEnumerator FadeAndRespawn()
    {
        // Iniciar FadeIn (pantalla oscura)
        fadeAnimator.Play("FadeIn");

        // Esperar duración del fade a negro
        yield return new WaitForSeconds(fadeDuration);

        // Teletransportar al jugador cuando ya está todo oscuro
        CharacterController cc = GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        transform.position = respawnPoint.position;

        if (cc != null) cc.enabled = true;

        // Iniciar FadeOut (pantalla vuelve a aclararse)
        fadeAnimator.Play("FadeOut");

        // Esperar hasta que termine el FadeOut (opcional)
        yield return new WaitForSeconds(fadeDuration);
    }
}
