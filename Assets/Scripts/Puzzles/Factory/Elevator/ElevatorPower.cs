using System.Collections;
using UnityEngine;
using TMPro;

public class ElevatorPower : MonoBehaviour
{
    [Header("Elevator Settings")]
    [SerializeField] private GameObject elevator;
    [SerializeField] private Transform upPosition;
    [SerializeField] private Transform downPosition;
    [SerializeField] private float speed = 2f;

    [Header("UI")]
    [SerializeField] private GameObject elevatorPromptPanel;
    [SerializeField] private TextMeshProUGUI elevatorPromptText;

    private bool hasPower = false;
    private bool playerOnPlatform = false;
    private bool isMoving = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            PortableBattery battery = other.GetComponent<PortableBattery>();
            if (battery != null && battery.isCharged)
            {
                hasPower = true;
                Debug.Log("Batería instalada, elevador con energía.");

                // Ocultar mensaje si estaba visible
                if (elevatorPromptPanel != null)
                    elevatorPromptPanel.SetActive(false);

                if (playerOnPlatform && !isMoving)
                {
                    StartCoroutine(MoveElevator());
                }
            }
        }
        else if (other.CompareTag("Player"))
        {
            SetPlayerOnPlatform(true);

            if (!hasPower)
            {
                if (elevatorPromptPanel != null)
                    elevatorPromptPanel.SetActive(true);

                if (elevatorPromptText != null)
                    elevatorPromptText.text = "Le falta energía al elevador.";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SetPlayerOnPlatform(false);

            if (elevatorPromptPanel != null)
                elevatorPromptPanel.SetActive(false);
        }
    }

    public void SetPlayerOnPlatform(bool state)
    {
        playerOnPlatform = state;

        if (hasPower && playerOnPlatform && !isMoving)
        {
            StartCoroutine(MoveElevator());
        }
    }

    public bool HasPower()
    {
        return hasPower;
    }

    IEnumerator MoveElevator()
    {
        isMoving = true;

        Vector3 currentPos = elevator.transform.position;
        Vector3 target;

        if (Vector3.Distance(currentPos, downPosition.position) < 0.1f)
        {
            target = upPosition.position;
        }
        else
        {
            target = downPosition.position;
        }

        while (Vector3.Distance(elevator.transform.position, target) > 0.05f)
        {
            elevator.transform.position = Vector3.MoveTowards(elevator.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }

        isMoving = false;
    }
}
