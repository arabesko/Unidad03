using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRadar : Weapon
{
    [SerializeField] private float _radarRadious;
    [SerializeField] private LayerMask _targetLayer; // Capa de objetos a detectar
    [SerializeField] private Material _myLitMaterial;
    [SerializeField] private bool _canRadar = true;

    public float RadarRadious {  get {  return _radarRadious; } set { _radarRadious = value; } }

    public override void Initialized(PlayerMovement player)
    {
        base.Initialized(player);
    }


    public override void PowerElement()
    {
        if (!_canRadar) return;
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radarRadious, _targetLayer);
        foreach (Collider collider in hitColliders)
        {
            IPuzzlesElements myPuzzle = collider.GetComponent<IPuzzlesElements>();
            if (myPuzzle != null)
            {
                myPuzzle.ActionPuzzle();
            }
        }
        _canRadar = false;
        StartCoroutine(CanRadarAgain());
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radarRadious);
    }

    private IEnumerator CanRadarAgain()
    {
        yield return new WaitForSeconds(3);
        _canRadar = true;
        _player.CanWeaponChange = true;
    }

}
