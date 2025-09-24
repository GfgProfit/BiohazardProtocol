using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class RandomAudioPlayer : MonoBehaviour
{
    [SerializeField] private AudioSource _audioSource;

    [Space]
    [SerializeField] private AudioClip[] _clips;

    [Space]
    [SerializeField] private float _cooldown = 3f;
    [SerializeField] private bool _playOnStart = true;

    private Coroutine _playRoutine;

    private void OnEnable()
    {
        if (_playOnStart)
            StartPlaying();
    }

    private void OnDisable()
    {
        StopPlaying();
    }

    public void StartPlaying()
    {
        _playRoutine ??= StartCoroutine(PlayLoop());
    }

    public void StopPlaying()
    {
        if (_playRoutine != null)
        {
            StopCoroutine(_playRoutine);
            _playRoutine = null;
        }
    }

    private IEnumerator PlayLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cooldown);

            if (_clips.Length > 0)
            {
                int index = Random.Range(0, _clips.Length);
                _audioSource.PlayOneShot(_clips[index], 10);
            }
        }
    }
}