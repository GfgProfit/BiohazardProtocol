using UnityEngine;

public class AimDetector : MonoBehaviour
{
    [Header("Aim")]
    [SerializeField] private Camera _cam;
    [SerializeField] private float _maxDistance = 50f;
    [SerializeField] private LayerMask _hitMask;

    private ITargetable _current;

    private void Update()
    {
        Ray ray = new(_cam.transform.position, _cam.transform.forward);
        ITargetable detected = null;

        if (Physics.Raycast(ray, out RaycastHit hit, _maxDistance, _hitMask, QueryTriggerInteraction.Ignore))
        {
            detected = hit.collider.GetComponentInParent<ITargetable>();
        }

        if (!ReferenceEquals(detected, _current))
        {
            _current?.Unfocus();
            _current = detected;
            _current?.Focus();
        }
    }
}