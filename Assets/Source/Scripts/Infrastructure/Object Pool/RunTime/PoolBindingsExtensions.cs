using UnityEngine;

public static class PoolBindingsExtensions
{
    public static BindingBuilder BindComponentPool<T>( this IContainer container, T prefab, int preload = 0, int maxSize = 0, Transform parent = null) where T : Component
    {
        return container.Bind<IPool<T>>(_ => new ComponentPool<T>(prefab, _, preload, maxSize, parent)).AsSingle();
    }

    public static BindingBuilder BindObjectPool<T>(this IContainer container, System.Func<T> factory, int preload = 0)
    {
        return container.Bind<IPool<T>>(_ => new ObjectPool<T>(factory, preload: preload)).AsSingle();
    }
}