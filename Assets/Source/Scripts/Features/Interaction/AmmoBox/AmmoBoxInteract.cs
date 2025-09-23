using DG.Tweening;
using UnityEngine;

#pragma warning disable IDE0044

public class AmmoBoxInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private string _zoneName;
    [SerializeField] private int _money;
    [SerializeField] private bool _canInteract = true;

    public string Text { get => _zoneName; set { } }
    public int Money { get => _money; set { } }
    public bool CanInteract { get => _canInteract; set { _canInteract = value; } }

    [Inject] private IMoney _moneyService;
    [Inject] private WeaponManager _weaponManager;

    public void Interact()
    {
        bool spended = _moneyService.SpendMoney(Money);

        if (spended)
        {
            _weaponManager.AddAmmoFromAmmoBox();
            AnimatePingPongPopUp();
        }
    }

    private void AnimatePingPongPopUp()
    {
        transform.DOScale(0.9f, 0.1f)
               .OnComplete(() => transform.DOScale(1.0f, 0.1f))
               .SetEase(Ease.OutBack);
    }
}
