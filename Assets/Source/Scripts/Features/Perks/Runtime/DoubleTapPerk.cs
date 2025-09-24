using UnityEngine;

public class DoubleTapPerk : PerkAbstract
{
    [SerializeField] private float _damageMultiplier = 1.3f;
    [SerializeField] private float _fireRateMultiplier = 1.2f;

    [Inject] private WeaponManager _weaponManager;

    public override void Use()
    {
        _weaponManager.DoubleTapActive = true;
        _weaponManager.DoubleTapDamageMultiplier = _damageMultiplier;
        _weaponManager.DoubleTapFireRateMultiplier = _fireRateMultiplier;
    }
}