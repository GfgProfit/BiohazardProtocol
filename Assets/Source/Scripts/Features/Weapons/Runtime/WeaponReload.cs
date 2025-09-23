using System.Collections;
using UnityEngine;

public sealed class WeaponReload
{
    private readonly WeaponEffects _effects;
    private readonly WeaponConfig _config;

    public WeaponReload(WeaponEffects fx, WeaponConfig cfg)
    {
        _effects = fx;
        _config = cfg;
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

            yield return new WaitForSeconds(_config.ReloadFullTime);

            _effects.MoveStateBool(_config.ReloadFullBoolName, false);
        }
        else
        {
            _effects.MoveStateBool(_config.ReloadBoolName, true);
            _effects.PlayOne(_config.ReloadSound);

            yield return new WaitForSeconds(_config.ReloadTime);

            _effects.MoveStateBool(_config.ReloadBoolName, false);
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