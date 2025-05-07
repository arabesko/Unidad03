using UnityEngine;

[RequireComponent(typeof(Collider))]
public class SDCardPickup : MonoBehaviour
{
    void Reset()
    {
        GetComponent<Collider>().isTrigger = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;

        var switcher = FindObjectOfType<PlayerModuleSwitcher>();
        if (switcher != null)
        {
            // Solo habilitamos el permiso de switch, sin cambiar ahora mismo
            switcher.UnlockSwitching();
        }
        else
        {
            Debug.LogWarning("No encontré PlayerModuleSwitcher en la escena.");
        }

        Destroy(gameObject);
    }
}
