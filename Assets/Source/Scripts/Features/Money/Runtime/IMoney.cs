using System;

public interface IMoney
{
    int CurrentMoney { get; set; }
    bool SpendMoney(int value);
    void AddMoney(int value);
    Action<int> OnChanged { get; set; }
}