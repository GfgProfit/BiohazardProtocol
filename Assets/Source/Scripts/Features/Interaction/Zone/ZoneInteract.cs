using UnityEngine;

#pragma warning disable IDE0044

public class ZoneInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private ZoneObject _zoneObject;

    [Space]
    [SerializeField] private string _zoneName;
    [SerializeField] private int _money;

    public string Text { get => _zoneName; set { } }
    public int Money { get => _money; set { } }

    [Inject] private IMoney _moneyService;

    public void Interact()
    {
        bool spended = _moneyService.SpendMoney(Money);

        if (spended)
        {
            _zoneObject.Interact();
        }
    }
}