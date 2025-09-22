using UnityEngine;

#pragma warning disable IDE0044

public class MoneySound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [Space]
    [SerializeField] private AudioClip _moneySpendedClip;

    [Inject]
    private IMoney _moneyService;

    private void OnEnable()
    {
        _moneyService.OnSpended += PlaySound;
    }

    private void OnDisable()
    {
        _moneyService.OnSpended -= PlaySound;
    }

    private void PlaySound()
    {
        _audioSource.PlayOneShot(_moneySpendedClip);
    }
}