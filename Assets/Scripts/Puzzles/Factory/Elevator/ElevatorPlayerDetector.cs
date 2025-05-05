using UnityEngine;
using TMPro;

public class ElevatorPlayerDetector : MonoBehaviour
{
    [SerializeField] private ElevatorPower elevatorPower;
    [SerializeField] private GameObject batteryPromptPanel; // Panel contenedor del mensaje
    [SerializeField] private TextMeshProUGUI batteryPromptText; // Texto del mensaje

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorPower.SetPlayerOnPlatform(true);

            // Mostrar mensaje solo si no hay batería
            if (!elevatorPower.HasPower())
            {
                if (batteryPromptPanel != null)
                    batteryPromptPanel.SetActive(true);

                if (batteryPromptText != null)
                    batteryPromptText.text = "El elevador no tiene energia.";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorPower.SetPlayerOnPlatform(false);

            if (batteryPromptPanel != null)
                batteryPromptPanel.SetActive(false);
        }
    }
}
