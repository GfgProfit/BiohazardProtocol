using DG.Tweening;
using TMPro;
using UnityEngine;

public class MoneyPopupView : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private TMP_Text _text;

    private Sequence _sequence;

    public void Create(int value)
    {
        _text.text = $"+{value} <color=#FFD800><size=26>$</color></size>";

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_canvasGroup.DOFade(1.0f, 0.5f).SetEase(Ease.OutCubic));
        _sequence.Append(_canvasGroup.DOFade(0.0f, 0.5f)
            .OnComplete(() =>
            {
                Destroy(gameObject);
            }).SetEase(Ease.InCubic));
    }
}