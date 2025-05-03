using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ScanObject : MonoBehaviour
{
    public GameObject scanPanel;       // Panel que contiene texto e imagen
    public TMP_Text scanPromptText;    // Texto TMP: "Presiona E..."
    public Image scanResultImage;      // Imagen que se muestra tras el escaneo

    private bool isPlayerInRange = false;
    private bool isScanning = false;

    private void Start()
    {
        scanPanel.SetActive(false);
        scanResultImage.gameObject.SetActive(false);
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
        scanPromptText.gameObject.SetActive(false); // Oculta solo el texto

        yield return new WaitForSeconds(3f); // Espera antes de mostrar imagen

        scanResultImage.gameObject.SetActive(true);

        yield return new WaitForSeconds(5f); // Imagen visible por 5 segundos

        scanResultImage.gameObject.SetActive(false);
        scanPromptText.gameObject.SetActive(true);  // Reactiva el texto
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
