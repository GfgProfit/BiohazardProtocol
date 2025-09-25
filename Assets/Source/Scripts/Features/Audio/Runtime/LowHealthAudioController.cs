using UnityEngine;
using UnityEngine.Audio;

public class LowHealthAudioController : MonoBehaviour
{
    [Header("Mixer")]
    [SerializeField] private AudioMixerSnapshot normalSnapshot;
    [SerializeField] private AudioMixerSnapshot lowHealthSnapshot;

    [Header("Health")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField, Range(0f, 1f)] private float lowHealthThreshold = 0.2f;
    [SerializeField] private float fadeTime = 0.35f;

    private bool _isLow;

    private void Update()
    {
        if (_playerHealth.Health == null || _playerHealth.Health.Max <= 0)
            return;

        if (_playerHealth.Health.IsDead)
        {
            if (_isLow)
            {
                _isLow = false;
                normalSnapshot.TransitionTo(fadeTime);
            }
            return;
        }

        float hp01 = Mathf.Clamp01((float)_playerHealth.Health.Current / _playerHealth.Health.Max);
        bool shouldBeLow = hp01 <= lowHealthThreshold;

        if (shouldBeLow == _isLow)
            return;

        _isLow = shouldBeLow;

        if (_isLow)
            lowHealthSnapshot.TransitionTo(fadeTime);
        else
            normalSnapshot.TransitionTo(fadeTime);
    }
}