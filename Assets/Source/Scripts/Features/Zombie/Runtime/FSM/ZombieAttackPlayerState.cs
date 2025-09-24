using UnityEngine;
using UnityEngine.AI;

public sealed class ZombieAttackPlayerState : IZombieState
{
    private readonly Zombie _self;
    private Transform _target;
    private readonly float _attackRange;
    private readonly int _damage;
    private readonly LayerMask _losObstacles;
    private readonly Transform _attackOrigin;

    private readonly float _turnSpeed = 720f;
    private IZombieState _chaseState;

    private NavMeshAgent _agent;

    public ZombieAttackPlayerState(
        Zombie self,
        Transform target,
        IZombieState chaseState,
        float attackRange = 1.2f,
        float exitBuffer = 0.25f,
        int damage = 10,
        LayerMask losObstacles = default,
        Transform attackOrigin = null)
    {
        _self = self;
        _target = target;
        _chaseState = chaseState;

        _attackRange = Mathf.Max(0f, attackRange);
        _damage = Mathf.Max(1, damage);
        _losObstacles = losObstacles;
        _attackOrigin = attackOrigin != null ? attackOrigin : self.transform;
    }

    public void Enter(Zombie zombie)
    {
        _agent = _self.Agent;

        if (_agent != null)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }

        _self.EnableUpperLayer(true);
        _self.SetUpperAttackBool(true);

        _self.BindAttackHit(OnAttackHit);
        _self.BindAttackFinished(OnAttackFinished);

        FaceTargetInstant();
    }

    public void Exit(Zombie zombie)
    {
        _self.UnbindAttackHit();
        _self.UnbindAttackFinished();

        _self.SetUpperAttackBool(false);
        _self.EnableUpperLayer(false);

        if (_agent != null)
        {
            _agent.isStopped = false;
        }
    }

    public void Tick(Zombie zombie)
    {
        if (_target == null)
        {
            _self.ChangeState(_chaseState);
            return;
        }

        float dist = HorizontalDistance(_self.transform.position, _target.position);

        if (dist > _attackRange)
        {
            _self.ChangeState(_chaseState);

            return;
        }

        if (!HasLineOfSight(_attackOrigin.position, _target.position))
        {
            _self.ChangeState(_chaseState);

            return;
        }

        FaceTargetSmooth();
    }

    public void SetChaseState(IZombieState chase) => _chaseState = chase;
    public void SetTarget(Transform t) => _target = t;

    private void OnAttackHit()
    {
        if (_target == null)
        {
            return;
        }

        if (!HasLineOfSight(_attackOrigin.position, _target.position))
        {
            return;
        }

        float dist = Vector3.Distance(_attackOrigin.position, _target.position);

        if (dist > _attackRange + 0.05f)
        {
            return;
        }

        if (_target.TryGetComponent<PlayerHealth>(out var dmg))
        {
            dmg.TakeDamage(_damage);

            return;
        }

        var hits = Physics.OverlapSphere(_attackOrigin.position, _attackRange, ~0, QueryTriggerInteraction.Ignore);

        foreach (var h in hits)
        {
            if (h.TryGetComponent<PlayerHealth>(out var d))
            {
                d.TakeDamage(_damage);

                break;
            }
        }
    }

    private void OnAttackFinished()
    {
        if (_target == null)
        {
            _self.ChangeState(_chaseState);
            return;
        }

        float dist = HorizontalDistance(_self.transform.position, _target.position);

        if (dist > _attackRange || !HasLineOfSight(_attackOrigin.position, _target.position))
        {
            _self.ChangeState(_chaseState);
            return;
        }
    }

    private void FaceTargetInstant()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 dir = _target.position - _self.transform.position;
        dir.y = 0f;

        if (dir.sqrMagnitude > 0.0001f)
        {
            _self.transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    private void FaceTargetSmooth()
    {
        if (_target == null)
        {
            return;
        }

        Vector3 to = _target.position - _self.transform.position;
        to.y = 0f;

        if (to.sqrMagnitude < 0.0001f)
        {
            return;
        }

        var wanted = Quaternion.LookRotation(to);
        _self.transform.rotation = Quaternion.RotateTowards(_self.transform.rotation, wanted, _turnSpeed * Time.deltaTime);
    }

    private bool HasLineOfSight(Vector3 from, Vector3 to)
    {
        Vector3 dir = to - from;
        float dist = dir.magnitude;

        if (dist <= 0.001f)
        {
            return true;
        }

        dir /= dist;

        return !Physics.Raycast(from, dir, dist, _losObstacles, QueryTriggerInteraction.Ignore);
    }

    private static float HorizontalDistance(Vector3 a, Vector3 b)
    {
        float dx = a.x - b.x;
        float dz = a.z - b.z;
        return Mathf.Sqrt(dx * dx + dz * dz);
    }
}