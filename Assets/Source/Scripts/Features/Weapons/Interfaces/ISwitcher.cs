public interface ISwitcher
{
    bool CanSwitch { get; set; }
    void Select(WeaponBehaviour weapon);
}