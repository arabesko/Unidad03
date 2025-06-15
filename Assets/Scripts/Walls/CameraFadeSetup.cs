using System.Collections.Generic;
using UnityEngine;

public class CameraFadeSetup : MonoBehaviour
{
    [Header("Configuración")]
    public Transform target;
    public LayerMask fadeLayer;
    [Range(0.1f, 0.9f)]
    public float targetAlpha = 0.3f;
    public float fadeSpeed = 5f;

    private Transform camTransform;
    private Dictionary<Renderer, FadingObject> fadingObjects = new Dictionary<Renderer, FadingObject>();
    private List<Renderer> toRemove = new List<Renderer>();

    private class FadingObject
    {
        public Material[] OriginalMaterials;
        public Material[] FadeMaterials;
        public float CurrentAlpha;
        public bool IsFading;
    }

    void Start()
    {
        camTransform = Camera.main.transform;
    }

    void Update()
    {
        HandleFade();
    }

    private void HandleFade()
    {
        if (target == null || camTransform == null) return;

        Vector3 dir = target.position - camTransform.position;
        float distance = dir.magnitude;

        // Resetear estado de todos los objetos
        foreach (var fo in fadingObjects.Values)
        {
            fo.IsFading = false;
        }

        // Detectar objetos en el camino
        RaycastHit[] hits = Physics.RaycastAll(
            camTransform.position,
            dir.normalized,
            distance,
            fadeLayer
        );

        // Procesar objetos detectados
        foreach (RaycastHit hit in hits)
        {
            Renderer renderer = hit.collider.GetComponent<Renderer>();
            if (renderer == null) continue;

            if (!fadingObjects.ContainsKey(renderer))
            {
                CreateFadingObject(renderer);
            }

            fadingObjects[renderer].IsFading = true;
        }

        // Actualizar transparencia y manejar restauración
        toRemove.Clear();
        foreach (var kvp in fadingObjects)
        {
            Renderer renderer = kvp.Key;
            FadingObject fo = kvp.Value;

            // Si el objeto fue destruido
            if (renderer == null)
            {
                toRemove.Add(renderer);
                continue;
            }

            float targetValue = fo.IsFading ? targetAlpha : 1f;
            fo.CurrentAlpha = Mathf.MoveTowards(
                fo.CurrentAlpha,
                targetValue,
                fadeSpeed * Time.deltaTime
            );

            ApplyAlpha(renderer, fo, fo.CurrentAlpha);

            // Marcar para remover si completamente restaurado
            if (!fo.IsFading && fo.CurrentAlpha >= 1f - Mathf.Epsilon)
            {
                // Solo restaurar materiales si el objeto sigue existiendo
                renderer.sharedMaterials = fo.OriginalMaterials;
                toRemove.Add(renderer);
            }
        }

        // Limpiar objetos restaurados
        foreach (Renderer renderer in toRemove)
        {
            if (fadingObjects.TryGetValue(renderer, out FadingObject fo))
            {
                // Destruir materiales temporales
                if (fo.FadeMaterials != null)
                {
                    foreach (Material mat in fo.FadeMaterials)
                    {
                        if (mat != null) Destroy(mat);
                    }
                }
                fadingObjects.Remove(renderer);
            }
        }
    }

    private void CreateFadingObject(Renderer renderer)
    {
        FadingObject fo = new FadingObject
        {
            OriginalMaterials = renderer.sharedMaterials,
            FadeMaterials = new Material[renderer.sharedMaterials.Length],
            CurrentAlpha = 1f,
            IsFading = true
        };

        for (int i = 0; i < renderer.sharedMaterials.Length; i++)
        {
            fo.FadeMaterials[i] = new Material(renderer.sharedMaterials[i]);
            ConfigureMaterialForTransparency(fo.FadeMaterials[i]);
        }

        fadingObjects[renderer] = fo;
    }

    private void ConfigureMaterialForTransparency(Material mat)
    {
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.renderQueue = 3000;
    }

    private void ApplyAlpha(Renderer renderer, FadingObject fo, float alpha)
    {
        for (int i = 0; i < fo.FadeMaterials.Length; i++)
        {
            Color color = fo.FadeMaterials[i].color;
            color.a = alpha;
            fo.FadeMaterials[i].color = color;
        }

        // Aplicar materiales transparentes
        renderer.sharedMaterials = fo.FadeMaterials;
    }

    private void OnDestroy()
    {
        // Limpieza final de materiales
        foreach (var kvp in fadingObjects)
        {
            if (kvp.Value.FadeMaterials != null)
            {
                foreach (Material mat in kvp.Value.FadeMaterials)
                {
                    if (mat != null) Destroy(mat);
                }
            }
        }
        fadingObjects.Clear();
    }
}