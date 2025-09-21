using UnityEngine;
using UnityEngine.UI;

public class StatusStat : MonoBehaviour
{
    public Image fill;

    [Space]
    public float value = 100;

    [Space]
    public float maxValue = 100;
    public float minValue = 0;

    [Space]
    public float increasePerSecond;
    public float decreasePerSecond;

    public float DefaultIncreasePerSecond { get; private set; }
    public float DefaultDecreasePerSecond { get; private set; }

    public void Awake()
    {
        InitStat();
    }

    private void InitStat()
    {
        value = maxValue;
        
        DefaultIncreasePerSecond = increasePerSecond;
        DefaultDecreasePerSecond = decreasePerSecond;

        DrawUI();
    }

    public void Increase(float valuePerSecond)
    {
        if (value < maxValue)
        {
            value += valuePerSecond * Time.deltaTime;
            DrawUI();
        }
    }

    public void Increase()
    {
        if (value < maxValue)
        {
            value += increasePerSecond * Time.deltaTime;
            DrawUI();
        }
    }

    public void Decrease(float valuePerSecond)
    {
        if (value > minValue)
        {
            value -= valuePerSecond * Time.deltaTime;
            DrawUI();
        }
    }

    public void Decrease()
    {
        if (value > minValue)
        {
            value -= decreasePerSecond * Time.deltaTime;
            DrawUI();
        }
    }

    private void DrawUI() => fill.fillAmount = value / maxValue;
}