using UnityEngine;
using System.Collections;

public class FlickerGlitch : MonoBehaviour
{
    [Header("Targets Sequence")]
    public GameObject[] targets;            // Array de GameObjects a mostrar en secuencia
    public GameObject lightObject;          // GameObject que contiene la luz proyectora
    public ParticleSystem particleSystem;   // Sistema de partículas a mostrar durante la secuencia

    [Header("Audio Clips")]
    public AudioClip glitchSound;           // Sonido al iniciar y al apagar cada uno
    public AudioClip onSound;               // Sonido cuando cada uno se mantiene encendido (loopable)

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
    private int currentIdx = -1;

    void Awake()
    {
        // Obtener o crear AudioSource para reproducir clips
        audioSource = GetComponent<AudioSource>() ?? gameObject.AddComponent<AudioSource>();
        audioSource.loop = false;

        // Apagar todos los targets, la luz y las partículas al iniciar
        if (targets != null)
        {
            foreach (var go in targets)
                if (go) go.SetActive(false);
        }
        if (lightObject)
            lightObject.SetActive(false);
        if (particleSystem)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
    }

    void Update()
    {
        // Hacer que la luz siga al target actual
        if (lightObject != null && isRunning && currentIdx >= 0 && currentIdx < targets.Length)
        {
            var tgt = targets[currentIdx];
            if (tgt != null)
                lightObject.transform.LookAt(tgt.transform.position);
        }

        // Disparo de la acción con cooldown
        if (Input.GetKeyDown(triggerKey) && Time.time >= nextAvailableTime && !isRunning)
        {
            StartCoroutine(RunSequence());
        }
    }

    IEnumerator RunSequence()
    {
        isRunning = true;
        nextAvailableTime = Time.time + keyCooldown;

        // Iniciar partículas
        if (particleSystem)
            particleSystem.Play();

        for (int i = 0; i < targets.Length; i++)
        {
            currentIdx = i;
            var go = targets[i];
            if (go == null) continue;

            // Detener cualquier sonido previo
            audioSource.Stop();

            // Sonido de glitch inicial
            if (glitchSound != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(glitchSound);
            }

            // Flicker encendiendo este GO y luz
            yield return StartCoroutine(Flicker(go, lightObject, true, flickerCountOn, flickerDurationOn));

            // Estado encendido
            SetState(go, true);
            if (lightObject) SetState(lightObject, true);

            // Sonido al quedar encendido (loop)
            if (onSound != null)
            {
                audioSource.clip = onSound;
                audioSource.loop = true;
                audioSource.Play();
            }

            // Mantener encendido
            yield return new WaitForSeconds(onDuration);

            // Sonido de glitch antes de apagar
            if (glitchSound != null)
            {
                audioSource.loop = false;
                audioSource.PlayOneShot(glitchSound);
            }

            // Flicker apagando este GO y luz
            yield return StartCoroutine(Flicker(go, lightObject, false, flickerCountOff, flickerDurationOff));

            SetState(go, false);
            if (lightObject) SetState(lightObject, false);

            // Asegurar que el loop de onSound se detenga
            audioSource.Stop();
        }

        // Finalizar partículas y secuencia
        if (particleSystem)
            particleSystem.Stop(true, ParticleSystemStopBehavior.StopEmitting);

        currentIdx = -1;
        isRunning = false;
    }

    IEnumerator Flicker(GameObject go, GameObject lightGo, bool finalState, int count, float duration)
    {
        float interval = duration / (count * 2f);
        for (int i = 0; i < count * 2; i++)
        {
            bool state = (i % 2 == 0) ? finalState : !finalState;
            SetState(go, state);
            if (lightGo) SetState(lightGo, state);
            yield return new WaitForSeconds(interval);
        }
        SetState(go, finalState);
        if (lightGo) SetState(lightGo, finalState);
    }

    void SetState(GameObject go, bool on)
    {
        if (go) go.SetActive(on);
    }
}
