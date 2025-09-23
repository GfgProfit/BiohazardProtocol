public readonly struct AnimationNames
{
    public readonly string Shoot, AimShoot, ReloadBool, ReloadFullBool, Hide, AimBool;

    public AnimationNames(string shoot, string aimShoot, string reloadBool, string reloadFullBool, string hide, string aimBool)
    {
        Shoot = shoot;
        AimShoot = aimShoot;
        ReloadBool = reloadBool;
        ReloadFullBool = reloadFullBool;
        Hide = hide;
        AimBool = aimBool;
    }
}