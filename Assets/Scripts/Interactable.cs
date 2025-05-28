using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public class Interactable : MonoBehaviour
{
    public float activationDistance = 3f;
    public KeyCode interactionKey = KeyCode.E;
    public GameObject floatingUI;
    public Vector3 uiOffset = new Vector3(0, 2f, 0);
    public UnityEvent onInteract;

    GameObject player;
    GameObject uiInstance;
    bool isActive, wasActive;

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (floatingUI != null)
        {
            uiInstance = Instantiate(floatingUI);
            uiInstance.transform.SetParent(null);
            uiInstance.SetActive(false);
        }
    }

    void Update()
    {
        if (!player) return;

        float dist = Vector3.Distance(transform.position, player.transform.position);
        isActive = dist <= activationDistance;

        if (isActive)
        {
            ShowUI();
            if (Input.GetKeyDown(interactionKey))
            {
                onInteract.Invoke();
                HideUI();
            }
        }
        else if (wasActive)
        {
            HideUI();
        }

        wasActive = isActive;
    }

    void ShowUI()
    {
        if (!uiInstance) return;
        uiInstance.SetActive(true);
        uiInstance.transform.position = transform.position + uiOffset;
        Vector3 lookDir = uiInstance.transform.position - Camera.main.transform.position;
        uiInstance.transform.rotation = Quaternion.LookRotation(lookDir, Vector3.up);
    }

    void HideUI()
    {
        if (uiInstance) uiInstance.SetActive(false);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, activationDistance);
    }
}