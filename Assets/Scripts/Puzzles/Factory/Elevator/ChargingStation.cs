using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingStation : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            other.GetComponent<PortableBattery>().StartCharging();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Battery"))
        {
            other.GetComponent<PortableBattery>().StopCharging();
        }
    }
}
