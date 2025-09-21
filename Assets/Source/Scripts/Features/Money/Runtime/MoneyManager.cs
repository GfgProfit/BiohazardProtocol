using System;

public class MoneyManager : IMoney
{
    public int CurrentMoney { get; set; }
    public Action<int> OnChanged { get; set; }

    public MoneyManager(int startMoney)
    {
        CurrentMoney = startMoney;
    }

    public bool SpendMoney(int value)
    {
        if (CurrentMoney >= value)
        {
            CurrentMoney -= value;

            OnChanged?.Invoke(CurrentMoney);

            return true;
        }
        else
        {
            return false;
        }
    }

    public void AddMoney(int value)
    {
        CurrentMoney += value;

        OnChanged?.Invoke(CurrentMoney);
    }
}