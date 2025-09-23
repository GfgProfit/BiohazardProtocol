using UnityEngine;
using Random = UnityEngine.Random;

public class WeaponRecoilManager : MonoBehaviour
{
    private Vector3 _targetRotation;
    private Vector3 _currentRotation;

    public void Tick(float returnSpeed, float snappiness)
    {
        _targetRotation = Vector3.Lerp(_targetRotation, Vector3.zero, returnSpeed * Time.deltaTime);
        _currentRotation = Vector3.Slerp(_currentRotation, _targetRotation, snappiness * Time.deltaTime);
        transform.localRotation = Quaternion.Euler(_currentRotation);
    }

    public void RecoilShoot(float recoilX, float recoilY, float recoilZ)
    {
        _targetRotation += new Vector3(-recoilX, Random.Range(-recoilY, recoilY), Random.Range(-recoilZ, recoilZ));
    }
}