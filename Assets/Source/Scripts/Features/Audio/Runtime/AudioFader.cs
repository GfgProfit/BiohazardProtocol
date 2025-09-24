using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class AudioFader : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float _fadeDuration = 1f;

    private Coroutine _fadeRoutine;

    private void Awake()
    {
        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Плавно запускает воспроизведение AudioClip.
    /// </summary>
    public void FadeIn(AudioClip clip = null, float duration = -1f)
    {
        if (clip != null)
            _audioSource.clip = clip;

        if (duration <= 0f)
            duration = _fadeDuration;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeInRoutine(duration));
    }

    /// <summary>
    /// Плавно завершает воспроизведение AudioClip.
    /// </summary>
    public void FadeOut(float duration = -1f)
    {
        if (duration <= 0f)
            duration = _fadeDuration;

        if (_fadeRoutine != null)
            StopCoroutine(_fadeRoutine);

        _fadeRoutine = StartCoroutine(FadeOutRoutine(duration));
    }

    private IEnumerator FadeInRoutine(float duration)
    {
        _audioSource.volume = 0f;
        _audioSource.Play();

        float time = 0f;
        while (time < duration)
        {
            time += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(0f, 1f, time / duration);
            yield return null;
        }
        _audioSource.volume = 1f;
    }

    private IEnumerator FadeOutRoutine(float duration)
    {
        float startVolume = _audioSource.volume;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            _audioSource.volume = Mathf.Lerp(startVolume, 0f, time / duration);
            yield return null;
        }

        _audioSource.volume = 0f;
        _audioSource.Stop();
    }
}