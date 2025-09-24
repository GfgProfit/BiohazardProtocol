using DG.Tweening;
using UnityEngine;

#pragma warning disable IDE0044

public class WorldWeaponInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private Transform _worldViewWeapon;
    [SerializeField] private WeaponBehaviour _weaponPrefab;

    [Space]
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
            AnimatePingPongPopUp();

            GameObject gameObject = Instantiate(_weaponPrefab.gameObject, _weaponManager.SwitchWeapon.transform);
            SceneInstaller.Container.InjectGameObject(gameObject, true);
            gameObject.SetActive(true);
            _weaponManager.SwitchWeapon.SwapWeapon(gameObject.GetComponent<WeaponBehaviour>());
        }
    }

    private void AnimatePingPongPopUp()
    {
        _worldViewWeapon.DOScale(0.9f, 0.1f)
               .OnComplete(() => _worldViewWeapon.DOScale(1.0f, 0.1f))
               .SetEase(Ease.OutBack);
    }
}