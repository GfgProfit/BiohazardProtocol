using System;
using System.Collections;
using UnityEngine;

public sealed class WeaponReload
{
    private readonly WeaponEffects _effects;
    private readonly WeaponConfig _config;
    private readonly WeaponManager _weaponManager;
    private readonly Animator _animator;

    private bool _tubeReloadCancel = false;
    private bool _didEmptyReload = false;
    private bool _isReloading = false;

    public Action OnUpdateText;

    public WeaponReload(WeaponEffects fx, WeaponConfig cfg, WeaponManager weaponManager, Animator animator)
    {
        _effects = fx;
        _config = cfg;
        _weaponManager = weaponManager;
        _animator = animator;
    }

    public void Tick()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0) && _isReloading)
        {
            _tubeReloadCancel = true;
        }
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
        if (_config.FireMode != FireMode.Shotgun && !_config.UseTube)
        {
            if (_config.CurrentAmmo >= _config.MagSize)
            {
                return false;
            }
        }
        else
        {
            if (_config.Tube >= _config.TubeSize)
            {
                return false;
            }
        }

        return true;
    }

    public IEnumerator Run()
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

    public IEnumerator ReloadTubeShotgun()
    {
        _isReloading = true;
        _tubeReloadCancel = false;
        _didEmptyReload = false;

        float ChamberedAnimationTime = _weaponManager.SpeedColaActive ? _config.ChamberedAnimationTime / _weaponManager.SpeedColaMultiplier : _config.ChamberedAnimationTime;
        float ReloadStartAnimationTime = _weaponManager.SpeedColaActive ? _config.ReloadStartAnimationTime / _weaponManager.SpeedColaMultiplier : _config.ReloadStartAnimationTime;
        float ReloadAnimationTime = _weaponManager.SpeedColaActive ? _config.ReloadAnimationTime / _weaponManager.SpeedColaMultiplier : _config.ReloadAnimationTime;
        
        if (_weaponManager.SpeedColaActive)
        {
            _effects.SetSpeedMultiplier(_weaponManager.SpeedColaMultiplier);
        }
        else
        {
            _effects.SetSpeedMultiplier(1);
        }

        if (_config.AvailableAmmo <= 0 || _config.Tube >= _config.TubeSize)
        {
            _isReloading = false;
            yield break;
        }

        if (_config.Chambered == 0 && _config.Tube == 0)
        {
            _effects.PlayOne(_config.FullReloadSound);
            yield return PlayAndWaitCancelable(_config.ChamberedAnimationName, ChamberedAnimationTime);

            if (_tubeReloadCancel)
            {
                _effects.SetSpeedMultiplier(1);
                yield return FinishAndExit();
                yield break;
            }

            _didEmptyReload = true;
        }

        if (!_didEmptyReload && _config.Chambered > 0 && _config.AvailableAmmo > 0 && _config.Tube < _config.TubeSize)
        {
            yield return PlayAndWaitCancelable(_config.ReloadStartAnimationName, ReloadStartAnimationTime);

            if (_tubeReloadCancel)
            {
                _effects.SetSpeedMultiplier(1);
                yield return FinishAndExit();
                yield break;
            }
        }

        while (!_tubeReloadCancel && _config.AvailableAmmo > 0 && _config.Tube < _config.TubeSize)
        {
            _effects.PlayOne(_config.ReloadSound);
            yield return PlayAndWaitCancelable(_config.ReloadAnimationName, ReloadAnimationTime);

            if (_tubeReloadCancel)
            {
                _effects.SetSpeedMultiplier(1);
                break;
            }
        }

        _effects.SetSpeedMultiplier(1);
        yield return FinishAndExit();
    }

    private IEnumerator PlayAndWaitCancelable(string state, float duration)
    {
        _animator.Play(state, 0, 0f);

        float time = 0f;

        while (time < duration && !_tubeReloadCancel)
        {
            time += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator FinishAndExit()
    {
        float ReloadEndAnimationTime = _weaponManager.SpeedColaActive ? _config.ReloadEndAnimationTime / _weaponManager.SpeedColaMultiplier : _config.ReloadEndAnimationTime;

        _animator.Play(_config.ReloadEndAnimationName, 0, 0f);

        float time = 0f;

        while (time < ReloadEndAnimationTime)
        {
            time += Time.deltaTime;
            _effects.SetSpeedMultiplier(1);
            yield return null;
        }

        _isReloading = false;
    }

    public void Tube()
    {
        _config.Tube++;
        _config.AvailableAmmo--;

        OnUpdateText?.Invoke();
    }

    public void Chambered()
    {
        _config.Chambered++;
        _config.AvailableAmmo--;

        OnUpdateText?.Invoke();
    }

    public void RefillMagazine()
    {
        int need = Mathf.Clamp(_config.MagSize - _config.CurrentAmmo, 0, _config.MagSize);
        int take = Mathf.Min(need, _config.AvailableAmmo);

        _config.CurrentAmmo += take;
        _config.AvailableAmmo -= take;
    }
}