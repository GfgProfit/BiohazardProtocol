using UnityEngine;

public class Impact : MonoBehaviour
{
    [SerializeField] private GameObject _impactPrefab;

    public GameObject ImpactPrefab => _impactPrefab;
}