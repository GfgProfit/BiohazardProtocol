using System.Collections;
using UnityEngine;

public class DamageIndicator : MonoBehaviour
{
    [SerializeField] private Transform _damageImagePivot;
    
    private Vector3 _damageLocation;
    private Transform _playerTransform;

    private void Start()
    {
        StartCoroutine(Destroy());
    }

    private void Update()
    {
        _damageLocation.y = _playerTransform.position.y;
        Vector3 direction = (_damageLocation - _playerTransform.position).normalized;
        float angle = (Vector3.SignedAngle(direction, _playerTransform.forward, Vector3.up));
        _damageImagePivot.transform.localEulerAngles = new(0, 0, angle);
    }

    public void Setup(Vector3 damageLocation, Transform playerTransform)
    {
        _damageLocation = damageLocation;
        _playerTransform = playerTransform;
    }

    private IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3);

        Destroy(gameObject);
    }
}