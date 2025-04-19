using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuctButton : MonoBehaviour
{
    [SerializeField] private GameObject _ductToOpen;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_ductToOpen != null)
            {
                _ductToOpen.SetActive(false); // Simula que la puerta se abre
                Debug.Log("Puerta abierta");
            }
        }
    }
}
