using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CustomButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("References")]
    [SerializeField] private Image _buttonImage;
    [SerializeField] private Image _buttonFadeImage;
    [SerializeField] private Image _upLineImage;
    [SerializeField] private Image _downLineImage;
    [SerializeField] private TMP_Text _buttonText;

    [Header("Parameters")]
    [SerializeField] private float _animationsDuration = 0.5f;

    [field: SerializeField] public UnityEvent OnClick { get; private set; } = new UnityEvent();

    private void OnDisable()
    {
        if (!this || gameObject == null || !gameObject.activeInHierarchy)
        {
            return;
        }

        ResetToExitState();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
        {
            return;
        }

        PlayClickAnimations();
        OnClick?.Invoke();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PlayEnterAnimations();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ResetToExitState();
    }

    private void PlayClickAnimations()
    {
        float halfDuration = _animationsDuration / 2.0f;

        AnimateColorPingPong(_buttonFadeImage, Color.gray, Color.white, halfDuration);
        AnimateColorPingPong(_buttonText, Color.white, Color.black, halfDuration);
        AnimateScalePingPong(_buttonText.rectTransform, Vector3.one * 0.95f, Vector3.one, halfDuration);
        AnimateFillPingPong(_upLineImage, 0.9f, 1.0f, halfDuration);
        AnimateFillPingPong(_downLineImage, 0.9f, 1.0f, halfDuration);
    }

    private void PlayEnterAnimations()
    {
        _buttonFadeImage.DOFade(1.0f, _animationsDuration).SetEase(Ease.OutBack);
        _buttonImage.DOFade(0.0f, _animationsDuration).SetEase(Ease.OutBack);
        _buttonText.DOColor(Color.black, _animationsDuration).SetEase(Ease.OutBack);

        _upLineImage.rectTransform.DOLocalMove(new Vector3(-125.0f, 23.5f, 0), _animationsDuration).SetEase(Ease.OutBack);
        _downLineImage.rectTransform.DOLocalMove(new Vector3(125.0f, -23.5f, 0), _animationsDuration).SetEase(Ease.OutBack);

        _upLineImage.DOFillAmount(1.0f, _animationsDuration).SetEase(Ease.OutBack);
        _downLineImage.DOFillAmount(1.0f, _animationsDuration).SetEase(Ease.OutBack);
    }

    private void ResetToExitState()
    {
        _buttonFadeImage.DOFade(0, _animationsDuration).SetEase(Ease.OutBack);
        _buttonImage.DOFade(0.588f, _animationsDuration).SetEase(Ease.OutBack);
        _buttonText.DOColor(Color.white, _animationsDuration).SetEase(Ease.OutBack);

        _upLineImage.rectTransform.DOLocalMove(new Vector3(225.0f, 23.5f, 0), _animationsDuration).SetEase(Ease.OutBack);
        _downLineImage.rectTransform.DOLocalMove(new Vector3(-225.0f, -23.5f, 0), _animationsDuration).SetEase(Ease.OutBack);

        _upLineImage.DOFillAmount(0.0f, _animationsDuration).SetEase(Ease.OutBack);
        _downLineImage.DOFillAmount(0.0f, _animationsDuration).SetEase(Ease.OutBack);
    }

    private void AnimateColorPingPong(Graphic target, Color first, Color second, float halfDuration)
    {
        target.DOColor(first, halfDuration)
              .OnComplete(() => target.DOColor(second, halfDuration))
              .SetEase(Ease.OutBack);
    }

    private void AnimateScalePingPong(RectTransform rectTransform, Vector3 from, Vector3 to, float halfDuration)
    {
        rectTransform.DOScale(from, halfDuration)
                     .OnComplete(() => rectTransform.DOScale(to, halfDuration))
                     .SetEase(Ease.OutBack);
    }

    private void AnimateFillPingPong(Image image, float from, float to, float halfDuration)
    {
        image.DOFillAmount(from, halfDuration)
             .OnComplete(() => image.DOFillAmount(to, halfDuration))
             .SetEase(Ease.OutBack);
    }
}