using UnityEngine;

public class CrosshairController : MonoBehaviour
{
    [Header("References (RectTransforms of ticks)")]
    [SerializeField] private RectTransform _up;
    [SerializeField] private RectTransform _down;
    [SerializeField] private RectTransform _left;
    [SerializeField] private RectTransform _right;

    [Header("Base spreads (pixels)")]
    [SerializeField] private float _idleSpread = 8f;
    [SerializeField] private float _walkSpread = 16f;
    [SerializeField] private float _sprintSpread = 28f;

    [Header("Shoot kick")]
    [SerializeField] private float _shootKick = 12f;
    [SerializeField] private float _shootKickCap = 36f;
    [SerializeField] private float _shootDecay = 40f;

    [Header("Smoothing")]
    [SerializeField] private float _smooth = 12f;

    private float _baseSpread;
    private float _shootImpulse;
    private float _currentSpread;

    private void Awake()
    {
        _baseSpread = _idleSpread;
        _shootImpulse = 0f;
        _currentSpread = _baseSpread;
        Apply(_currentSpread);
    }

    private void Update()
    {
        if (_shootImpulse > 0f)
        {
            _shootImpulse = Mathf.Max(0f, _shootImpulse - _shootDecay * Time.deltaTime);
        }

        float target = _baseSpread + _shootImpulse;
        float time = 1f - Mathf.Exp(-_smooth * Time.deltaTime);

        _currentSpread = Mathf.Lerp(_currentSpread, target, time);

        Apply(_currentSpread);
    }

    public void Walk() => _baseSpread = _walkSpread;
    public void Sprint() => _baseSpread = _sprintSpread;
    public void Shoot() => _shootImpulse = Mathf.Min(_shootImpulse + _shootKick, _shootKickCap);
    public void SetIdle() => _baseSpread = _idleSpread;

    private void Apply(float spread)
    {
        if (_up) _up.anchoredPosition = new Vector2(0f, spread);
        if (_down) _down.anchoredPosition = new Vector2(0f, -spread);
        if (_left) _left.anchoredPosition = new Vector2(-spread, 0f);
        if (_right) _right.anchoredPosition = new Vector2(spread, 0f);
    }
}