using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponGarras : Weapon
{
    public override void Initialized(PlayerMovement player)
    {
        base.Initialized(player);
    }

    public override void PowerElement()
    {
        //Aqui se debe hacer el da�o al enemigo utilizando el area de ataque de las garras
    }
}
