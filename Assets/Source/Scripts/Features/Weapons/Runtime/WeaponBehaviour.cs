using System.Collections;
using UnityEngine;

#pragma warning disable IDE0044

[RequireComponent(typeof(Animator)), RequireComponent(typeof(AudioSource))]
public class WeaponBehaviour : MonoBehaviour
{
    [Header("Config")]
    [SerializeField] private WeaponConfig _baseConfig;

    [Header("Components")]
    [SerializeField] private Transform _barrelTransform;
    [SerializeField] private Animator _animator;
    [SerializeField] private AudioSource _audioSource;

    [Header("Gizmo")]
    [SerializeField] private bool _showWire = false;
    [SerializeField] private Color _gizmoColor = Color.white;
    [SerializeField] private float _gizmoRadius = 0.1f;

    [Inject] private WeaponManager _weaponManager;

    private bool _canShoot = true;
    private bool _canReload = true;
    private bool _canScope = true;
    private bool _isReload = false;
    private bool _inScope = false;
    private bool _canSwitch = true;

    private WeaponConfig _config;
    private IWeaponContext _context;
    private WeaponEffects _effects;
    private WeaponFire _fire;
    private WeaponReload _reload;

    private void Awake()
    {
        _config = Instantiate(_baseConfig);

        _audioSource.playOnAwake = false;
        _animator.applyRootMotion = false;
        _animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;

        _context = new WeaponContextAdapter(_weaponManager);
        _effects = new WeaponEffects(_barrelTransform, _animator, _audioSource);
        _fire = new WeaponFire(_context, _effects, _config, _barrelTransform);
        _reload = new WeaponReload(_effects, _config);
    }

    private void OnEnable()
    {
        _context.SetAmmoText(BuildAmmoText());
        _effects.PlayOne(_config.EquipSound);
        StartCoroutine(SwitchDelay());
    }

    private void Update()
    {
        _context.Recoil.Tick(_config.RecoilReturnSpeed, _config.RecoilSnappiness);

        FireTick tick = new(
            fireHeld: Input.GetKey(KeyCode.Mouse0),
            firePressed: Input.GetKeyDown(KeyCode.Mouse0),
            aimHeld: Input.GetKey(KeyCode.Mouse1)
        );

        Aiming(tick.AimHeld);

        if (_canShoot && !_isReload && GateFire(tick))
        {
            bool shot = _fire.TryShoot(_inScope, this);

            if (!shot && Input.GetKeyDown(KeyCode.Mouse0))
            {
                _effects.PlayOne(_config.EmptyMagSound);
            }

            _context.SetAmmoText(BuildAmmoText());
        }

        if (_canReload && !_inScope && !_context.Player.IsSprinting)
        {
            bool wantReload = Input.GetKeyDown(KeyCode.R) || (_config.AvailableAmmo > 0 && _config.CurrentAmmo == 0);

            if (wantReload && _reload.CanStart(_inScope, _context.Player.IsSprinting))
            {
                StartCoroutine(DoReload());
            }
        }

        PlayLocomotionAnimations();
    }

    private bool GateFire(FireTick t)
    {
        if (_config.CurrentAmmo <= 0)
        {
            return t.FirePressed;
        }

        return _config.FireMode switch
        {
            FireMode.Auto => t.FireHeld,
            FireMode.Semi => t.FirePressed,
            FireMode.Shotgun => t.FirePressed,
            _ => false
        };
    }

    private void Aiming(bool aimHeld)
    {
        if (_context.Player.IsSprinting)
        {
            _inScope = false;
            _canScope = false;
        }
        else
        {
            _canScope = true;
            _inScope = _canScope && aimHeld;
        }

        _effects.SetAimBool(_config.AimBoolName, _inScope);

        CanvasGroup ch = _context.Crosshair.Group;

        if (_inScope)
        {
            if (ch.alpha > 0)
            {
                ch.alpha -= Time.deltaTime * 5f;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, _config.WeaponAimingPosition, Time.deltaTime * 20);
            _weaponManager.PlayerCamera.fieldOfView = Mathf.Lerp(_weaponManager.PlayerCamera.fieldOfView, _config.AimingFOV, Time.deltaTime * _config.AimingSmooth);
        }
        else 
        {
            if (ch.alpha < 1)
            {
                ch.alpha += Time.deltaTime * 5f;
            }

            transform.localPosition = Vector3.Lerp(transform.localPosition, _config.WeaponDefaultPosition, Time.deltaTime * 20);
            _weaponManager.PlayerCamera.fieldOfView = Mathf.Lerp(_weaponManager.PlayerCamera.fieldOfView, _config.DefaultFOV, Time.deltaTime * _config.AimingSmooth);
        }
    }

    private void PlayLocomotionAnimations()
    {
        bool walking = _context.Player.IsWalking;
        bool sprint = _context.Player.IsSprinting;

        _animator.SetBool("Walk", walking);
        _animator.SetBool("Run", sprint && !_weaponManager.PlayerController.IsCrouching);
        _animator.SetBool("Aim Walk", walking && _inScope);

        if (sprint)
        {
            _canReload = false; _canShoot = false; _canScope = false; _inScope = false;
        }
        else if (!_isReload)
        {
            _canReload = true; _canShoot = true; _canScope = true;
        }
    }

    private IEnumerator DoReload()
    {
        _canShoot = _canScope = _canReload = false;
        _isReload = true;
        _context.Switcher.CanSwitch = false;
        _canSwitch = false;

        yield return _reload.Run(this);

        _canShoot = _canScope = _canReload = true;
        _isReload = false;
        _context.Switcher.CanSwitch = true;
        _canSwitch = true;

        _context.SetAmmoText(BuildAmmoText());
        _reload.RefillMagazine();
    }

    private IEnumerator SwitchDelay()
    {
        _canSwitch = false;

        yield return new WaitForSeconds(_config.SwitchDelay);

        _canSwitch = true;
    }

    public IEnumerator SwitchWeapon(WeaponBehaviour selectableWeapon)
    {
        if (selectableWeapon == this)
        {
            yield break;
        }

        if (!_canSwitch)
        {
            yield break;
        }

        _animator.Play(_config.HideName);

        yield return new WaitForSeconds(_config.SwitchDelay);

        _context.Switcher.Select(selectableWeapon);
    }

    public IEnumerator HideWeapon()
    {
        _animator.Play(_config.HideName);

        _canReload = false;
        _canShoot = false;
        _canSwitch = false;
        _canScope = false;

        _weaponManager.InteractDetector.CanInteract = false;

        yield return new WaitForSeconds(_config.SwitchDelay);

        gameObject.SetActive(false);
    }

    public IEnumerator DrawWeapon()
    {
        gameObject.SetActive(true);

        yield return null;

        _canReload = true;
        _canShoot = true;
        _canSwitch = true;
        _canScope = true;

        _weaponManager.InteractDetector.CanInteract = true;
    }

    private string BuildAmmoText()
    {
        string current = _config.CurrentAmmo > 0
            ? $"<color=#FFE5BF>{Utils.FormatNumber(_config.CurrentAmmo, '.')}</color>\\"
            : $"<color=#FF0000>{Utils.FormatNumber(Mathf.Max(0, _config.CurrentAmmo), '.')}</color>\\";

        string available = _config.AvailableAmmo > 0
            ? $"<size=22><color=#FFFFFF>{Utils.FormatNumber(_config.AvailableAmmo, '.')}</color></size>"
            : $"<size=22><color=#FF0000>{Utils.FormatNumber(Mathf.Max(0, _config.AvailableAmmo), '.')}</color></size>";

        return current + available;
    }

    private void OnDrawGizmos()
    {
        if (_barrelTransform == null)
        {
            return;
        }

        Gizmos.color = _gizmoColor;

        if (_showWire)
        {
            Gizmos.DrawWireSphere(_barrelTransform.position, _gizmoRadius);
        }
        else
        {
            Gizmos.DrawSphere(_barrelTransform.position, _gizmoRadius);
        }
    }

    public string GetName() => _config.WeaponName;
    public Sprite GetWeaponIcon() => _config.WeaponIcon;

    public void AddAmmoFromAmmoBox()
    {
        _config.AvailableAmmo += _config.AmmoBoxCount;
        _context.SetAmmoText(BuildAmmoText());
    }
}
