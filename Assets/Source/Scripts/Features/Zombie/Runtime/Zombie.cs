using System.Collections;
using UnityEngine;
using UnityEngine.AI;

#pragma warning disable IDE0044
#pragma warning disable IDE0079
#pragma warning disable UNT0008

// Если используешь Object Pool — оставь как есть.
// Иначе можешь заменить на обычный MonoBehaviour.
public class Zombie : PooledBehaviour<Zombie>
{
    [Header("Refs")]
    [SerializeField] private NavMeshAgent _agent;
    [SerializeField] private ZombieHealthView _healthView; // опционально
    [SerializeField] private Animator _animator;

    [Header("Move")]
    [SerializeField] private float _walkSpeed = 1.5f;
    [SerializeField] private float _reachRadius = 0.25f;

    [Header("Ragdoll")]
    [SerializeField] private Rigidbody[] _bodies;
    [SerializeField] private ZombieHitScan[] _hitScans;

    // === Твои сервисы (инъекции опциональны; оставляю, как у тебя было) ===
    [Inject] private HealthModel _health;
    [Inject] private WaveService _waves;
    [Inject] private IMoney _moneyService;

    // === FSM ===
    private IZombieState _currentState;

    // Состояния (минимум: Spawn/Walk/Attack). Здесь храню только Walk.
    private ZombieWalkToBarricadeState _walkToBarricadeState;
    private ZombieChasePlayerState _chaseState;
    private IZombieState _attackBarricadeState; // задай снаружи, если уже есть

    // === Текущая цель баррикады ===
    private BarricadeSlots _currentSlots; // нужно, чтобы уметь освободить слот при смерти/деспауне

    public NavMeshAgent Agent => _agent;
    public System.Action<Zombie> Died;

    // ================= Mono =================

    private void Update()
    {
        _currentState?.Tick(this);
    }

    public override void OnSpawned()
    {
        foreach (Rigidbody rigidbody in _bodies)
        {
            rigidbody.isKinematic = true;
        }

        foreach (ZombieHitScan hitScan in _hitScans)
        {
            hitScan.enabled = true;
        }

        _animator.enabled = true;
        _agent.enabled = true;

        // ХП/визуал — по желанию
        if (_waves != null && _health != null) _health.Reset(_waves.CurrentBaseHP);

        _healthView?.Setup(_health);

        if (_agent != null)
        {
            _agent.speed = _walkSpeed;
            SetupAgentTuning();
        }

        EnsureOnNavMesh();
    }

    public override void OnDespawned()
    {
        // Освободить слот, если был
        TryReleaseBarricadeSlotIfAny();
        _currentSlots = null;

        _currentState = null;
        _walkToBarricadeState = null;

        _healthView?.OnDespawned();
    }

    public void JumpAnimation() => _animator.SetTrigger("Jump");

    // ================= FSM helpers =================

    public void ChangeState(IZombieState newState)
    {
        _currentState?.Exit(this);
        _currentState = newState;
        _currentState?.Enter(this);
    }

    // Вызови это, когда выбираешь баррикаду, к которой зомби должен подойти.
    public void GoToBarricade(BarricadeSlots slots)
    {
        _currentSlots = slots;
        _walkToBarricadeState = new ZombieWalkToBarricadeState(slots, this);
        ChangeState(_walkToBarricadeState);
        _animator.SetBool("Walk", true);
    }

    // Если у тебя есть готовое состояние атаки — можешь поставить его в этот зомби:
    public void SetAttackState(IZombieState attackState)
    {
        _attackBarricadeState = attackState;
    }

    // Коллбек из Walking-состояния: «я встал в свой слот»
    public void OnReachedBarricadeSlot()
    {
        if (_attackBarricadeState != null)
        {
            ChangeState(_attackBarricadeState);
        }
        else
        {
            // Заглушка: просто стоим, смотрим на баррикаду
            StopAndFace(_currentSlots != null
                ? _currentSlots.transform.position
                : transform.position + transform.forward * 2f);
        }
    }

    // ================= Навигация / утилиты =================

    private void SetupAgentTuning()
    {
        // Небольшая рандомизация — меньше локальных «пробок»
        _agent.stoppingDistance = 0.15f;
        _agent.avoidancePriority = Random.Range(20, 80);
        _agent.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        _agent.autoBraking = true;
        _agent.angularSpeed = Mathf.Max(120f, _agent.angularSpeed);
        _agent.acceleration = Mathf.Max(6f, _agent.acceleration);
    }


    private bool EnsureOnNavMesh(float maxSnap = 1.25f, int areaMask = NavMesh.AllAreas)
    {
        if (_agent == null || !_agent.enabled) return false;

        // В новых Unity есть _agent.isOnNavMesh — проверь, если доступно.
        // Универсально: пробуем семплить рядом с текущей позицией.
        if (NavMesh.SamplePosition(transform.position, out var hit, maxSnap, areaMask))
        {
            _agent.Warp(hit.position);  // принудительно «приклеиваемся»
            return true;
        }
        return false;
    }

    public void SetDestinationToPoint(Vector3 worldPoint)
    {
        if (_agent == null || !_agent.enabled) return;

        // если не на сетке — попробуем «приклеиться»
        if (!EnsureOnNavMesh())
        {
            // не смогли — не дергаем SetDestination, чтобы не ловить ошибку
            return;
        }

        _agent.isStopped = false;
        _agent.SetDestination(worldPoint);
    }

    public bool IsNear(Vector3 point, float radius)
    {
        var p = transform.position;
        float dx = p.x - point.x;
        float dz = p.z - point.z;
        return (dx * dx + dz * dz) <= radius * radius;
    }

    public bool IsNear(Vector3 point) => IsNear(point, _reachRadius);

    public void StopAndFace(Vector3 lookAt)
    {
        if (_agent != null && _agent.enabled)
        {
            _agent.ResetPath();
            _agent.isStopped = true;
        }

        Vector3 dir = (lookAt - transform.position);
        dir.y = 0f;
        if (dir.sqrMagnitude > 0.0001f)
        {
            transform.rotation = Quaternion.LookRotation(dir);
        }
    }

    public void BeginChase(Transform playerTransform, float repathInterval = 0.2f, float repathMoveThreshold = 0.4f)
    {
        if (playerTransform == null) return;

        _chaseState = new ZombieChasePlayerState(this, playerTransform, repathInterval, repathMoveThreshold);
        ChangeState(_chaseState);
    }

    // ================= Урон/смерть =================

    public void Damage(int dmg)
    {
        // Визуал может анимировать, потом дернуть Death().
        _healthView?.Damage(dmg, Death);
    }

    private void Death()
    {
        _animator.enabled = false;
        _agent.enabled = false;

        foreach(Rigidbody rigidbody in _bodies)
        {
            rigidbody.isKinematic = false;
        }

        foreach (ZombieHitScan hitScan in _hitScans)
        {
            hitScan.enabled = false;
        }

        _moneyService.AddMoney(Random.Range(100, 200));

        // ВАЖНО: освободить слот, чтобы другой мог занять центр/апгрейднуться
        TryReleaseBarricadeSlotIfAny();
        StartCoroutine(WaitReturnToPool());

        Died?.Invoke(this);
    }

    private IEnumerator WaitReturnToPool()
    {
        yield return new WaitForSeconds(2.5f);
        ReturnToPool(); // если без пула — Destroy(gameObject);
    }

    private void TryReleaseBarricadeSlotIfAny()
    {
        if (_currentSlots != null)
        {
            _currentSlots.Release(this);
        }
    }

    // ================= API совместимости (из твоего старого кода) =================

    // Если где-то ожидается "SetDestinationToTarget(Transform)" — оставим обёртку:
    public void SetDestinationToTarget(Transform target)
    {
        if (target == null) return;
        SetDestinationToPoint(target.position);
    }

    // Если где-то ожидается "SetSpawnPoint" — оставим:
    public void SetSpawnPoint(Transform point)
    {
        if (point == null) return;
        if (_agent != null) _agent.enabled = false;

        transform.SetPositionAndRotation(point.position, point.rotation);

        if (_agent != null)
        {
            _agent.enabled = true;
            // warp в текущую позицию, затем прижать к сетке
            _agent.Warp(transform.position);
            EnsureOnNavMesh();
        }
    }

    [ContextMenu("TEST_KILL")]
    private void TEST_KillMe()
    {
        Damage(999999);
    }
}