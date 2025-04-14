using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform _target; // Arrastra el jugador aquí desde el Inspector

    [Header("Ajustes")]
    [SerializeField] private float _smoothSpeed = 5f; // Suavizado del movimiento
    [SerializeField] private float _xOffset = 2f; // Offset en X para no centrar completamente la cámara
    [SerializeField] private float _fixedY = 5f; // Altura fija de la cámara (ajusta según tu escena)
    [SerializeField] private float _fixedZ = -10f; // Posición Z fija (distancia desde el jugador)

    void LateUpdate()
    {
        if (_target == null) return;

        // Calcula la posición deseada (solo sigue en X, mantiene Y y Z fijas)
        Vector3 desiredPosition = new Vector3(
            _target.position.x + _xOffset,
            _fixedY,
            _fixedZ
        );

        // Interpola suavemente hacia la posición deseada
        Vector3 smoothedPosition = Vector3.Lerp(
            transform.position,
            desiredPosition,
            _smoothSpeed * Time.deltaTime
        );

        transform.position = smoothedPosition;
    }
}
