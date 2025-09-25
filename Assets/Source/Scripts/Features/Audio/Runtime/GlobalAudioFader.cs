using UnityEngine;
using UnityEngine.Audio;

public class GlobalAudioFader : MonoBehaviour
{
    [SerializeField] private AudioMixerSnapshot normal;
    [SerializeField] private AudioMixerSnapshot muted;

    public void FadeToMuted(float duration) => muted.TransitionTo(duration);
    public void FadeToNormal(float duration) => normal.TransitionTo(duration);
}