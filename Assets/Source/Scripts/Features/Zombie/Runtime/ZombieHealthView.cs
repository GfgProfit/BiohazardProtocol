using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ZombieHealthView : MonoBehaviour, ITargetable
{
    [Header("UI")]
    [SerializeField] private TMP_Text _healthCountText;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Slider _hpBar;

    [Header("Fade")]
    [SerializeField] private float _fadeDuration = 0.15f;
    [SerializeField] private bool _startHidden = true;

    private Coroutine _fadeRoutine;
    private HealthModel _health;

    private void Awake()
    {
        if (_group == null)
            _group = GetComponentInChildren<CanvasGroup>(true);

        if (_startHidden && _group != null)
        {
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;
        }
    }

    public void Focus() => FadeTo(1f);
    public void Unfocus() => FadeTo(0f);

    private void FadeTo(float target)
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
        }

        _fadeRoutine = StartCoroutine(FadeRoutine(target));
    }

    private IEnumerator FadeRoutine(float target)
    {
        _group.gameObject.SetActive(true);
        _group.blocksRaycasts = target > 0.5f;
        _group.interactable = target > 0.5f;

        float start = _group.alpha;
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            float k = time / _fadeDuration;
            _group.alpha = Mathf.Lerp(start, target, k);
            yield return null;
        }

        _group.alpha = target;

        if (Mathf.Approximately(target, 0f))
        {
            _group.interactable = false;
            _group.blocksRaycasts = false;
            _group.gameObject.SetActive(false);
        }

        _fadeRoutine = null;
    }

    public void Setup(HealthModel health)
    {
        _health = health;
        _hpBar.maxValue = _health.Max;

        Refresh();
    }

    public void Refresh()
    {
        if (_healthCountText != null && _health != null)
        {
            _healthCountText.text = $"<color=#D66A55>HP:</color> {_health.Current}";
            _hpBar.value = _health.Current;
        }
    }

    public void Damage(int damage, Action deathCallback = null)
    {
        if (_health == null)
        {
            return;
        }

        _health.Damage(damage);
        Refresh();

        if (_health.Current <= 0)
        {
            deathCallback?.Invoke();
        }
    }

    public void OnDespawned()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        if (_group != null)
        {
            _group.alpha = 0f;
            _group.interactable = false;
            _group.blocksRaycasts = false;
            _group.gameObject.SetActive(false);
        }

        _health = null;
    }
}