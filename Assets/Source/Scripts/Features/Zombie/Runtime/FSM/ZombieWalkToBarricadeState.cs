using UnityEngine;

public class ZombieWalkToBarricadeState : IZombieState
{
    private readonly BarricadeSlots _slots;
    private readonly Zombie _self;

    private int _mySlot = -1;
    private Vector3 _currentTarget;
    private float _retryTimer;

    public ZombieWalkToBarricadeState(BarricadeSlots slots, Zombie self)
    {
        _slots = slots;
        _self = self;
    }

    public void Enter(Zombie zombie)
    {
        if (_slots == null || _self == null)
        {
            return;
        }

        if (_slots.TryReserveBest(_self, out _mySlot, out Vector3 pos))
        {
            _currentTarget = pos;
        }
        else
        {
            _mySlot = -1;
            _currentTarget = _slots.GetWaitingPointFor(_self);
            _retryTimer = _slots.RetryReserveInterval;
        }

        _self.SetDestinationToPoint(_currentTarget);
    }

    public void Exit(Zombie zombie)
    {
        _slots?.Release(_self);
        _mySlot = -1;
    }

    public void Tick(Zombie zombie)
    {
        if (_slots == null)
        {
            return;
        }

        if (_mySlot >= 0)
        {
            Vector3 desired = _slots.GetSlotPosition(_mySlot);

            if ((desired - _currentTarget).sqrMagnitude > 0.02f)
            {
                _currentTarget = desired;
                _self.SetDestinationToPoint(_currentTarget);
            }

            _retryTimer -= Time.deltaTime;

            if (_retryTimer <= 0f)
            {
                if (_slots.TryUpgrade(_self, _mySlot, out var betterSlot, out Vector3 betterPos))
                {
                    _mySlot = betterSlot;
                    _currentTarget = betterPos;
                    _self.SetDestinationToPoint(_currentTarget);
                }
                _retryTimer = _slots.RetryReserveInterval;
            }

            if (_self.IsNear(_currentTarget, 0.5f))
            {
                _self.OnReachedBarricadeSlot();
            }

            return;
        }

        _retryTimer -= Time.deltaTime;

        if (_retryTimer <= 0f)
        {
            if (_slots.TryReserveBest(_self, out _mySlot, out Vector3 pos))
            {
                _currentTarget = pos;
                _self.SetDestinationToPoint(_currentTarget);
            }
            else
            {
                _currentTarget = _slots.GetWaitingPointFor(_self);
                _self.SetDestinationToPoint(_currentTarget);
            }

            _retryTimer = _slots.RetryReserveInterval;
        }
    }
}