using UnityEngine;

public class ZombieSpawnState : IZombieState
{
    private readonly Transform _spawnPoint;
    private readonly Zombie _self;
    private readonly BarricadeSlots _slots;

    public ZombieSpawnState(Transform spawnPoint, Zombie self, BarricadeSlots slots)
    {
        _self = self;
        _spawnPoint = spawnPoint;
        _slots = slots;
    }

    public void Enter(Zombie zombie)
    {
        zombie.SetSpawnPoint(_spawnPoint);
        zombie.ChangeState(new ZombieWalkToBarricadeState(_slots, _self));
    }

    public void Exit(Zombie zombie)
    {
    }

    public void Tick(Zombie zombie)
    {
    }
}