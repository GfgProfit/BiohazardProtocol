using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BarricadeController : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private BarricadeSlots _slots;
    [SerializeField] private Transform _spawnPoint;
    [SerializeField] private Transform _pointToBarricade;

    [Header("Jump path")]
    [SerializeField] private Transform _point1;
    [SerializeField] private Transform _point2;
    [SerializeField] private Transform _point3;
    [SerializeField] private Transform _point4;

    [Header("Planks (0 — первая ломается)")]
    [SerializeField] private PlankView[] _plankViews;

    private int _nextPlankIndex = 0;

    private readonly Queue<Zombie> _climbQueue = new();
    private readonly HashSet<Zombie> _queued = new();
    private readonly HashSet<Zombie> _climbed = new();
    private Zombie _currentClimber = null;
    private bool _isClimbing = false;

    public Transform SpawnPoint => _spawnPoint;
    public Transform PointToBarricade => _pointToBarricade;
    public BarricadeSlots Slots => _slots;

    public bool IsOpen => _nextPlankIndex >= (_plankViews?.Length ?? 0);
    public int RemainingPlanks => Mathf.Max(0, (_plankViews?.Length ?? 0) - _nextPlankIndex);

    private void Awake()
    {
        if (_slots == null) _slots = GetComponent<BarricadeSlots>();
    }

    [ContextMenu("Get Planks")]
    private void GetPlanks()
    {
        _plankViews = GetComponentsInChildren<PlankView>(includeInactive: true);
    }

    public void BuildPlank()
    {
        _nextPlankIndex--;
        PlankView pv = _plankViews[_nextPlankIndex];
        pv.gameObject.SetActive(true);
        pv.SetBroken(false);

        pv.transform.DOLocalMove(pv.UpPoint.localPosition, 0.15f).SetEase(Ease.InCubic)
            .OnComplete(() =>
            {
                pv.transform.DOLocalMove(pv.RepairedPosition, 0.15f).SetEase(Ease.OutCubic);
            });
    }

    public bool TryHitOnePlank()
    {
        if (IsOpen) return false;

        PlankView pv = _plankViews[_nextPlankIndex];
        if (pv != null && pv.gameObject.activeSelf)
        {
            pv.Break();
        }

        _nextPlankIndex++;
        return true;
    }

    public void RequestClimb(Zombie z)
    {
        if (!IsOpen || z == null) return;

        if (_climbed.Contains(z)) return;

        if (_queued.Contains(z) || _currentClimber == z) return;

        if (_slots != null) _slots.Release(z);

        _queued.Add(z);
        _climbQueue.Enqueue(z);
        TryProcessClimbQueue();
    }

    private void TryProcessClimbQueue()
    {
        if (_isClimbing) return;
        if (_climbQueue.Count == 0) return;

        _currentClimber = _climbQueue.Dequeue();
        _queued.Remove(_currentClimber);

        _isClimbing = true;
        JumpOverBarricade(_currentClimber, () =>
        {
            _climbed.Add(_currentClimber);

            _currentClimber = null;
            _isClimbing = false;

            TryProcessClimbQueue();
        });
    }

    private void JumpOverBarricade(Zombie target, System.Action onDone)
    {
        if (target == null) { onDone?.Invoke(); return; }

        target.Agent.enabled = false;

        Vector3[] waypoints = {
            _point1.position, _point2.position, _point3.position, _point4.position
        };

        target.JumpAnimation();
        target.transform.DOPath(waypoints, 0.95f, PathType.CatmullRom, PathMode.Full3D)
            .SetEase(Ease.InOutSine)
            .OnComplete(() =>
            {
                target.transform.position = _point4.position;

                target.Agent.enabled = true;

                // аккуратно приклеиться: семплим рядом с point4
                if (NavMesh.SamplePosition(target.transform.position, out var hit, 1.5f, NavMesh.AllAreas))
                {
                    target.Agent.Warp(hit.position);
                }
                else
                {
                    Debug.LogWarning("[Barricade] Не удалось найти NavMesh рядом с точкой приземления.");
                }

                if (_pointToBarricade != null)
                {
                    target.BeginChase(_pointToBarricade);
                }
                else if (_pointToBarricade != null)
                {
                    target.SetDestinationToTarget(_pointToBarricade);
                }

                onDone?.Invoke();
            });
    }

    public void NotifyZombieGone(Zombie z)
    {
        if (z == null) return;

        _queued.Remove(z);
        _climbed.Remove(z);

        if (_currentClimber == z)
        {
            _currentClimber = null;
            _isClimbing = false;
            TryProcessClimbQueue();
        }
    }
}
