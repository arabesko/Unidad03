using UnityEngine;

public class EagleVision : MonoBehaviour
{
    public enum WaveState { Inactive, Expanding, Fading }

    [Header("Configuración")]
    [SerializeField] private float waveSpeed = 25f;
    [SerializeField] private float maxRadius = 150f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Material visionMaterial;

    [Header("Debug")]
    [SerializeField] private WaveState currentState = WaveState.Inactive;

    private float currentRadius;
    private float fadeTimer;
    private Vector3 waveOrigin;

    void Update()
    {
        switch (currentState)
        {
            case WaveState.Inactive:
                if (Input.GetKeyDown(KeyCode.V)) ActivateVision();
                break;

            case WaveState.Expanding:
                UpdateExpansion();
                break;

            case WaveState.Fading:
                UpdateFading();
                break;
        }
    }

    void ActivateVision()
    {
        currentState = WaveState.Expanding;
        currentRadius = 0.1f;
        fadeTimer = 0f;
        waveOrigin = transform.position;

        visionMaterial.SetVector("_WaveCenter", waveOrigin);
        visionMaterial.SetFloat("_WaveFade", 1f); // Totalmente visible
    }

    void UpdateExpansion()
    {
        currentRadius += waveSpeed * Time.deltaTime;
        visionMaterial.SetFloat("_WaveRadius", currentRadius);

        // Cambiar a estado de desvanecimiento al alcanzar el radio máximo
        if (currentRadius >= maxRadius)
        {
            currentState = WaveState.Fading;
            fadeTimer = fadeDuration;
        }
    }

    void UpdateFading()
    {
        fadeTimer -= Time.deltaTime;

        // Calcular fade (1 → 0)
        float fadeValue = Mathf.Clamp01(fadeTimer / fadeDuration);
        visionMaterial.SetFloat("_WaveFade", fadeValue);

        // Resetear al completar el desvanecimiento
        if (fadeTimer <= 0)
        {
            currentState = WaveState.Inactive;
            visionMaterial.SetFloat("_WaveRadius", -1); // Ocultar onda
        }
    }
}