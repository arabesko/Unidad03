using UnityEngine;

public class PickupAnimator : MonoBehaviour
{
    [Header("Rotación")]
    public Vector3 rotationPerSecond = new Vector3(0, 90, 0);

    [Header("Bobbing")]
    public float bobAmplitude = 0.25f;
    public float bobFrequency = 1f;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    void Update()
    {
        // Rotar continuamente
        transform.Rotate(rotationPerSecond * Time.deltaTime, Space.World);

        // Bobear (eje Y)
        float newY = startPos.y + Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
        transform.position = new Vector3(startPos.x, newY, startPos.z);
    }
}
