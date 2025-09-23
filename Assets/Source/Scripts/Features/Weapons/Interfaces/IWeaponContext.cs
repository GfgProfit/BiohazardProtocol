using UnityEngine;

public interface IWeaponContext
{
    Transform ShootOrigin { get; }
    Vector3 Forward { get; }

    IRecoil Recoil { get; }
    ICrosshair Crosshair { get; }
    IPlayer Player { get; }
    ISwitcher Switcher { get; }

    void SetAmmoText(string text);
}