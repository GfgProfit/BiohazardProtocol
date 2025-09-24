using UnityEngine.Rendering;

public sealed class HealthModel
{
    public int Max { get; private set; }
    public int Current { get; private set; }

    public HealthModel(int max)
    {
        Max = max;
        Current = max;
    }

    public void Reset(int max)
    {
        Max = max;
        Current = max;
    }

    public bool IsDead => Current <= 0;

    public void Heal(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Current = System.Math.Min(Current + amount, Max);
    }

    public void Damage(int amount)
    {
        if (amount <= 0)
        {
            return;
        }

        Current -= amount;

        if (Current <= 0)
        {
            Current = 0;
        }
    }
}