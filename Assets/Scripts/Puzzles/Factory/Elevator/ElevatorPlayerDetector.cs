using UnityEngine;

public class ElevatorPlayerDetector : MonoBehaviour
{
    [SerializeField] private ElevatorPower elevatorPower;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorPower.SetPlayerOnPlatform(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            elevatorPower.SetPlayerOnPlatform(false);
        }
    }
}
