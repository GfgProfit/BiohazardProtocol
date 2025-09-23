using DG.Tweening;
using UnityEngine;

public class WaveClearedAnimations : MonoBehaviour
{
    [SerializeField] private CanvasGroup _backgroundGroup;
    [SerializeField] private RectTransform _background;
    [SerializeField] private CanvasGroup _missionNameGroup;
    [SerializeField] private RectTransform _missionName;
    [SerializeField] private CanvasGroup _textGroup;
    [SerializeField] private GameObject _root;

    [Space]
    [SerializeField] private float _animationDuration = 0.5f;

    private Sequence _sequence;

    public void StartInAnimation()
    {
        _sequence?.Kill();
        DOTween.Kill(_backgroundGroup);
        DOTween.Kill(_missionNameGroup);
        DOTween.Kill(_textGroup);
        DOTween.Kill(_missionName);
        DOTween.Kill(_background);

        _root.SetActive(true);
        _backgroundGroup.alpha = 0;
        _background.localScale = Vector3.one * 0.5f;
        _missionNameGroup.alpha = 0;
        _missionName.localScale = Vector3.one * 0.5f;
        _textGroup.alpha = 0;
        _missionNameGroup.gameObject.SetActive(true);
        _textGroup.gameObject.SetActive(true);

        _sequence = DOTween.Sequence();

        _sequence.Append(_backgroundGroup.DOFade(1, _animationDuration * 0.5f));
        _sequence.Join(_background.DOScale(1.2f, _animationDuration * 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_background.DOScale(1f, _animationDuration * 0.2f));

        _sequence.Append(_textGroup.DOFade(1, _animationDuration * 0.3f));

        _sequence.Append(_missionNameGroup.DOFade(1, _animationDuration * 0.4f));
        _sequence.Join(_missionName.DOScale(1.1f, _animationDuration * 0.4f).SetEase(Ease.OutBack));
        _sequence.Append(_missionName.DOScale(1f, _animationDuration * 0.2f));

        _sequence.Play();
    }

    public void StartOutAnimation()
    {
        _sequence?.Kill();
        DOTween.Kill(_backgroundGroup);
        DOTween.Kill(_missionNameGroup);
        DOTween.Kill(_textGroup);
        DOTween.Kill(_missionName);
        DOTween.Kill(_background);

        _root.SetActive(true);
        _backgroundGroup.alpha = 1f;
        _missionNameGroup.alpha = 1f;
        _textGroup.alpha = 1f;
        _missionName.localScale = Vector3.one;

        _sequence = DOTween.Sequence();

        _sequence.Join(_backgroundGroup.DOFade(0f, 0.35f).SetEase(Ease.InCubic));
        _sequence.Join(_missionNameGroup.DOFade(0f, 0.55f).SetEase(Ease.InCubic));
        _sequence.Join(_missionName
            .DOScale(0f, 0.55f)
            .SetEase(Ease.InBack, overshoot: 0.5f));

        _sequence.Join(_textGroup.DOFade(0f, 0.4f).SetEase(Ease.InCubic));

        float longest = Mathf.Max(0.35f, 0.55f, 0.55f, 0.9f);

        _sequence.AppendInterval(longest)
            .AppendCallback(() =>
            {
                if (_missionNameGroup)
                {
                    _missionNameGroup.gameObject.SetActive(false);
                }

                if (_textGroup)
                {
                    _textGroup.gameObject.SetActive(false);
                }
            });

        _sequence.AppendCallback(() => _root.SetActive(false));

        _sequence.Play();
    }
}