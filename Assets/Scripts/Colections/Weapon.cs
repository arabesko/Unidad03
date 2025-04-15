using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Weapon : MonoBehaviour, IDrivers
{
    private MeshRenderer _render;

    private void Awake()
    {
        _render = GetComponent<MeshRenderer>();
    }
    public virtual void Initialized(Player player)
    {
       ChangeColor(player);
    }
    public virtual void PowerElement()
    {
        //Lo que sea que haga cada arma
    }

    public void ChangeColor(Player player)
    {
        //Provisorio, solo para que se entienda que paso algo al colectar
        foreach (var item in player.BodyRender)
        {
            item.material = _render.material;
        }
    }
}
