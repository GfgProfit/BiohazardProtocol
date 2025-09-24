using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[DisallowMultipleComponent]
public class PlayerHealthBarView : MonoBehaviour
{
    [Header("Refs")]
    [SerializeField] private PlayerHealth _player;
    [SerializeField] private Image _hpFill;
    [SerializeField] private Image _hpFillWhite;

    [Header("Timings")]
    [SerializeField] private float _redDamageDuration = 0.08f;
    [SerializeField] private float _redHealDuration = 0.15f;
    [SerializeField] private float _whiteLagDelay = 0.12f;
    [SerializeField] private float _whiteLagDuration = 0.35f;
    [SerializeField] private bool _useUnscaledTime = true;

    [Header("Options")]
    [SerializeField] private bool _instantWhiteOnHeal = true;
    [SerializeField] private bool _autoFindRefs = true;

    private float _lastPercent = -1f;
    private Tween _redTween, _whiteTween;
    private Sequence _whiteSeq;

    private void Reset()
    {
        TryAutoWire();
    }

    private void Awake()
    {
        if (_autoFindRefs) TryAutoWire();
        EnsureFilled(_hpFill);
        EnsureFilled(_hpFillWhite);

        DOTween.Init(false, true, LogBehaviour.Default);
        SyncImmediate();
    }

    private void OnEnable()
    {
        SyncImmediate();
    }

    private void OnDisable()
    {
        KillTweens();
    }

    private void Update()
    {
        if (_player == null || _player.Health == null)
        {
            return;
        }

        float p = CalcPercent(_player.Health);

        if (p < 0f)
        {
            return;
        }

        if (Mathf.Approximately(p, _lastPercent))
        {
            return;
        }

        bool damage = (_lastPercent >= 0f && p < _lastPercent);

        AnimateTo(p, damage);
        _lastPercent = p;
    }

    private void AnimateTo(float target, bool damage)
    {
        _redTween?.Kill();
        float redDur = damage ? _redDamageDuration : _redHealDuration;

        if (redDur <= 0f)
        {
            _hpFill.fillAmount = target;
        }
        else
        {
            _redTween = _hpFill.DOFillAmount(target, redDur).SetEase(damage ? Ease.Linear : Ease.OutQuad).SetUpdate(_useUnscaledTime);
        }

        _whiteSeq?.Kill();
        _whiteTween?.Kill();

        if (damage)
        {
            _whiteSeq = DOTween.Sequence().SetUpdate(_useUnscaledTime);

            if (_whiteLagDelay > 0f)
            {
                _whiteSeq.AppendInterval(_whiteLagDelay);
            }

            _whiteSeq.Append(_hpFillWhite.DOFillAmount(target, Mathf.Max(0.01f, _whiteLagDuration)) .SetEase(Ease.OutCubic));
        }
        else
        {
            if (_instantWhiteOnHeal)
            {
                _hpFillWhite.fillAmount = target;
            }
            else
            {
                _whiteTween = _hpFillWhite.DOFillAmount(target, Mathf.Max(0.06f, _redHealDuration * 0.6f)).SetEase(Ease.OutQuad).SetUpdate(_useUnscaledTime);
            }
        }
    }

    private void SyncImmediate()
    {
        if (_player == null || _player.Health == null)
        {
            return;
        }

        float p = CalcPercent(_player.Health);

        if (p < 0f)
        {
            p = 0f;
        }

        KillTweens();

        if (_hpFill)
        {
            _hpFill.fillAmount = p;
        }

        if (_hpFillWhite)
        {
            _hpFillWhite.fillAmount = p;
        }

        _lastPercent = p;
    }

    private void KillTweens()
    {
        _redTween?.Kill(); _redTween = null;
        _whiteTween?.Kill(); _whiteTween = null;
        _whiteSeq?.Kill(); _whiteSeq = null;
    }

    private static float CalcPercent(HealthModel h)
    {
        if (h == null || h.Max <= 0)
        {
            return -1f;
        }

        return Mathf.Clamp01((float)h.Current / h.Max);
    }

    private void TryAutoWire()
    {
        if (_player == null)
        {
            _player = GetComponentInParent<PlayerHealth>() ?? FindAnyObjectByType<PlayerHealth>();
        }

        if (_hpFill == null || _hpFillWhite == null)
        {
            var images = GetComponentsInChildren<Image>(true);
            foreach (var img in images)
            {
                string n = img.name.ToLowerInvariant();

                if (_hpFill == null && (n.Contains("red") || n.Contains("hpfill")))
                {
                    _hpFill = img;
                }
                else if (_hpFillWhite == null && (n.Contains("white") || n.Contains("hpfillwhite")))
                {
                    _hpFillWhite = img;
                }
            }

            if (_hpFill == null && images.Length > 0)
            {
                _hpFill = images[0];
            }

            if (_hpFillWhite == null && images.Length > 1)
            {
                _hpFillWhite = images[1];
            }
        }
    }

    private static void EnsureFilled(Image img)
    {
        if (img == null)
        {
            return;
        }
        
        img.type = Image.Type.Filled;

        if (img.fillMethod == Image.FillMethod.Radial360 && img.fillAmount == 0f)
        {
            img.fillOrigin = 2;
        }
    }
}
