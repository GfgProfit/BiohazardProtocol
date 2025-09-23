using UnityEngine;

[CreateAssetMenu(menuName = "Weapon/Config", fileName = "Weapon Config")]
public class WeaponConfig : ScriptableObject
{
    [SerializeField] private string _weaponName = "New Weapon";
    public string WeaponName => _weaponName;

    [SerializeField] private Sprite _weaponIcon;
    public Sprite WeaponIcon => _weaponIcon;

    [SerializeField] private FireMode _fireMode = FireMode.Auto;
    public FireMode FireMode => _fireMode;

    [SerializeField] private LayerMask _hitScanMask;
    public LayerMask HitScanMask => _hitScanMask;

    [SerializeField] private Vector3 _weaponDefaultPosition;
    public Vector3 WeaponDefaultPosition => _weaponDefaultPosition;

    [SerializeField] private Vector3 _weaponAimingPosition;
    public Vector3 WeaponAimingPosition => _weaponAimingPosition;

    [SerializeField] private int _damage = 10;
    public int Damage => _damage;

    [SerializeField] private int _fireRate = 600;
    public int FireRate => _fireRate;

    [SerializeField] private int _shotgunPellets = 8;
    public int ShotgunPellets => _shotgunPellets;

    [Header("Switch")]
    [SerializeField] private float _switchDelay = 0.5f;
    public float SwitchDelay => _switchDelay;

    [Header("Aiming")]
    [SerializeField] private float _defaultFOV = 60;
    public float DefaultFOV => _defaultFOV;

    [SerializeField] private float _aimingFOV = 50;
    public float AimingFOV => _aimingFOV;

    [SerializeField] private float _aimingSmooth = 10;
    public float AimingSmooth => _aimingSmooth;

    [Header("Ammo")]
    [SerializeField, Range(0, 200)] private int _magSize = 30;
    public int MagSize { get { return _magSize; } set { _magSize = value; } }

    [SerializeField, Range(0, 200)] private int _currentAmmo = 30;
    public int CurrentAmmo { get { return _currentAmmo; } set { _currentAmmo = value; } }

    [SerializeField, Range(0, 600)] private int _availableAmmo = 90;
    public int AvailableAmmo { get { return _availableAmmo; } set { _availableAmmo = value; } }

    [SerializeField] private int _ammoBoxCount = 250;
    public int AmmoBoxCount => _ammoBoxCount;

    [Header("Animation")]
    [SerializeField] private string _shootName = "Shoot";
    public string ShootName => _shootName;

    [SerializeField] private string _aimShootName = "Aim Shoot";
    public string AimShootName => _aimShootName;

    [SerializeField] private string _reloadBoolName = "Reload";
    public string ReloadBoolName => _reloadBoolName;

    [SerializeField] private string _reloadFullBoolName = "Reload Full";
    public string ReloadFullBoolName => _reloadFullBoolName;

    [SerializeField] private string _aimBoolName = "Aim";
    public string AimBoolName => _aimBoolName;

    [SerializeField] private string _hideName = "Hide";
    public string HideName => _hideName;

    [Space]
    [SerializeField, Range(0.0f, 10.0f)] private float _reloadTime = 2.0f;
    public float ReloadTime => _reloadTime;

    [SerializeField, Range(0.0f, 15.0f)] private float _reloadFullTime = 3.0f;
    public float ReloadFullTime => _reloadFullTime;

    [Header("Sound")]
    [SerializeField] private AudioClip _emptyMagSound;
    public AudioClip EmptyMagSound => _emptyMagSound;

    [SerializeField] private AudioClip _reloadSound;
    public AudioClip ReloadSound => _reloadSound;

    [SerializeField] private AudioClip _fullReloadSound;
    public AudioClip FullReloadSound => _fullReloadSound;

    [SerializeField] private AudioClip _equipSound;
    public AudioClip EquipSound => _equipSound;

    [SerializeField] private AudioClip _hideSound;
    public AudioClip HideSound => _hideSound;

    [Space]
    [SerializeField] private AudioClip[] _shootSounds;
    public AudioClip[] ShootSounds => _shootSounds;

    [Header("Muzzle Flash")]
    [SerializeField] private bool _enableMuzzle = true;
    public bool EnableMuzzle => _enableMuzzle;

    [SerializeField, Range(0.0f, 2.0f)] private float _muzzleScaleFactor = 1.0f;
    public float MuzzleScaleFactor => _muzzleScaleFactor;

    [SerializeField, Range(0.0f, 5.0f)] private float _muzzleDestroyTime = 2.0f;
    public float MuzzleDestroyTime => _muzzleDestroyTime;

    [SerializeField] private GameObject[] _muzzlePrefabs;
    public GameObject[] MuzzlePrefabs => _muzzlePrefabs;

    [Header("Recoil")]
    [SerializeField] private Vector3 _recoilPattern = new(1.5f, 2.0f, 1.0f);
    public Vector3 RecoilPattern => _recoilPattern;

    [SerializeField, Range(0.0f, 50.0f)] private float _recoilSnappiness = 2.0f;
    public float RecoilSnappiness => _recoilSnappiness;

    [SerializeField, Range(0.0f, 50.0f)] private float _recoilReturnSpeed = 5.0f;
    public float RecoilReturnSpeed => _recoilReturnSpeed;

    [Header("Spread")]
    [SerializeField] private bool _applySpread = true;
    public bool ApplySpread => _applySpread;

    [SerializeField] private Vector3 _spreadVariance = new(1.0f, 1.0f, 1.0f);
    public Vector3 SpreadVariance => _spreadVariance;

    [SerializeField] private float _bulletSpeed = 100;
    public float BulletSpeed => _bulletSpeed;

    [Header("Trail")]
    [SerializeField] private AnimationCurve _widthCurve;
    public AnimationCurve WidthCurve => _widthCurve;

    [SerializeField] private float _duration = 0.1f;
    public float Duration => _duration;

    [SerializeField] private float _minVertexDistance = 0.1f;
    public float MinVertexDistance => _minVertexDistance;

    [SerializeField] private Gradient _trailColor;
    public Gradient TrailColor => _trailColor;

    [SerializeField] private Material _material;
    public Material Material => _material;

    [SerializeField] private TrailRenderer _bulletTrail;
    public TrailRenderer BulletTrail => _bulletTrail;
}