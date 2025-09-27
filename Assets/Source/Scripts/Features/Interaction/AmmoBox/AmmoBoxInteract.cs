using DG.Tweening;
using UnityEngine;

#pragma warning disable IDE0044

public class AmmoBoxInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private WaveController _waveController;

    [Header("UI/Price")]
    [SerializeField] private string _text;
    [SerializeField] private int _money;
    [SerializeField] private bool _canInteract = true;

    [Header("Wave gating")]
    [SerializeField] private int _repeatEveryWaves = 3;
    [SerializeField] private int _firstAllowedWave = 1;

    [Header("Blocked settings")]
    [SerializeField] private string _blockedText = "<color=red>Can't Buy</color>";

    public string Text { get => _text; set { } }
    public int Money { get => _money; set { } }
    public bool CanInteract { get => _canInteract; set { _canInteract = value; } }

    [Inject] private IMoney _moneyService;
    [Inject] private WeaponManager _weaponManager;

    private string _defaultText;
    private int _lastPurchaseWave = int.MinValue;

    private void Awake()
    {
        _defaultText = _text;
    }

    private void Update()
    {
        int wave = _waveController.GetCurrentWaveIndex;

        bool isRightWave = wave >= _firstAllowedWave && ((wave - _firstAllowedWave) % _repeatEveryWaves == 0);
        bool notAlreadyBoughtThisWave = wave != _lastPurchaseWave;
        bool available = isRightWave && notAlreadyBoughtThisWave;

        _canInteract = available;

        if (available)
        {
            _text = _defaultText;
        }
        else
        {
            _text = _blockedText;
        }
    }

    public void Interact()
    {
        if (!CanInteract) return;

        int wave = _waveController.GetCurrentWaveIndex;
        bool spent = _moneyService.SpendMoney(Money);

        if (spent)
        {
            _weaponManager.AddAmmoFromAmmoBox();
            _lastPurchaseWave = wave;

            _canInteract = false;
            _text = _blockedText;

            AnimatePingPongPopUp();
        }
        else
        {
            DenyNudge();
        }
    }

    private void AnimatePingPongPopUp()
    {
        transform.DOScale(0.9f, 0.1f)
            .OnComplete(() => transform.DOScale(1.0f, 0.1f))
            .SetEase(Ease.OutBack);
    }

    private void DenyNudge()
    {
        transform.DOKill();
        Vector3 original = transform.localScale;
        transform.DOScale(original * 0.97f, 0.06f)
            .OnComplete(() => transform.DOScale(original, 0.06f));
    }
}
