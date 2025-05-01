using System.Collections;
using UnityEngine;

public class ElevatorPower : MonoBehaviour
{
    [SerializeField] private GameObject elevator;
    [SerializeField] private Transform upPosition;
    [SerializeField] private Transform downPosition;
    [SerializeField] private float speed = 2f;

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

                if (playerOnPlatform && !isMoving)
                {
                    StartCoroutine(MoveElevator());
                }
            }
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
