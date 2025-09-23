using UnityEngine;

public class WeaponSway : MonoBehaviour
{
    [SerializeField] private float _swayAmount = 4.0f;
    [SerializeField] private float _smoothTime = 10.0f;

    private void Update()
    {
        float inputX = Input.GetAxis("Mouse X");
        float inputY = Input.GetAxis("Mouse Y");

        var mouseX = inputX * _swayAmount;
        var mouseY = inputY * _swayAmount;

        var rotationX = Quaternion.AngleAxis(mouseY, Vector3.right);
        var rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        var targetRotation = rotationX * rotationY;

        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, _smoothTime * Time.deltaTime);
    }
}