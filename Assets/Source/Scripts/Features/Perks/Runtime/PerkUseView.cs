using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PerkUseView : MonoBehaviour
{
    [SerializeField] private Image _fxImage;
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _holder;
    [SerializeField] private Image _perkIconPrefab;

    [Space]
    [SerializeField] private float _animationDuration = 0.5f;

    private Sequence _sequence;
    private readonly Coroutine _waitFramesRoutine;

    public void Fade(float value, Color color)
    {
        _fxImage.color = color;

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(value, _animationDuration).SetEase(Ease.OutCubic));
        _sequence.AppendCallback(() =>
        {
            if (_waitFramesRoutine != null)
            {
                StopCoroutine(_waitFramesRoutine);
            }

            StartCoroutine(WaitFrames(1));
        });
        _sequence.Append(_canvasGroup.DOFade(0, _animationDuration / 2).SetEase(Ease.InCubic));
    }

    private IEnumerator WaitFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
    }

    public void CreatePerkIcon(Sprite sprite)
    {
        Image perkIcon = Instantiate(_perkIconPrefab, _holder);
        RectTransform iconRectTransform = perkIcon.GetComponent<RectTransform>();

        perkIcon.sprite = sprite;
        iconRectTransform.localScale = Vector3.zero;

        iconRectTransform.DOScale(1.1f, _animationDuration / 2)
            .OnComplete(() =>
            {
                iconRectTransform.DOScale(1, 0.2f);
            }).SetEase(Ease.OutCubic);
    }
}