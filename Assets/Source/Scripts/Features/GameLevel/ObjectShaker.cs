using UnityEngine;

public class ObjectShaker : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float _amplitude = 0.05f;
    [SerializeField] private float _frequency = 5f;
    [SerializeField] private Vector3 _shakeAxis = Vector3.right;

    private Vector3 _startPos;
    private float _timeOffset;

    private void Awake()
    {
        _startPos = transform.localPosition;

        _timeOffset = Random.Range(0f, 100f);
    }

    private void Update()
    {
        float shake = Mathf.Sin((Time.time + _timeOffset) * _frequency) * _amplitude;
        transform.localPosition = _startPos + _shakeAxis.normalized * shake;
    }
}
