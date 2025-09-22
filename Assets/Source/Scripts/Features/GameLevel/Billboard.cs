using UnityEngine;

public class Billboard : MonoBehaviour
{
    private Camera _playerCamera;

    private void Start() => _playerCamera = Camera.main;

    private void LateUpdate()
    {
        if (_playerCamera == null)
        {
            return;
        }

        transform.rotation = _playerCamera.transform.rotation;
    }
}