using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class ImpactSound : MonoBehaviour
{
    [SerializeField] private AudioClip[] _impactClips;

    private AudioSource _audioSource;

    public void PlaySound()
    {
        _audioSource = GetComponent<AudioSource>();

        var randomClip = _impactClips[Random.Range(0, _impactClips.Length)];
        _audioSource.PlayOneShot(randomClip, 3);
    }
}