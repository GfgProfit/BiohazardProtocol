using UnityEngine;

public class ZombieHitScan : MonoBehaviour
{
    [SerializeField] private float _damageMultiplier;
    public float DamageMultiplier => _damageMultiplier;
}