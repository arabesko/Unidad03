using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _target; 

    [Header("Ajustes")]
    [SerializeField] private float _smoothSpeed = 5f; // Suavizado del movimiento
    [SerializeField] private float _xOffset = 2f; // Offset en X para no centrar completamente la cámara
    [SerializeField] private float _fixedY = 8.3f; // Altura fija de la cámara (ajusta según tu escena)
    [SerializeField] private float _fixedZ = -27.2f; // Posición Z fija (distancia desde el jugador)

    void LateUpdate()
    {
        if (_target == null) return;
        Vector3 desiredPosition = new Vector3(_target.position.x + _xOffset, _fixedY, _fixedZ);
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, _smoothSpeed * Time.deltaTime);
        transform.position = smoothedPosition;
    }
}
