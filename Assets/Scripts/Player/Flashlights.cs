using UnityEngine;

public class Flashlights : MonoBehaviour
{
    [SerializeField] private Light linternaIzquierda;
    [SerializeField] private Light linternaDerecha;
    private bool lucesEncendidas = true;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            lucesEncendidas = !lucesEncendidas;

            linternaIzquierda.enabled = lucesEncendidas;
            linternaDerecha.enabled = lucesEncendidas;
        }
    }
}
