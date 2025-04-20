using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WeaponRadar : Weapon
{
    [SerializeField] private float _radarRadious;
    [SerializeField] private LayerMask _targetLayer; // Capa de objetos a detectar
    [SerializeField] private Material _myLitMaterial;


    public override void Initialized(Player player)
    {
        base.Initialized(player);
    }

    public override void PowerElement()
    {
        base.PowerElement();
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, _radarRadious, _targetLayer);
        foreach (Collider collider in hitColliders)
        {
            IPuzzlesElements myPuzzle = collider.GetComponent<IPuzzlesElements>();
            if (myPuzzle != null)
            {
                myPuzzle.ActionPuzzle();
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _radarRadious);
    }

}
