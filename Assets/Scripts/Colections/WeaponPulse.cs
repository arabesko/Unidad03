using UnityEngine;

public class WeaponPulse : Weapon
{
    [SerializeField] public GameObject _myBulletPrebaf;
    [SerializeField] public Transform _instancePoint;

    public override void Initialized(PlayerMovement player)
    {
        base.Initialized(player);
    }

    public override void PowerElement()
    {
        base.PowerElement();
        Instantiate(_myBulletPrebaf, _instancePoint.position, transform.rotation);
        _player.CanWeaponChange = true;
    }

}
