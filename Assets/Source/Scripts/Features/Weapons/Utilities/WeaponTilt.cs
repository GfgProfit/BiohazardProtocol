using UnityEngine;

public class WeaponTilt : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;

    [Space]
    [SerializeField] private float _tiltSpeed = 4.0f;
    [SerializeField] private float _tiltAmount = 15.0f;

    private void Update()
    {
        var rotation = Tilt();
        transform.localRotation = Quaternion.Lerp(transform.localRotation, rotation, Time.deltaTime * _tiltSpeed);
    }

    private Quaternion Tilt()
    {
        if (!_playerController.IsWalking)
            return Quaternion.Euler(Vector3.zero);

        float inputX = Input.GetAxisRaw("Horizontal");

        Vector3 vector = new Vector3(0, 0, -inputX).normalized * _tiltAmount;

        return Quaternion.Euler(vector);
    }
}