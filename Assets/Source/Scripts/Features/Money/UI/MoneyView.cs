using DG.Tweening;
using System.Collections;
using TMPro;
using UnityEngine;

#pragma warning disable IDE0044

public class MoneyView : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;
    [SerializeField] private MoneyPopupView _moneyPopupPrefab;
    [SerializeField] private RectTransform _moneyPopupHolder;

    [Inject] private IMoney _moneyService;

    private Sequence _sequence;
    private Coroutine _waitFramesRoutine;

    private void Start()
    {
        UpdateMoneyOnScreen(_moneyService.CurrentMoney);
    }

    private void OnEnable()
    {
        _moneyService.OnChanged += UpdateMoneyOnScreen;
        _moneyService.OnAdded += CreatePopup;
    }

    private void OnDisable()
    {
        _moneyService.OnChanged -= UpdateMoneyOnScreen;
        _moneyService.OnAdded -= CreatePopup;
    }

    private void UpdateMoneyOnScreen(int value)
    {
        _moneyText.text = $"<color=#FFD800>$:</color> {Utils.FormatNumber(value, '.')}";
        AnimatePingPong(_moneyText.rectTransform, new Vector3(1.05f, 1.05f, 1.05f), Vector3.one, 0.2f);
    }

    private void CreatePopup(int value)
    {
        MoneyPopupView moneyPopup = Instantiate(_moneyPopupPrefab, _moneyPopupHolder);
        moneyPopup.Create(value);
    }

    private void AnimatePingPong(RectTransform rectTransform, Vector3 from, Vector3 to, float halfDuration)
    {
        rectTransform.DOScale(from, halfDuration)
               .OnComplete(() => rectTransform.DOScale(to, halfDuration))
               .SetEase(Ease.OutBack);
    }
}