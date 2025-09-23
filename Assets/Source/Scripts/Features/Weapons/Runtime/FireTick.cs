public readonly struct FireTick
{
    public readonly bool FireHeld;
    public readonly bool FirePressed;
    public readonly bool AimHeld;
    public FireTick(bool fireHeld, bool firePressed, bool aimHeld)
    { FireHeld = fireHeld; FirePressed = firePressed; AimHeld = aimHeld; }
}