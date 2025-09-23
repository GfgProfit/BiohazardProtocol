using UnityEngine;

public sealed class WeaponContextAdapter : IWeaponContext
{
    private readonly WeaponManager _weaponManager;

    public WeaponContextAdapter(WeaponManager weaponManager)
    {
        _weaponManager = weaponManager;
    }

    public Transform ShootOrigin => _weaponManager.ShootTransform;
    public Vector3 Forward => _weaponManager.ShootTransform.forward;

    public IRecoil Recoil => new RecoilAdapter(_weaponManager.WeaponRecoilManager);
    public ICrosshair Crosshair => new CrosshairAdapter(_weaponManager.CrosshairController, _weaponManager.Crosshair);
    public IPlayer Player => new PlayerAdapter(_weaponManager.PlayerController);
    public ISwitcher Switcher => new SwitcherAdapter(_weaponManager.SwitchWeapon);

    public void SetAmmoText(string text)
    {
        if (_weaponManager.AmmoText != null) _weaponManager.AmmoText.text = text;
        if (_weaponManager.AmmoText != null) _weaponManager.AmmoText.enabled = true;
    }

    private sealed class RecoilAdapter : IRecoil
    {
        private readonly WeaponRecoilManager _recoilManager;

        public RecoilAdapter(WeaponRecoilManager recoilManager)
        {
            _recoilManager = recoilManager;
        }

        public void Tick(float returnSpeed, float snappiness) => _recoilManager.Tick(returnSpeed, snappiness);
        public void Shoot(float x, float y, float z) => _recoilManager.RecoilShoot(x, y, z);
    }

    private sealed class CrosshairAdapter : ICrosshair
    {
        public CanvasGroup Group { get; }

        private readonly CrosshairController _crosshaircontroller;

        public CrosshairAdapter(CrosshairController crosshaircontroller, CanvasGroup crosshair)
        {
            _crosshaircontroller = crosshaircontroller;
            Group = crosshair;
        }

        public void Shoot() => _crosshaircontroller.Shoot();
    }

    private sealed class PlayerAdapter : IPlayer
    {
        private readonly PlayerController _pc;
        
        public PlayerAdapter(PlayerController pc)
        {
            _pc = pc;
        }

        public bool IsWalking => _pc.IsWalking;
        public bool IsSprinting => _pc.IsSprinting;
    }

    private sealed class SwitcherAdapter : ISwitcher
    {
        private readonly WeaponSwitch _weaponSwitch;

        public SwitcherAdapter(WeaponSwitch weaponSwitch)
        {
            _weaponSwitch = weaponSwitch;
        }

        public bool CanSwitch { get => _weaponSwitch.CanSwitch; set => _weaponSwitch.CanSwitch = value; }
        public void Select(WeaponBehaviour w) => _weaponSwitch.Select(w);
    }
}
