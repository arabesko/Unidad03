using UnityEngine;

public class PortableBattery : MonoBehaviour
{
    public bool isCharged = false;
    [SerializeField] private float chargeTime = 5f;
    [SerializeField] private float timer = 0f;
    private bool isCharging = false;

    [Header("UI / SFX")]
    public ChargingStation currentStation; // ? Referencia a la estación que la está cargando
    public AudioSource audioSource;
    public AudioClip chargedSound;

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

                if (currentStation != null)
                {
                    currentStation.HideChargingText();
                    currentStation = null;
                }

                if (audioSource != null && chargedSound != null)
                    audioSource.PlayOneShot(chargedSound);
            }
        }
    }

    public void StartCharging(ChargingStation station)
    {
        if (!isCharged)
        {
            isCharging = true;
            timer = 0f;
            currentStation = station;
        }
    }

    public void StopCharging()
    {
        isCharging = false;
        timer = 0f;
        currentStation = null;
    }
}
