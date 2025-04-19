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

    public void DetectionColor();
}
