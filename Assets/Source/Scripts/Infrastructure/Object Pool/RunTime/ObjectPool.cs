using System;
using System.Collections.Generic;

public sealed class ObjectPool<T> : IPool<T>, IDisposable
{
    private readonly Stack<T> _stack = new();
    private readonly Func<T> _factory;
    private readonly Action<T> _onSpawned;
    private readonly Action<T> _onDespawned;

    public int CountInactive => _stack.Count;
    public int CountAll { get; private set; }

    public ObjectPool(Func<T> factory, Action<T> onSpawned = null, Action<T> onDespawned = null, int preload = 0)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _onSpawned = onSpawned;
        _onDespawned = onDespawned;

        for (int i = 0; i < preload; i++)
        {
            _stack.Push(Create());
        }
    }

    public T Get()
    {
        T item = _stack.Count > 0 ? _stack.Pop() : Create();
        (_onSpawned ?? TryCallSpawned)?.Invoke(item);
        return item;
    }

    public void Release(T item)
    {
        if (item is null)
        {
            return;
        }

        (_onDespawned ?? TryCallDespawned)?.Invoke(item);
        _stack.Push(item);
    }

    public void Clear() => _stack.Clear();

    private T Create()
    {
        T item = _factory();
        CountAll++;
        return item;
    }

    private static void TryCallSpawned(T item) => (item as IPoolable)?.OnSpawned();
    private static void TryCallDespawned(T item) => (item as IPoolable)?.OnDespawned();

    public void Dispose() => Clear();
}