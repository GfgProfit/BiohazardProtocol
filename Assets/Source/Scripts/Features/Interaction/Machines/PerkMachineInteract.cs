using System.Collections;
using UnityEngine;

#pragma warning disable IDE0044

public class PerkMachineInteract : MonoBehaviour, IInteractable
{
    [SerializeField] private MeshRenderer _meshRenderer;
    [SerializeField] private Material _machineWorkingMaterial;
    [SerializeField] private Light _machineWorkingLight;
    [SerializeField] private ObjectShaker _objectShaker;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private SodaCan _sodaCanPrefab;
    [SerializeField] private Material _sodaCanMaterial;

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
            _meshRenderer.sharedMaterial = _machineWorkingMaterial;
            _machineWorkingLight.enabled = true;
            _objectShaker.enabled = true;
            CanInteract = false;
            _audioSource.Play();
            StartCoroutine(Use());

        }
    }

    public IEnumerator Use()
    {
        yield return _weaponManager.SwitchWeapon.HideWeapon();

        SodaCan sodaCan = Instantiate(_sodaCanPrefab, _weaponManager.PlayerCamera.transform);

        sodaCan.ChangeMaterial(_sodaCanMaterial);

        yield return new WaitForSeconds(2.5f);

        yield return _weaponManager.SwitchWeapon.DrawWeapon();

        Destroy(sodaCan.gameObject);
    }
}