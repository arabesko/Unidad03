using System.Collections.Generic;
using UnityEngine;

public class ElementPuzzle : MonoBehaviour, IPuzzlesElements
{
    [SerializeField] private bool _activateRotation;
    [SerializeField] private float _speedRotation;
    [SerializeField] private List<MeshRenderer> _MyParts;

    public void Activate()
    {
        _activateRotation = true;
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_activateRotation)
        {
            transform.Rotate(2*_speedRotation * Time.deltaTime, _speedRotation * Time.deltaTime, _speedRotation * Time.deltaTime);
        }
    }

    public void Desactivate()
    {
        _activateRotation = false;
    }

    public void OriginalColor()
    {
        //Almacenar sus materiales originales
    }

    public void DetectionColor()
    {
        //Poner en blanco por una fraccion de segundos los elementos de puzzles
    }
}
