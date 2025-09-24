using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[DisallowMultipleComponent]
public class BarricadeSlots : MonoBehaviour
{
    [Header("Slots Arc")]
    [Range(1, 64)] public int slotCount = 8;
    [Min(0.1f)] public float slotRadius = 1.25f;
    [Range(10f, 340f)] public float slotArcDegrees = 140f;
    public Vector3 localCenterOffset = new(0, 0, 0.25f);

    [Header("Waiting Arc")]
    [Min(0.1f)] public float waitRadius = 2.5f;
    [Range(10f, 340f)] public float waitArcDegrees = 140f;

    [Header("Behaviour")]
    public float retryReserveInterval = 0.4f;

    [Header("NavMesh Projection")]
    [SerializeField] private bool _projectToNavMesh = true;
    [SerializeField] private float _sampleMaxDistance = 1.0f;
    [SerializeField] private int _areaMask = NavMesh.AllAreas;

    private struct Slot
    {
        public Vector3 worldPos;
        public Zombie owner;
    }

    private Slot[] _slots;
    private int[] _priorityOrder;
    private readonly HashSet<Zombie> _waiting = new();

    public float RetryReserveInterval => retryReserveInterval;

    private void Awake()
    {
        BuildAll();
    }

    private void OnValidate()
    {
        slotCount = Mathf.Max(1, slotCount);
        slotRadius = Mathf.Max(0.1f, slotRadius);
        waitRadius = Mathf.Max(0.1f, waitRadius);
        slotArcDegrees = Mathf.Clamp(slotArcDegrees, 10f, 340f);
        waitArcDegrees = Mathf.Clamp(waitArcDegrees, 10f, 340f);
        BuildAll();
    }

    private void BuildAll()
    {
        BuildSlots();
        BuildPriorityOrder();
    }

    private void BuildSlots()
    {
        _slots = new Slot[slotCount];

        Vector3 center = transform.TransformPoint(localCenterOffset);
        Vector3 baseDirSlots = -transform.forward;

        for (int i = 0; i < slotCount; i++)
        {
            float t = (slotCount == 1) ? 0.5f : i / (float)(slotCount - 1);
            float angleDeg = -slotArcDegrees * 0.5f + slotArcDegrees * t;

            Quaternion rot = Quaternion.AngleAxis(angleDeg, Vector3.up);
            Vector3 dir = rot * baseDirSlots;
            Vector3 world = center + dir.normalized * slotRadius;

            world = SnapToNavMesh(world);

            _slots[i].worldPos = world;
            _slots[i].owner = null;
        }
    }

    private Vector3 SnapToNavMesh(Vector3 world)
    {
        if (!_projectToNavMesh)
        {
            return world;
        }

        if (NavMesh.SamplePosition(world, out var hit, _sampleMaxDistance, _areaMask))
        {
            return hit.position;
        }

        return world;
    }

    private void BuildPriorityOrder()
    {
        _priorityOrder = new int[slotCount];
        int mid = GetCentralIndex();
        int idx = 0;
        _priorityOrder[idx++] = mid;

        int left = mid - 1, right = mid + 1;

        while (idx < slotCount)
        {
            if (left >= 0)
            {
                _priorityOrder[idx++] = left--;
            }

            if (idx >= slotCount)
            {
                break;
            }

            if (right < slotCount)
            {
                _priorityOrder[idx++] = right++;
            }
        }
    }

    public int GetCentralIndex() => slotCount / 2;

    public bool TryReserve(Zombie z, out int slotIndex, out Vector3 pos)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].owner == z)
            {
                slotIndex = i;
                pos = _slots[i].worldPos;
                return true;
            }
        }

        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].owner == null)
            {
                _slots[i].owner = z;
                slotIndex = i;
                pos = _slots[i].worldPos;
                return true;
            }
        }

        slotIndex = -1;
        pos = default;
        return false;
    }

    public bool TryReserveBest(Zombie z, out int slotIndex, out Vector3 pos)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].owner == z)
            {
                slotIndex = i;
                pos = _slots[i].worldPos;
                return true;
            }
        }

        foreach (int i in _priorityOrder)
        {
            if (_slots[i].owner == null)
            {
                _slots[i].owner = z;
                slotIndex = i;
                pos = _slots[i].worldPos;
                return true;
            }
        }

        slotIndex = -1;
        pos = default;
        return false;
    }

    public bool TryUpgrade(Zombie z, int currentSlot, out int newSlot, out Vector3 pos)
    {
        if (currentSlot < 0 || currentSlot >= slotCount)
        {
            newSlot = -1;
            pos = default;
            return false;
        }

        int curRank = -1;

        for (int r = 0; r < _priorityOrder.Length; r++)
        {
            if (_priorityOrder[r] == currentSlot)
            {
                curRank = r;
                break;
            }
        }

        if (curRank <= 0)
        {
            newSlot = -1;
            pos = default;
            return false;
        }

        for (int r = 0; r < curRank; r++)
        {
            int candidate = _priorityOrder[r];

            if (_slots[candidate].owner == null)
            {
                _slots[currentSlot].owner = null;
                _slots[candidate].owner = z;
                newSlot = candidate;
                pos = _slots[candidate].worldPos;
                return true;
            }
        }

        newSlot = -1;
        pos = default;
        return false;
    }

    public void Release(Zombie z)
    {
        for (int i = 0; i < _slots.Length; i++)
        {
            if (_slots[i].owner == z)
            {
                _slots[i].owner = null;
                break;
            }
        }

        _waiting.Remove(z);
    }

    public Vector3 GetSlotPosition(int index)
    {
        index = Mathf.Clamp(index, 0, _slots.Length - 1);

        return _slots[index].worldPos;
    }

    public bool IsOwner(Zombie z, int index)
    {
        if (index < 0 || index >= _slots.Length)
        {
            return false;
        }

        return _slots[index].owner == z;
    }

    public Vector3 GetWaitingPointFor(Zombie z)
    {
        _waiting.Add(z);

        int myOrder = 0;
        
        foreach (var w in _waiting)
        {
            if (w == z)
            {
                break;
            }

            myOrder++;
        }

        int count = Mathf.Max(1, _waiting.Count);
        float step = (count == 1) ? 0f : waitArcDegrees / (count - 1);
        float start = -waitArcDegrees * 0.5f;
        float angleDeg = start + step * myOrder;

        Vector3 center = transform.TransformPoint(localCenterOffset);
        Quaternion rot = Quaternion.AngleAxis(angleDeg, Vector3.up);
        Vector3 dir = rot * transform.forward;

        return center + dir.normalized * waitRadius;
    }

    private void OnDrawGizmos()
    {
        if (_slots == null || _slots.Length != slotCount)
        {
            BuildAll();
        }

        Vector3 center = transform.TransformPoint(localCenterOffset);

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(center, 0.05f);

        Gizmos.color = Color.green;
        Vector3 leftSlots = Quaternion.AngleAxis(-slotArcDegrees * 0.5f, Vector3.up) * (-transform.forward);
        Vector3 rightSlots = Quaternion.AngleAxis(+slotArcDegrees * 0.5f, Vector3.up) * (-transform.forward);
        Gizmos.DrawLine(center, center + leftSlots.normalized * slotRadius);
        Gizmos.DrawLine(center, center + rightSlots.normalized * slotRadius);

        for (int i = 0; i < _slots.Length; i++)
        {
            Gizmos.color = _slots[i].owner == null ? new Color(0f, 1f, 0f, 0.95f) : new Color(1f, 0f, 0f, 0.95f);
            Gizmos.DrawSphere(_slots[i].worldPos, 0.08f);
        }

        Gizmos.color = new Color(1f, 0.65f, 0f, 0.9f);
        Vector3 wLeft = Quaternion.AngleAxis(-waitArcDegrees * 0.5f, Vector3.up) * (transform.forward);
        Vector3 wRight = Quaternion.AngleAxis(+waitArcDegrees * 0.5f, Vector3.up) * (transform.forward);
        Gizmos.DrawLine(center, center + wLeft.normalized * waitRadius);
        Gizmos.DrawLine(center, center + wRight.normalized * waitRadius);
    }
}