using UnityEngine;

[System.Serializable]
public class WaveSpawnData
{
    [SerializeField] private BarricadeController _barricadeController;
    public BarricadeController BarricadeController => _barricadeController;

    [SerializeField] private BarricadeSlots _barricadeSlots;
    public BarricadeSlots BarricadeSlots => _barricadeSlots;

    [SerializeField] private string _requiredZoneKey;
    public string RequiredZoneKey => _requiredZoneKey;
}