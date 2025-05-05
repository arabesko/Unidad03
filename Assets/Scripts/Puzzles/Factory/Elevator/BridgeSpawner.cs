using System.Collections;
using UnityEngine;
using TMPro;

public class BridgeSpawner : MonoBehaviour
{
    [Header("Bridge Settings")]
    public GameObject bridgePrefab;
    public Transform spawnPoint;
    public float bridgeDuration = 4f;

    [Header("UI Settings")]
    public GameObject interactPanel;
    public TMP_Text interactText;    

    private bool playerInRange = false;
    private bool isSpawning = false;

    private void Start()
    {
        if (interactPanel != null)
            interactPanel.SetActive(false);
    }

    private void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E) && !isSpawning)
        {
            StartCoroutine(SpawnBridge());
        }
    }

    private IEnumerator SpawnBridge()
    {
        isSpawning = true;
        if (interactPanel != null)
            interactPanel.SetActive(false);

        GameObject bridge = Instantiate(bridgePrefab, spawnPoint.position, spawnPoint.rotation);
        yield return new WaitForSeconds(bridgeDuration);
        Destroy(bridge);

        isSpawning = false;
        if (playerInRange && interactPanel != null)
            interactPanel.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            if (!isSpawning && interactPanel != null)
            {
                interactPanel.SetActive(true);
                interactText.text = "Presione E para interactuar";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (interactPanel != null)
                interactPanel.SetActive(false);
        }
    }
}
