using DG.Tweening;
using TMPro;
using UnityEngine;

public class WaveCountdownTimerView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _parentGroup;
    [SerializeField] private RectTransform _parentTransform;
    [SerializeField] private TMP_Text _timerText;

    [Space]
    [SerializeField] private float _animationDuration = 0.25f;

    public void StartInAnimation()
    {
        _parentGroup.alpha = 0.0f;
        _parentTransform.localScale = Vector3.zero;

        _parentGroup.DOFade(1, _animationDuration);
        _parentTransform.DOScale(1.1f, _animationDuration)
            .OnComplete(() =>
            {
                _parentTransform.DOScale(1f, _animationDuration / 2);
            });
    }

    public void StartOutAnimation()
    {
        _parentGroup.alpha = 1.0f;
        _parentTransform.localScale = Vector3.one;

        _parentGroup.DOFade(0, _animationDuration);
        _parentTransform.DOScale(1.1f, _animationDuration / 2)
            .OnComplete(() =>
            {
                _parentTransform.DOScale(0f, _animationDuration);
            });
    }

    public void UpdateText(float value)
    {
        _timerText.text = value.ToString("F2");
    }
}