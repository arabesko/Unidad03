using UnityEngine;

public class EagleVision : MonoBehaviour
{
    public enum WaveState { Inactive, Expanding, Fading }

    [Header("Configuración")]
    [SerializeField] private float waveSpeed = 25f;
    [SerializeField] private float maxRadius = 150f;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private Color waveColor = new Color(0, 0.8f, 1f, 0.7f);
    [SerializeField] private float waveWidth = 10f;

    [Header("Referencias")]
    [SerializeField] private Material visionMaterial; // Material del shader de post-processing

    [Header("Debug")]
    [SerializeField] private WaveState currentState = WaveState.Inactive;

    private float currentRadius;
    private float fadeTimer;

    void Start()
    {
        if (visionMaterial != null)
        {
            visionMaterial.SetColor("_WaveColor", waveColor);
            visionMaterial.SetFloat("_WaveWidth", waveWidth);
        }
    }

    void Update()
    {
        if (visionMaterial == null) return;

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

        visionMaterial.SetFloat("_WaveFade", 1f);
    }

    void UpdateExpansion()
    {
        currentRadius += waveSpeed * Time.deltaTime;
        visionMaterial.SetFloat("_WaveRadius", currentRadius);

        if (currentRadius >= maxRadius)
        {
            currentState = WaveState.Fading;
            fadeTimer = fadeDuration;
        }
    }

    void UpdateFading()
    {
        fadeTimer -= Time.deltaTime;
        float fadeValue = Mathf.Clamp01(fadeTimer / fadeDuration);
        visionMaterial.SetFloat("_WaveFade", fadeValue);

        if (fadeTimer <= 0)
        {
            currentState = WaveState.Inactive;
            visionMaterial.SetFloat("_WaveRadius", -1);
        }
    }

    void OnDestroy()
    {
        // Resetear valores al salir
        if (visionMaterial != null)
        {
            visionMaterial.SetFloat("_WaveRadius", -1);
            visionMaterial.SetFloat("_WaveFade", 0);
        }
    }
}