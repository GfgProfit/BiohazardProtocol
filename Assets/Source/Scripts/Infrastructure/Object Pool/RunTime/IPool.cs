using System;

public interface IPool<T>
{
    T Get();
    void Release(T item);
    int CountInactive { get; }
    int CountAll { get; }
    void Clear();
}