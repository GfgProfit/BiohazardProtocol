using TMPro;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [field: SerializeField] public TMP_Text AmmoText { get; private set; }
    [field: SerializeField] public PlayerController PlayerController { get; private set; }
    [field: SerializeField] public WeaponSwitch SwitchWeapon { get; private set; }
    [field: SerializeField] public WeaponRecoilManager WeaponRecoilManager { get; private set; }
    [field: SerializeField] public CanvasGroup Crosshair { get; private set; }
    [field: SerializeField] public Transform ShootTransform { get; private set; }
    [field: SerializeField] public Camera PlayerCamera { get; private set; }
    [field: SerializeField] public CrosshairController CrosshairController { get; private set; }
    [field: SerializeField] public InteractDetector InteractDetector { get; private set; }

    public bool SpeedColaActive { get; set; } = false;
    public float SpeedColaMultiplier { get; set; } = 0;

    public bool DoubleTapActive { get; set; } = false;
    public float DoubleTapDamageMultiplier { get; set; } = 0;
    public float DoubleTapFireRateMultiplier { get; set; } = 0;

    public void AddAmmoFromAmmoBox()
    {
        SwitchWeapon.GetCurrentWeapon().AddAmmoFromAmmoBox();
    }
}