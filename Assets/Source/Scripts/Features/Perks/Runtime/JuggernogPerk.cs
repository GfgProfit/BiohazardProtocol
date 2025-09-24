using UnityEngine;

public class JuggernogPerk : PerkAbstract
{
    [SerializeField] private PlayerHealth _playerHealth;

    public override void Use()
    {
        _playerHealth.ResetHealth(200);
        _playerHealth.JuggernogActive = true;
    }
}