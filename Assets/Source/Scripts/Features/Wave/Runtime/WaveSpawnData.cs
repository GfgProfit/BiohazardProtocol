using UnityEngine;

[System.Serializable]
public class WaveSpawnData
{
    [SerializeField] private BarricadeController _barricadeController;
    public BarricadeController BarricadeController => _barricadeController;

    [SerializeField] private string _requiredZoneKey;
    public string RequiredZoneKey => _requiredZoneKey;
}