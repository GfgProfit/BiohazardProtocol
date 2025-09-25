using UnityEngine;

public class PlankView : MonoBehaviour
{
    [Header("What to disable when broken")]
    [SerializeField] private GameObject[] _disableOnBreak;
    [SerializeField] private Quaternion _selfLocalRepairedRotation;
    [SerializeField] private Vector3 _selfLocalRepairedPosition;
    [SerializeField] private Transform _brokenPoint;
    [SerializeField] private Transform _upPoint;

    [Header("Optional FX")]
    [SerializeField] private ParticleSystem _breakVfx;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private AudioClip _breakSfx;

    private bool _broken;

    public bool IsBroken => _broken;
    public Quaternion RepairedRotation => _selfLocalRepairedRotation;
    public Vector3 RepairedPosition => _selfLocalRepairedPosition;
    public Transform BrokenPoint => _brokenPoint;
    public Transform UpPoint => _brokenPoint;

    public void Break()
    {
        if (_broken)
        {
            return;
        }

        SetBroken(true);

        if (_disableOnBreak != null)
        {
            foreach (var go in _disableOnBreak)
            {
                if (go != null)
                {
                    go.transform.position = _brokenPoint.position;
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

    public void SetBroken(bool value) => _broken = value;

    [ContextMenu("Set Self")]
    public void SetSelf()
    {
        _disableOnBreak = new GameObject[1];
        _disableOnBreak[0] = gameObject;
        _selfLocalRepairedPosition = transform.localPosition;
        _selfLocalRepairedRotation = transform.localRotation;
    }
}