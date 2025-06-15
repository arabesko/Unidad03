using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeObstruction : MonoBehaviour
{
    [Header("Configuración")]
    public Transform target;          // El jugador
    public LayerMask fadeLayer;       // Layer a transparentar
    [Range(0.1f, 0.9f)]
    public float targetAlpha = 0.3f;  // Transparencia deseada
    public float fadeSpeed = 5f;      // Velocidad del fade

    private List<FadingObject> objectsInWay = new List<FadingObject>();
    private List<FadingObject> objectsToRestore = new List<FadingObject>();
    private Transform camTransform;

    private class FadingObject
    {
        public Renderer Renderer;
        public Material[] OriginalMaterials;
        public Material[] FadeMaterials;
        public float CurrentAlpha;
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
        Vector3 dir = target.position - camTransform.position;
        float distance = dir.magnitude;

        // Limpiar listas temporales
        objectsToRestore.Clear();
        objectsToRestore.AddRange(objectsInWay);

        // Detectar objetos entre la cámara y el jugador
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

            FadingObject existing = objectsInWay.Find(o => o.Renderer == renderer);

            if (existing != null)
            {
                objectsToRestore.Remove(existing);
            }
            else
            {
                FadingObject newFading = new FadingObject()
                {
                    Renderer = renderer,
                    OriginalMaterials = renderer.sharedMaterials,
                    FadeMaterials = new Material[renderer.sharedMaterials.Length],
                    CurrentAlpha = 1f
                };

                // Crear materiales transparentes
                for (int i = 0; i < renderer.sharedMaterials.Length; i++)
                {
                    newFading.FadeMaterials[i] = new Material(renderer.sharedMaterials[i]);
                    newFading.FadeMaterials[i].SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                    newFading.FadeMaterials[i].SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                    newFading.FadeMaterials[i].EnableKeyword("_ALPHABLEND_ON");
                    newFading.FadeMaterials[i].renderQueue = 3000;
                }

                objectsInWay.Add(newFading);
            }
        }

        // Restaurar objetos que ya no están en el camino
        foreach (FadingObject obj in objectsToRestore)
        {
            StartCoroutine(RestoreObject(obj));
            objectsInWay.Remove(obj);
        }

        // Aplicar transparencia a objetos actuales
        foreach (FadingObject obj in objectsInWay)
        {
            obj.CurrentAlpha = Mathf.MoveTowards(
                obj.CurrentAlpha,
                targetAlpha,
                fadeSpeed * Time.deltaTime
            );

            ApplyAlpha(obj, obj.CurrentAlpha);
        }
    }

    private void ApplyAlpha(FadingObject obj, float alpha)
    {
        // Configurar alpha en todos los materiales
        foreach (Material mat in obj.FadeMaterials)
        {
            Color color = mat.color;
            color.a = alpha;
            mat.color = color;
        }

        obj.Renderer.sharedMaterials = obj.FadeMaterials;
    }

    private IEnumerator RestoreObject(FadingObject obj)
    {
        float startAlpha = obj.CurrentAlpha;
        float progress = 0f;

        while (progress < 1f)
        {
            progress += fadeSpeed * Time.deltaTime;
            obj.CurrentAlpha = Mathf.Lerp(startAlpha, 1f, progress);
            ApplyAlpha(obj, obj.CurrentAlpha);
            yield return null;
        }

        // Restaurar materiales originales
        obj.Renderer.sharedMaterials = obj.OriginalMaterials;

        // Destruir materiales temporales
        foreach (Material mat in obj.FadeMaterials)
        {
            Destroy(mat);
        }
    }
}