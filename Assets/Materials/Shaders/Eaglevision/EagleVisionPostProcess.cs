using UnityEngine;

[ExecuteInEditMode]
public class EagleVisionPostProcess : MonoBehaviour
{
    [SerializeField] private Material visionMaterial;
    private Camera mainCamera;

    void Start()
    {
        mainCamera = GetComponent<Camera>();
        if (visionMaterial == null)
        {
            // Buscar automáticamente el material
            visionMaterial = new Material(Shader.Find("Hidden/EagleVisionEffect"));
        }

        // Configurar propiedades iniciales
        visionMaterial.SetFloat("_WaveRadius", -1);
        visionMaterial.SetFloat("_WaveFade", 0);
    }

    void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (visionMaterial != null)
        {
            // Actualizar posición del centro (jugador)
            visionMaterial.SetVector("_WaveCenter", transform.position);
            Graphics.Blit(source, destination, visionMaterial);
        }
        else
        {
            Graphics.Blit(source, destination);
        }
    }
}