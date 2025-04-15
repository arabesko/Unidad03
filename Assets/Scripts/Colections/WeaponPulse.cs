using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPulse : Weapon
{
    [SerializeField] public GameObject _myBulletPrebaf;
    [SerializeField] public Transform _instancePoint;
    public override void Initialized(Player player)
    {
        base.Initialized(player);
    }

    public override void PowerElement()
    {
        Instantiate(_myBulletPrebaf, _instancePoint.position, transform.rotation);
    }
}
