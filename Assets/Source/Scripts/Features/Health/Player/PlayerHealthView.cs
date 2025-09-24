using DG.Tweening;
using UnityEngine;

public class PlayerHealthView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private RectTransform _pulseTarget;
    [SerializeField] private AudioFader _audioFader;
    [SerializeField] private AudioClip _heartBeatClip;

    [Header("Logic")]
    [Range(0f, 1f)]
    [SerializeField] private float _thresholdPercent = 0.20f;

    [Header("Tween")]
    [SerializeField] private float _fadeIn = 0.2f;
    [SerializeField] private float _fadeOut = 0.2f;
    [SerializeField] private float _pulseScale = 1.08f;
    [SerializeField] private float _pulseDuration = 0.6f;
    [SerializeField] private float _blinkAlphaHigh = 1f;
    [SerializeField] private float _blinkAlphaLow = 0.35f;
    [SerializeField] private bool _useUnscaledTime = true;

    private Sequence _seq;
    private bool _active;
    private Vector3 _initialScale = Vector3.one;

    private void Awake()
    {
        if (_group != null)
        {
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }

        if (_pulseTarget == null)
        {
            _pulseTarget = transform as RectTransform;
        }

        if (_pulseTarget != null)
        {
            _initialScale = _pulseTarget.localScale;
        }
    }

    private void OnEnable()
    {
        Evaluate();
    }

    private void Update()
    {
        Evaluate();
    }

    private void Evaluate()
    {
        if (_playerHealth == null || _playerHealth.Health == null || _group == null)
        {
            return;
        }    

        float pct = (_playerHealth.Health.Max > 0) ? (float)_playerHealth.Health.Current / _playerHealth.Health.Max : 0f;

        bool shouldBeActive = pct <= _thresholdPercent && !_playerHealth.Health.IsDead == true;

        if (shouldBeActive && !_active)
        {
            Activate();
            _audioFader.FadeIn(_heartBeatClip);
        }
        else if (!shouldBeActive && _active)
        {
            Deactivate();
            _audioFader.FadeOut();
        }
    }

    private void Activate()
    {
        _active = true;
        _group.DOKill();
        _pulseTarget?.DOKill();
        _seq?.Kill();

        _group.alpha = 0f;
        _group.DOFade(_blinkAlphaHigh, _fadeIn).SetUpdate(_useUnscaledTime);

        _seq = DOTween.Sequence().SetUpdate(_useUnscaledTime);

        _seq.Append(_group.DOFade(_blinkAlphaLow, _fadeOut));

        if (_pulseTarget != null)
        {
            _seq.Join(_pulseTarget.DOScale(_pulseScale, _pulseDuration * 0.5f).SetEase(Ease.OutSine));
        }

        _seq.Append(_group.DOFade(_blinkAlphaHigh, _fadeIn));

        if (_pulseTarget != null)
        {
            _seq.Join(_pulseTarget.DOScale(_initialScale, _pulseDuration * 0.5f).SetEase(Ease.InSine));
        }

        _seq.SetLoops(-1, LoopType.Restart);
    }

    private void Deactivate()
    {
        _active = false;
        _seq?.Kill();
        _seq = null;

        if (_pulseTarget != null)
        {
            _pulseTarget.DOScale(_initialScale, _fadeOut).SetUpdate(_useUnscaledTime);
        }

        _group.DOFade(0f, _fadeOut)
              .SetUpdate(_useUnscaledTime)
              .OnComplete(() =>
              {
                  _group.interactable = false;
                  _group.blocksRaycasts = false;
              });
    }

    private void OnDisable()
    {
        _seq?.Kill();
        _seq = null;
        _group?.DOKill();
        _pulseTarget?.DOKill();

        if (_group != null)
        {
            _group.alpha = 0f;
        }

        if (_pulseTarget != null)
        {
            _pulseTarget.localScale = _initialScale;
        }
    }
}