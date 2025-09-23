using UnityEngine;

#pragma warning disable IDE0044

public class ZoneInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private ZoneObject _zoneObject;
    [SerializeField] private ZoneGate _zoneGate;

    [Space]
    [SerializeField] private string _zoneName;
    [SerializeField] private int _money;
    [SerializeField] private bool _canInteract = true;

    public string Text { get => _zoneName; set { } }
    public int Money { get => _money; set { } }
    public bool CanInteract { get => _canInteract; set { _canInteract = value; } }

    [Inject] private IMoney _moneyService;

    public void Interact()
    {
        if (_zoneGate != null && !_zoneGate.CanUnlock())
        {
            return;
        }

        if (!_moneyService.SpendMoney(Money))
        {
            return;
        }

        _zoneObject?.Interact();

        if (_zoneGate != null)
        {
            bool opened = _zoneGate.Unlock();
            if (!opened)
            {
                Debug.LogWarning("Unlock() вернул false — странно, раз CanUnlock был true. Проверь конфиг.");
            }
        }
    }
}
