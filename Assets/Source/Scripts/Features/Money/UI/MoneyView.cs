using TMPro;
using UnityEngine;

#pragma warning disable IDE0044

public class MoneyView : MonoBehaviour
{
    [SerializeField] private TMP_Text _moneyText;

    [Inject]
    private IMoney _moneyService;

    private void Start()
    {
        UpdateMoneyOnScreen(_moneyService.CurrentMoney);
    }

    private void OnEnable()
    {
        _moneyService.OnChanged += UpdateMoneyOnScreen;
    }

    private void OnDisable()
    {
        _moneyService.OnChanged -= UpdateMoneyOnScreen;
    }

    private void UpdateMoneyOnScreen(int value)
    {
        _moneyText.text = $"<color=#FFD800>$:</color> {Utils.FormatNumber(value, '.')}";
    }
}