using System.Collections;
using UnityEngine;
using TMPro;

public class ScanObject : MonoBehaviour
{
    public GameObject scanPanel;
    public TMP_Text scanPromptText;
    public GameObject scanResultObject;

    private bool isPlayerInRange = false;
    private bool isScanning = false;

    private void Start()
    {
        scanPanel.SetActive(false);
        scanResultObject.SetActive(false);
    }

    private void Update()
    {
        if (isPlayerInRange && Input.GetKeyDown(KeyCode.E) && !isScanning)
        {
            StartCoroutine(StartScan());
        }
    }

    private IEnumerator StartScan()
    {
        isScanning = true;
        scanPromptText.gameObject.SetActive(false);

        yield return new WaitForSeconds(3f); // Simula tiempo de escaneo

        scanResultObject.SetActive(true); // Activa objeto 3D

        yield return new WaitForSeconds(5f); // Muestra objeto durante 5s

        scanResultObject.SetActive(false);
        scanPromptText.gameObject.SetActive(true);
        isScanning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (!isScanning)
                scanPanel.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            scanPanel.SetActive(false);
        }
    }
}
