using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortableBattery : MonoBehaviour
{
    public bool isCharged = false;
    [SerializeField] private float chargeTime = 5f;
    [SerializeField] private float timer = 0f;
    private bool isCharging = false;

    void Update()
    {
        if (isCharging)
        {
            timer += Time.deltaTime;
            if (timer >= chargeTime)
            {
                isCharged = true;
                isCharging = false;
                Debug.Log("Batería cargada completamente.");
            }
        }
    }

    public void StartCharging()
    {
        if (!isCharged)
        {
            isCharging = true;
            timer = 0f;
        }
    }

    public void StopCharging()
    {
        isCharging = false;
        timer = 0f;
    }
}
