using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _survivedWavesText;
    [SerializeField] private TMP_Text _survivedTimeText;

    public int Kills { get; set; }
    public int SurvivedWaves { get; set; }
    public float SurvivedTimeSeconds { get; set; }

    private Sequence _sequence;

    private void Update()
    {
        if (_playerHealth == null)
        {
            return;
        }

        SurvivedTimeSeconds += Time.deltaTime;
    }

    public void ShowPanel()
    {
        _killsText.text = $"Kills: <color=#FF6969>{Kills}</color>";
        _survivedWavesText.text = $"Lived through Waves: <color=#6EFFA1>{SurvivedWaves}</color>";
        _survivedTimeText.text = $"Survived Time: <color=#FFC453>{GetFormattedTime(SurvivedTimeSeconds)}</color>";

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_killsText.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_survivedWavesText.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_survivedTimeText.DOFade(1, 0.5f).SetEase(Ease.OutBack));

        _sequence?.Play();
    }

    public string GetFormattedTime(float seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);

        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            time.Hours, time.Minutes, time.Seconds);
    }
}