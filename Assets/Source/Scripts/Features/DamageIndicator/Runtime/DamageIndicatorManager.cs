using UnityEngine;

public class DamageIndicatorManager : MonoBehaviour
{
    [SerializeField] private DamageIndicator _damageIndicatorPrefab;
    [SerializeField] private RectTransform _holder;
    [SerializeField] private Transform _playerTransform;

    public void CreateIndicator(Vector3 attackerPosition)
    {
        DamageIndicator indicator = Instantiate(_damageIndicatorPrefab, _holder);
        indicator.Setup(attackerPosition, _playerTransform);
    }
}