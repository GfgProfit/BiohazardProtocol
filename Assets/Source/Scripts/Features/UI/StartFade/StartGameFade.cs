using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class StartGameFade : MonoBehaviour
{
    [SerializeField] private CanvasGroup _fadeImage;

    private void Start()
    {
        _fadeImage.alpha = 1.0f;

        _fadeImage.DOFade(0.0f, 1.0f).SetEase(Ease.InCubic);
    }
}