using UnityEngine;
using UnityEngine.AI;

#pragma warning disable IDE0044
#pragma warning disable IDE0079
#pragma warning disable UNT0008

public class Zombie : PooledBehaviour<Zombie>
{
    [SerializeField] private ZombieHealthView _healthView;
    [SerializeField] private NavMeshAgent _agent;

    [Inject] private HealthModel _health;
    [Inject] private WaveService _waves;

    private Transform _spawnPoint;
    
    public event System.Action<Zombie> Died;

    public override void OnDespawned()
    {
        _healthView.OnDespawned();

        _spawnPoint = null;
    }

    public override void OnSpawned()
    {
        _health.Reset(_waves.CurrentBaseHP);
        _healthView?.Setup(_health);
    }

    public void SetSpawnPoint(Transform point)
    {
        _spawnPoint = point;

        if (_agent != null)
        {
            _agent.enabled = false;
        }

        transform.SetPositionAndRotation(_spawnPoint.position, _spawnPoint.rotation);

        if (_agent != null)
        {
            _agent.enabled = true;

            _agent.Warp(transform.position);
        }
    }

    public void Damage(int damage)
    {
        _healthView.Damage(damage, Death);
    }

    [ContextMenu("TEST_DEATH")]
    public void TEST()
    {
        Damage(100000);
    }

    private void Death()
    {
        ReturnToPool();
        Died?.Invoke(this);
    }
}