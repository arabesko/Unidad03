using UnityEngine;

public class Weapon : MonoBehaviour, IDrivers
{
    private MeshRenderer _render;
    protected Player _player;

    private void Awake()
    {
        _render = GetComponent<MeshRenderer>();
    }
    public virtual void Initialized(Player player)
    {
       _player = player;
    }
    public virtual void PowerElement()
    {
    }

    public void ChangeColor(Player player)
    {
        //Provisorio, solo para que se entienda que paso algo al colectar
        foreach (var item in player.bodyRender)
        {
            item.material = _render.material;
        }
    }
}
