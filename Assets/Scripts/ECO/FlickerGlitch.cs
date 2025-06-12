using UnityEngine;
using System.Collections;

public class FlickerGlitch : MonoBehaviour
{
    [Header("Target Objects")]
    public GameObject target;          // Objeto principal a encender/apagar
    public GameObject lightObject;     // GameObject que contiene la luz

    [Header("Audio Clips")]
    public AudioClip glitchSound;      // Sonido al comenzar y al apagar
    public AudioClip onSound;          // Sonido cuando está encendido

    [Header("Trigger Key & Cooldown")]
    public KeyCode triggerKey = KeyCode.E;
    public float keyCooldown = 3f;

    [Header("Flicker Settings")]
    public float flickerDurationOn = 0.5f;
    public int flickerCountOn = 6;
    public float onDuration = 2f;
    public float flickerDurationOff = 0.3f;
    public int flickerCountOff = 4;

    private bool isRunning = false;
    private float nextAvailableTime = 0f;
    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        // La luz siempre apunta al target
        if (lightObject != null && target != null)
        {
            lightObject.transform.LookAt(target.transform.position);
        }

        // Disparo de la acción con cooldown
        if (Input.GetKeyDown(triggerKey) && Time.time >= nextAvailableTime && !isRunning)
        {
            StartCoroutine(ToggleFlickerWithCooldown());
        }
    }

    IEnumerator ToggleFlickerWithCooldown()
    {
        isRunning = true;
        nextAvailableTime = Time.time + keyCooldown;

        // Sonido de glitch inicial
        if (glitchSound != null)
            audioSource.PlayOneShot(glitchSound);

        // Flicker al encender
        yield return StartCoroutine(Flicker(true, flickerCountOn, flickerDurationOn));
        SetState(true);

        // Sonido al encendido
        if (onSound != null)
            audioSource.PlayOneShot(onSound);

        // Permanecer encendido
        yield return new WaitForSeconds(onDuration);

        // Sonido de glitch antes de apagar
        if (glitchSound != null)
            audioSource.PlayOneShot(glitchSound);

        // Flicker al apagar
        yield return StartCoroutine(Flicker(false, flickerCountOff, flickerDurationOff));
        SetState(false);

        isRunning = false;
    }

    IEnumerator Flicker(bool finalState, int count, float duration)
    {
        float interval = duration / (count * 2f);
        for (int i = 0; i < count * 2; i++)
        {
            bool state = (i % 2 == 0) ? finalState : !finalState;
            SetState(state);
            yield return new WaitForSeconds(interval);
        }
        SetState(finalState);
    }

    void SetState(bool on)
    {
        if (target != null)
            target.SetActive(on);
        if (lightObject != null)
            lightObject.SetActive(on);
    }
}
