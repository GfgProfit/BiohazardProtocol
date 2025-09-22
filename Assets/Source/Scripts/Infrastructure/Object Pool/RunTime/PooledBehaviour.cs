using UnityEngine;

public abstract class PooledBehaviour<T> : MonoBehaviour, IPoolable where T : Component
{
    private IPool<T> _pool;

    [Inject] public IPool<T> Pool
    {
        private get => _pool;
        set => _pool = value;
    }

    public void ReturnToPool()
    {
        if (_pool == null)
        {
            gameObject.SetActive(false);
            return;
        }

        _pool.Release(this as T);
    }

    public virtual void OnSpawned() { }
    public virtual void OnDespawned() { }
}