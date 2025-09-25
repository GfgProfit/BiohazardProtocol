using DG.Tweening;
using System;
using TMPro;
using UnityEngine;

#pragma warning disable IDE0044

public class GameInfo : MonoBehaviour
{
    [SerializeField] private PlayerHealth _playerHealth;
    [SerializeField] private CanvasGroup _panel;
    [SerializeField] private TMP_Text _killsText;
    [SerializeField] private TMP_Text _survivedWavesText;
    [SerializeField] private TMP_Text _survivedTimeText;
    [SerializeField] private TMP_Text _silverRewardText;

    public int Kills { get; set; }
    public int SurvivedWaves { get; set; }
    public float SurvivedTimeSeconds { get; set; }

    private const float killValue = 5f;
    private const float waveValue = 25f;
    private const float timeValuePerMinute = 3f;
    private const float baseReward = 100f;

    private Sequence _sequence;

    private void Update()
    {
        if (_playerHealth == null)
        {
            return;
        }

        SurvivedTimeSeconds += Time.deltaTime;
    }

    public int CalculateSilver(int kills, int survivedWaves, float survivedTimeSeconds)
    {
        float minutes = survivedTimeSeconds / 60f;

        float reward = baseReward +
                       kills * killValue +
                       survivedWaves * waveValue +
                       minutes * timeValuePerMinute;

        float multiplier = 1f + (survivedWaves / 50f);
        reward *= multiplier;

        return Mathf.RoundToInt(reward);
    }

    public void ShowPanel()
    {
        _killsText.text = $"Kills: <color=#FF6969>{Kills}</color>";
        _survivedWavesText.text = $"Lived through Waves: <color=#6EFFA1>{SurvivedWaves}</color>";
        _survivedTimeText.text = $"Survived Time: <color=#FFC453>{GetFormattedTime(SurvivedTimeSeconds)}</color>";

        int rewardedSilver = CalculateSilver(Kills, SurvivedWaves, SurvivedTimeSeconds);
        _silverRewardText.text = $"Silver: <color=#FFFB00>{rewardedSilver}</color>";

        _sequence?.Kill();
        _sequence = DOTween.Sequence();

        _sequence.Append(_panel.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_killsText.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_survivedWavesText.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_survivedTimeText.DOFade(1, 0.5f).SetEase(Ease.OutBack));
        _sequence.Append(_silverRewardText.DOFade(1, 0.5f).SetEase(Ease.OutBack));

        _sequence?.Play();
    }

    public string GetFormattedTime(float seconds)
    {
        var time = TimeSpan.FromSeconds(seconds);

        return string.Format("{0:D2}:{1:D2}:{2:D2}",
            time.Hours, time.Minutes, time.Seconds);
    }
}