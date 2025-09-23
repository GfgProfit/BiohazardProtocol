using System.Linq;
using UnityEngine;

public sealed class ZoneGate : MonoBehaviour
{
    [Header("Prerequisites")]
    [SerializeField] private string[] _requiredAll;
    [SerializeField] private string[] _requiredAny;

    [Header("Unlock Effect")]
    [SerializeField] private string[] _opensOnUnlock;
    [SerializeField] private bool _oneTime = true;

    [Header("Optional")]
    [SerializeField] private bool _openOnStart;

    [Inject] private IZoneService _zones;

    public bool IsUnlocked { get; private set; }

    private void Start()
    {
        if (_openOnStart)
        {
            _zones.OpenMany(_opensOnUnlock);
            IsUnlocked = true;
        }
    }

    public bool CanUnlock()
    {
        if (_oneTime && IsUnlocked)
        {
            return false;
        }

        bool allOk = _requiredAll == null || _requiredAll.Length == 0 || _requiredAll.All(z => _zones.IsOpen(z));

        bool anyOk = _requiredAny == null || _requiredAny.Length == 0 || _requiredAny.Any(z => _zones.IsOpen(z));

        if ((_requiredAll == null || _requiredAll.Length == 0) && (_requiredAny == null || _requiredAny.Length == 0))
        {
            return true;
        }

        return allOk && anyOk;
    }

    public bool Unlock()
    {
        if (!CanUnlock())
        {
            return false;
        }

        _zones.OpenMany(_opensOnUnlock);
        IsUnlocked = true;

        return true;
    }

    public string[] GetOpens() => _opensOnUnlock ?? System.Array.Empty<string>();
    public (string[] all, string[] any) GetRequirements() => (_requiredAll ?? System.Array.Empty<string>(), _requiredAny ?? System.Array.Empty<string>());
}
