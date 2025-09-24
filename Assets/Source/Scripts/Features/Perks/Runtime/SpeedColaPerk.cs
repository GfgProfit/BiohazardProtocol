using UnityEngine;

public class SpeedColaPerk : PerkAbstract
{
    [SerializeField] private float _speedMultiplier = 1.3f; // 30%

    [Inject] private WeaponManager _weaponManager;

    public override void Use()
    {
        _weaponManager.SpeedColaActive = true;
        _weaponManager.SpeedColaMultiplier = _speedMultiplier;
    }
}