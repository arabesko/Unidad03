using UnityEngine;

public class EagleVision : MonoBehaviour
{
    [SerializeField] private float waveSpeed = 25f;
    [SerializeField] private float maxRadius = 150f;
    [SerializeField] private Material visionMaterial;

    private float currentRadius;
    private bool isActive;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            ActivateVision();
        }

        if (isActive)
        {
            currentRadius += waveSpeed * Time.deltaTime;
            visionMaterial.SetVector("_WaveCenter", transform.position);
            visionMaterial.SetFloat("_WaveRadius", currentRadius);

            if (currentRadius > maxRadius)
                isActive = false;
        }
    }

    void ActivateVision()
    {
        isActive = true;
        currentRadius = 0.1f;
        visionMaterial.SetFloat("_WaveWidth", 3f); // Ancho de la onda
    }
}