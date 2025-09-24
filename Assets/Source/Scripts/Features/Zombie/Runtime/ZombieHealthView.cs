using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening; // ? �����

public class ZombieHealthView : MonoBehaviour, ITargetable
{
    [Header("UI")]
    [SerializeField] private TMP_Text _healthCountText;
    [SerializeField] private CanvasGroup _group;
    [SerializeField] private Slider _hpBar;
    [SerializeField] private Slider _hpBarWhite;

    [Header("Fade")]
    [SerializeField] private float _fadeDuration = 0.15f;
    [SerializeField] private bool _startHidden = true;

    [Header("White Bar Lag")]
    [SerializeField, Min(0f)] private float _whiteDelay = 1.0f;      // �������� ����� ��������� ����� ������
    [SerializeField, Min(0f)] private float _whiteTweenDuration = 0.35f; // ������������ ����� � �������
    [SerializeField] private Ease _whiteEase = Ease.OutCubic;         // ���������
    [SerializeField] private bool _snapWhiteOnHeal = true;            // ��� ������� � ����� ����� ���������

    public bool IsDead => _health == null || _health.Current <= 0;

    private Coroutine _fadeRoutine;
    private HealthModel _health;

    // ���� ��� ����� ������ � ������, ����� �������� ��� ����� �����
    private Tweener _whiteTweener;
    private Tween _whiteDelayTweener;

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

    private void OnDisable()
    {
        KillWhiteTweens();
    }

    public void Focus() => FadeTo(1f);
    public void Unfocus() => FadeTo(0f);

    private void FadeTo(float target)
    {
        if (_fadeRoutine != null) StopCoroutine(_fadeRoutine);
        if (isActiveAndEnabled) _fadeRoutine = StartCoroutine(FadeRoutine(target));
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
            _group.alpha = Mathf.Lerp(start, target, time / _fadeDuration);
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
        _hpBarWhite.maxValue = _health.Max;

        // ��������� �������� ����������
        float v = _health.Current;
        _hpBar.value = v;
        _hpBarWhite.value = v;

        RefreshLabel();
    }

    public void Refresh()
    {
        // ���� ����� ������� � ����� ���� �� ����� �����������
        RefreshLabel();
    }

    private void RefreshLabel()
    {
        if (_healthCountText != null && _health != null)
            _healthCountText.text = $"<color=#D66A55>HP:</color> {_health.Current}";
    }

    public void Damage(int damage, Action deathCallback = null)
    {
        if (_health == null) return;

        int prev = _health.Current;
        _health.Damage(damage);
        int curr = Mathf.Max(_health.Current, 0);

        // ������� � �����
        _hpBar.value = curr;
        RefreshLabel();

        // ����� � � ��������� � ������ � ��������
        AnimateWhiteBarToward(_hpBar.value, isHeal: curr > prev);

        if (_health.Current <= 0)
        {
            Unfocus();
            deathCallback?.Invoke();
        }
    }

    // ���� ���-�� ���� ����� ������� � ���� ���
    public void Heal(int amount)
    {
        if (_health == null || amount <= 0) return;

        int prev = _health.Current;
        _health.Heal(amount);
        int curr = Mathf.Min(_health.Current, _health.Max);

        _hpBar.value = curr;
        RefreshLabel();

        AnimateWhiteBarToward(_hpBar.value, isHeal: curr > prev);
    }

    private void AnimateWhiteBarToward(float targetValue, bool isHeal)
    {
        // �������� ����� ���������� ��������/�����
        KillWhiteTweens();

        if (isHeal && _snapWhiteOnHeal)
        {
            // ��� ������� ����� ����������� (����� ��� �� ���� ���� �������)
            _hpBarWhite.value = targetValue;
            return;
        }

        // ��������� ���������� ����� ����� (��������)
        _whiteDelayTweener = DOVirtual.DelayedCall(_whiteDelay, () =>
        {
            _whiteTweener = _hpBarWhite
                .DOValue(targetValue, _whiteTweenDuration)
                .SetEase(_whiteEase)
                .SetUpdate(true); // ����� ����� � �� �����/������ timescale, �� �������
        }).SetUpdate(true);
    }

    private void KillWhiteTweens()
    {
        if (_whiteDelayTweener != null && _whiteDelayTweener.IsActive()) _whiteDelayTweener.Kill();
        if (_whiteTweener != null && _whiteTweener.IsActive()) _whiteTweener.Kill();
        _whiteDelayTweener = null;
        _whiteTweener = null;
    }

    public void OnDespawned()
    {
        if (_fadeRoutine != null)
        {
            StopCoroutine(_fadeRoutine);
            _fadeRoutine = null;
        }

        KillWhiteTweens();

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
