using UnityEngine;

[System.Serializable]
public class WaveSpawnData
{
    [SerializeField] private Transform _spawnPoint;
    public Transform SpawnPoint => _spawnPoint;

    [SerializeField] private Transform _pointToBarricade;
    public Transform PointToBarricade => _pointToBarricade;

    [SerializeField] private string _requiredZoneKey;
    public string RequiredZoneKey => _requiredZoneKey;
}