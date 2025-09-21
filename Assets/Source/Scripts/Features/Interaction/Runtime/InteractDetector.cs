using TMPro;
using UnityEngine;

public class InteractDetector : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxDistance = 3.0f;
    [SerializeField] private LayerMask _layerMask = ~0;

    [Header("UI Prompt")]
    [SerializeField] private CanvasGroup _promptGroup;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _fadeSpeed = 12f;

    [Header("Input")]
    [SerializeField] private KeyCode _interactKey = KeyCode.F;

    private IInteractable _currentTarget;
    private float _targetAlpha = 0f;

    private void Awake()
    {
        if (_camera == null) _camera = Camera.main;
        if (_promptGroup != null) _promptGroup.alpha = 0f;
    }

    private void Update()
    {
        Ray ray = _camera != null
            ? _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f))
            : new Ray(transform.position, transform.forward);

        _currentTarget = null;

        if (Physics.Raycast(ray, out var hit, _maxDistance, _layerMask, QueryTriggerInteraction.Collide))
        {
            if (TryGetInteractable(hit.collider, out IInteractable interactable))
            {
                _currentTarget = interactable;
            }
        }

        _targetAlpha = _currentTarget != null ? 1f : 0f;

        if (_promptGroup != null)
        {
            _promptGroup.alpha = Mathf.MoveTowards(_promptGroup.alpha, _targetAlpha, _fadeSpeed * Time.deltaTime);
            _promptGroup.interactable = _promptGroup.blocksRaycasts = (_promptGroup.alpha > 0.99f);
            
            if (_currentTarget != null)
            {
                string text = string.Format($"{_currentTarget.Text}", $"<color=#FFD800>$:</color> {_currentTarget.Money}");
                _text.text = text;
                _text.ForceMeshUpdate();
                Canvas.ForceUpdateCanvases();
            }
        }

        if (_currentTarget != null && Input.GetKeyDown(_interactKey))
        {
            _currentTarget.Interact();
        }
    }

    private bool TryGetInteractable(Collider col, out IInteractable interactable)
    {
        if (col.TryGetComponent(out interactable))
            return true;

        if (col.attachedRigidbody != null &&
            col.attachedRigidbody.TryGetComponent(out interactable))
            return true;

        interactable = col.GetComponentInParent<IInteractable>();
        if (interactable != null) return true;

        interactable = col.GetComponentInChildren<IInteractable>();
        return interactable != null;
    }
}