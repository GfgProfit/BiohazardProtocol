using System.Collections;
using UnityEngine;

public sealed class WeaponReload
{
    private readonly WeaponEffects _effects;
    private readonly WeaponConfig _config;
    private readonly WeaponManager _weaponManager;

    public WeaponReload(WeaponEffects fx, WeaponConfig cfg, WeaponManager weaponManager)
    {
        _effects = fx;
        _config = cfg;
        _weaponManager = weaponManager;
    }

    public bool CanStart(bool inScope, bool playerSprinting)
    {
        if (inScope)
        {
            return false;
        }

        if (playerSprinting)
        {
            return false;
        }

        if (_config.AvailableAmmo <= 0)
        {
            return false;
        }

        if (_config.CurrentAmmo >= _config.MagSize)
        {
            return false;
        }

        return true;
    }

    public IEnumerator Run(MonoBehaviour host)
    {
        bool empty = _config.CurrentAmmo == 0;

        if (empty)
        {
            _effects.MoveStateBool(_config.ReloadFullBoolName, true);
            _effects.PlayOne(_config.FullReloadSound);

            float speed = _weaponManager.SpeedColaActive ? _weaponManager.SpeedColaMultiplier : 1;

            float reloadSpeed = _weaponManager.SpeedColaActive ? _config.ReloadFullTime / _weaponManager.SpeedColaMultiplier : _config.ReloadFullTime;

            _effects.SetSpeedMultiplier(speed);

            yield return new WaitForSeconds(reloadSpeed);

            _effects.MoveStateBool(_config.ReloadFullBoolName, false);

            _effects.SetSpeedMultiplier(1);
        }
        else
        {
            _effects.MoveStateBool(_config.ReloadBoolName, true);
            _effects.PlayOne(_config.ReloadSound);

            float speed = _weaponManager.SpeedColaActive ? _weaponManager.SpeedColaMultiplier : 1;

            float reloadSpeed = _weaponManager.SpeedColaActive ? _config.ReloadTime / _weaponManager.SpeedColaMultiplier : _config.ReloadTime;

            _effects.SetSpeedMultiplier(speed);

            yield return new WaitForSeconds(reloadSpeed);

            _effects.MoveStateBool(_config.ReloadBoolName, false);

            _effects.SetSpeedMultiplier(1);
        }

        RefillMagazine();
    }

    public void RefillMagazine()
    {
        int need = Mathf.Clamp(_config.MagSize - _config.CurrentAmmo, 0, _config.MagSize);
        int take = Mathf.Min(need, _config.AvailableAmmo);

        _config.CurrentAmmo += take;
        _config.AvailableAmmo -= take;
    }
}