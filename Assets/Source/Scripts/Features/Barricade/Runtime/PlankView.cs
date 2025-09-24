using UnityEngine;

public class PlankView : MonoBehaviour
{
    [Header("What to disable when broken")]
    [SerializeField] private GameObject[] _disableOnBreak;

    [Header("Optional FX")]
    [SerializeField] private ParticleSystem _breakVfx;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _breakSfx;

    private bool _broken;

    public bool IsBroken => _broken;

    public void Break()
    {
        if (_broken)
        {
            return;
        }

        _broken = true;

        if (_disableOnBreak != null)
        {
            foreach (var go in _disableOnBreak)
            {
                if (go != null)
                {
                    go.SetActive(false);
                }
            }
        }

        if (_breakVfx != null)
        {
            _breakVfx.Play();
        }

        if (_audioSource != null && _breakSfx != null)
        {
            _audioSource.PlayOneShot(_breakSfx, 5);
        }
    }

    [ContextMenu("Set Self")]
    public void SetSelf()
    {
        _disableOnBreak = new GameObject[1];
        _disableOnBreak[0] = gameObject;
    }
}