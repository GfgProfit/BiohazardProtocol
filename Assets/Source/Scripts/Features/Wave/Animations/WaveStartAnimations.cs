using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

public class WaveStartAnimations : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _selfRectTransform;
    [SerializeField] private GameObject _root;
    [SerializeField] private TMP_Text _waveText;

    [Space]
    [SerializeField] private float _animationDuration = 0.25f;

    public TMP_Text WaveText => _waveText;

    private Sequence _sequence;
    private Coroutine _waitFramesRoutine;

    public void StartInAnimation()
    {
        _sequence?.Kill();

        DOTween.Kill(_selfRectTransform);
        DOTween.Kill(_canvasGroup);

        if (_waitFramesRoutine != null)
        {
            StopCoroutine(_waitFramesRoutine);
            _waitFramesRoutine = null;
        }

        _canvasGroup.alpha = 1f;
        _selfRectTransform.localScale = Vector3.one;
        _root.SetActive(false);

        _sequence = DOTween.Sequence();

        _sequence.Join(_canvasGroup.DOFade(0f, _animationDuration));
        _sequence.Join(_selfRectTransform.DOScale(1.6f, _animationDuration));

        _sequence.AppendCallback(() => _root.SetActive(true));

        _sequence.Append(_canvasGroup.DOFade(1f, _animationDuration / 2f));
        _sequence.Join(_selfRectTransform.DOScale(0.9f, _animationDuration / 2f));

        _sequence.AppendCallback(() =>
        {
            if (_waitFramesRoutine != null)
            {
                StopCoroutine(_waitFramesRoutine);
            }

            _waitFramesRoutine = StartCoroutine(WaitFramesAndScale(8, 1.02f, _animationDuration / 7f));
        });

        _sequence.Join(_selfRectTransform.DOScale(1.0f, _animationDuration / 2f));

        _sequence.Play();
    }

    public void StartOutAnimation()
    {
        _sequence?.Kill();

        DOTween.Kill(_selfRectTransform);
        DOTween.Kill(_canvasGroup);

        if (_waitFramesRoutine != null)
        {
            StopCoroutine(_waitFramesRoutine);
            _waitFramesRoutine = null;
        }

        _canvasGroup.alpha = 1f;
        _selfRectTransform.localScale = Vector3.one;
        _root.SetActive(true);

        _sequence = DOTween.Sequence();

        _sequence.AppendCallback(() =>
        {
            if (_waitFramesRoutine != null)
            {
                StopCoroutine(_waitFramesRoutine);
            }

            _waitFramesRoutine = StartCoroutine(WaitFrames(10));
        });

        _sequence.Append(_canvasGroup.DOFade(0, _animationDuration));

        _sequence.AppendCallback(() => _root.SetActive(false));

        _sequence.Play();
    }

    private IEnumerator WaitFramesAndScale(int frames, float targetScale, float duration)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }

        _selfRectTransform.DOScale(targetScale, duration);
        _waitFramesRoutine = null;
    }

    private IEnumerator WaitFrames(int frames)
    {
        for (int i = 0; i < frames; i++)
        {
            yield return null;
        }
    }
}