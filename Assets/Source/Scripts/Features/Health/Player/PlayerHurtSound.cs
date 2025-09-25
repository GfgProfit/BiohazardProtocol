using UnityEngine;

public class PlayerHurtSound : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [Space]
    [SerializeField] private AudioClip[] _hurtSounds;

    public void Hurt()
    {
        _audioSource.PlayOneShot(_hurtSounds[Random.Range(0, _hurtSounds.Length)], 5);
    }
}