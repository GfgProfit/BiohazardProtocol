using UnityEngine;

public interface ITargetable
{
    void Focus();
    void Unfocus();
    bool IsDead { get; }
}