using System.Collections;
using UnityEngine;

public class BridgeSpawner : MonoBehaviour
{
    public GameObject bridgePrefab; // Prefab del puente
    public Transform spawnPoint;    // Punto donde aparecerá el puente
    public float bridgeDuration = 4f;

    private bool playerInRange = false;

    void Update()
    {
        if (playerInRange && Input.GetKeyDown(KeyCode.E))
        {
            StartCoroutine(SpawnBridge());
        }
    }

    private IEnumerator SpawnBridge()
    {
        GameObject bridge = Instantiate(bridgePrefab, spawnPoint.position, spawnPoint.rotation);
        yield return new WaitForSeconds(bridgeDuration);
        Destroy(bridge);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }
}
