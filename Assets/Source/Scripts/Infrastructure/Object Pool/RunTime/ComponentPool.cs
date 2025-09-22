using System;
using System.Collections.Generic;
using UnityEngine;

public sealed class ComponentPool<T> : IPool<T>, IDisposable where T : Component
{
    private readonly Queue<T> _queue = new();
    private readonly T _prefab;
    private readonly Transform _parent;
    private readonly IContainer _container;
    private readonly int _maxSize;

    public int CountInactive => _queue.Count;
    public int CountAll { get; private set; }

    public ComponentPool(T prefab, IContainer container, int preload = 0, int maxSize = 0, Transform parent = null)
    {
        _prefab = prefab ? prefab : throw new ArgumentNullException(nameof(prefab));
        _container = container ?? throw new ArgumentNullException(nameof(container));
        _parent = parent;
        _maxSize = maxSize;

        for (int i = 0; i < preload; i++)
        {
            _queue.Enqueue(InstantiateItem(inactive: true));
        }
    }

    public T Get()
    {
        T item = _queue.Count > 0 ? _queue.Dequeue() : InstantiateItem(inactive: false);

        if (!item.gameObject.activeSelf)
        {
            item.gameObject.SetActive(true);
        }

        _container.InjectGameObject(item.gameObject, includeInactive: true);

        (item as IPoolable)?.OnSpawned();
        return item;
    }

    public void Release(T item)
    {
        if (!item)
        {
            return;
        }

        (item as IPoolable)?.OnDespawned();
        item.gameObject.SetActive(false);

        if (_maxSize > 0 && CountAll > _maxSize)
        {
            UnityEngine.Object.Destroy(item.gameObject);
            CountAll--;
            return;
        }

        if (_parent)
        {
            item.transform.SetParent(_parent, false);
        }

        _queue.Enqueue(item);
    }

    public void Clear()
    {
        while (_queue.Count > 0)
        {
            T it = _queue.Dequeue();
            if (it)
            {
                UnityEngine.Object.Destroy(it.gameObject);
            }
        }

        CountAll = 0;
    }

    public void Dispose() => Clear();

    private T InstantiateItem(bool inactive)
    {
        T inst = UnityEngine.Object.Instantiate(_prefab, _parent);

        if (inactive)
        {
            inst.gameObject.SetActive(false);
        }

        CountAll++;
        return inst;
    }
}