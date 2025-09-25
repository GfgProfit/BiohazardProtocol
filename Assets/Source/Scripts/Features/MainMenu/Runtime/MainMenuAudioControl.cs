using System.Collections;
using UnityEngine;

public class MainMenuAudioControl : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioSource _loopAudioSource;

    [Space]
    [SerializeField] private float _timeToPlayLoopClip = 0.0f;

    [Space]
    [SerializeField] private AudioClip _introClip;
    [SerializeField] private AudioClip _loopClip;

    private void Awake()
    {
        if (_audioSource == null || _introClip == null || _loopClip == null)
        {
            return;
        }

        _timeToPlayLoopClip = _introClip.length;

        StartCoroutine(Play());
    }

    private IEnumerator Play()
    {
        _audioSource.clip = _introClip;
        _audioSource.Play();

        yield return new WaitForSeconds(_timeToPlayLoopClip - 5);

        _loopAudioSource.loop = true;
        _loopAudioSource.clip = _loopClip;
        _loopAudioSource.Play();
    }
}