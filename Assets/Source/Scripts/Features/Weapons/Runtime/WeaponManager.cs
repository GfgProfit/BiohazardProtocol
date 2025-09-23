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

    public void AddAmmoFromAmmoBox()
    {
        SwitchWeapon.GetCurrentWeapon().AddAmmoFromAmmoBox();
    }
}