using UnityEngine;

public class Flashlights : MonoBehaviour
{
    [SerializeField] private Light linternaIzquierda;
    [SerializeField] private Light linternaDerecha;

    [SerializeField] private AudioClip sonidoEncender;
    [SerializeField] private AudioClip sonidoApagar;

    private AudioSource audioSource;
    private bool lucesEncendidas;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        // Sincronizar estado de las luces al iniciar
        lucesEncendidas = linternaIzquierda.enabled || linternaDerecha.enabled;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lucesEncendidas = !lucesEncendidas;

            linternaIzquierda.enabled = lucesEncendidas;
            linternaDerecha.enabled = lucesEncendidas;

            if (lucesEncendidas)
                audioSource.PlayOneShot(sonidoEncender);
            else
                audioSource.PlayOneShot(sonidoApagar);
        }
    }
}
