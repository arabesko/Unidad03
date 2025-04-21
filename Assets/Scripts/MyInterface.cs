using System.Collections.Generic;

public interface IDamagiable
{
    public void Health(int health);
    public void Damage(int damage);
}

public interface IDrivers
{
    public void Initialized(Player player);
    public void PowerElement();
}

public interface IPuzzlesElements
{
    public void Activate();
    public void Desactivate();

    public void ActionPuzzle();
    public int MyReturnNumber();
}

public enum ModulosUnit03
{
    BrazoIzquierdo,
    Proyector,
    BrazoDerecho
}
