using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ElementPuzzle : MonoBehaviour, IPuzzlesElements
{
    [SerializeField] private bool _activateRotation;
    [SerializeField] private float _speedRotation;
    [SerializeField] private List<MeshRenderer> _MyParts;
    [SerializeField] private List<Material> _MyPartsBackup;

    [SerializeField] private Material _myMaterialLight;

    private bool _activateRadar = false;

    public void Activate()
    {
        _activateRotation = true;
    }

    void Start()
    {
        foreach (var part in _MyParts)
        {
            _MyPartsBackup.Add(part.material);
        }
    }

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
        for (int i = 0; i < _MyPartsBackup.Count; i++)
        {
            _MyParts[i].material = _MyPartsBackup[i];
        }
    }

    public void DetectionColor()
    {
        foreach(var item in _MyParts)
        {
            item.material = _myMaterialLight;
        }
    }

    public IEnumerator ChangeColorBlink(float time)
    {
        DetectionColor();
        yield return new WaitForSeconds(time);
        OriginalColor();
        yield return new WaitForSeconds(time);
        DetectionColor();
        yield return new WaitForSeconds(time);
        OriginalColor();
        yield return new WaitForSeconds(time);
        DetectionColor();
        yield return new WaitForSeconds(time);
        OriginalColor();
        yield return new WaitForSeconds(time);
        DetectionColor();
        yield return new WaitForSeconds(time);
        OriginalColor();
        yield return new WaitForSeconds(time);
        DetectionColor();
        yield return new WaitForSeconds(time);
        OriginalColor();
        _activateRadar = false;
    }

    public void RadarActivate()
    {
        if (_activateRadar) return;
        StartCoroutine(ChangeColorBlink(0.05f));
        _activateRadar = true;
    }
}
