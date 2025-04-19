
using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

public class WeaponInvisible : Weapon
{
    public override void Initialized(Player player)
    {
        base.Initialized(player);
    }

    public override void PowerElement()
    {
        if (_player.IsInvisible) return;
        base.PowerElement();
        _player.IsInvisible = true;
        ChangeColor(_player);
        StartCoroutine(InvisibleTime());
    }

    public IEnumerator InvisibleTime()
    {
        yield return new WaitForSeconds(5);
        RecoveryMaterial();
    }
    public void RecoveryMaterial()
    {
        for (int i = 0; i < _player.BodyRenderOriginal.Count; i++)
        {
            _player.bodyRender[i].material = _player.BodyRenderOriginal[i];
        }
        _player.IsInvisible = false;
    }

    
}
