using UnityEngine;

public class ZombieChasePlayerState : IZombieState
{
    private readonly Zombie _self;
    private Transform _target;

    // как часто обновлять путь, и на сколько сдвинулся игрок, чтобы обновлять немедленно
    private readonly float _repathInterval;
    private readonly float _repathMoveThreshold;
    private float _timer;
    private Vector3 _lastTargetPos;

    // опционально: дистанция атаки, чтобы сменить состояние
    private readonly float _attackRange;

    public ZombieChasePlayerState(Zombie self, Transform target, float repathInterval = 0.2f, float repathMoveThreshold = 0.4f, float attackRange = 1.2f)
    {
        _self = self;
        _target = target;
        _repathInterval = Mathf.Max(0.05f, repathInterval);
        _repathMoveThreshold = Mathf.Max(0.05f, repathMoveThreshold);
        _attackRange = Mathf.Max(0f, attackRange);
    }

    public void Enter(Zombie zombie)
    {
        _timer = 0f;
        if (_target != null)
        {
            _lastTargetPos = _target.position;
            _self.SetDestinationToPoint(_lastTargetPos);
        }
    }

    public void Exit(Zombie zombie)
    {
        // ничего
    }

    public void Tick(Zombie zombie)
    {
        if (_target == null)
        {
            // цель потеряна — можно встать или пойти к последней известной
            _self.SetDestinationToPoint(_lastTargetPos);
            return;
        }

        _timer -= Time.deltaTime;

        // если цель заметно сдвинулась — сразу перестраиваем
        Vector3 cur = _target.position;
        if ((cur - _lastTargetPos).sqrMagnitude >= _repathMoveThreshold * _repathMoveThreshold || _timer <= 0f)
        {
            _lastTargetPos = cur;
            _self.SetDestinationToPoint(cur);
            _timer = _repathInterval;
        }

        // при желании — авто-переход в атаку у игрока
        if (_attackRange > 0f && _self.IsNear(cur, _attackRange))
        {
            // TODO: ChangeState(_attackPlayerState);
            // пока просто повернуться к игроку
            _self.StopAndFace(cur);
        }
    }

    public void SetTarget(Transform t) => _target = t; // на случай смены цели на лету
}