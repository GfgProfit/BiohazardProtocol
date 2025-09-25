using DG.Tweening;
using TMPro;
using UnityEngine;
using EPOOutline;

public class InteractDetector : MonoBehaviour
{
    [Header("Raycast")]
    [SerializeField] private Camera _camera;
    [SerializeField] private float _maxDistance = 3.0f;
    [SerializeField] private LayerMask _layerMask = ~0;

    [Header("UI Prompt")]
    [SerializeField] private CanvasGroup _promptGroup;
    [SerializeField] private RectTransform _promptTransform;
    [SerializeField] private TMP_Text _text;
    [SerializeField] private float _fadeSpeed = 12f;

    [Header("Input")]
    [SerializeField] private KeyCode _interactKey = KeyCode.F;

    public bool CanInteract { get; set; } = true;

    private IInteractable _currentTarget;
    private IInteractable _previousTarget;
    private float _targetAlpha = 0f;

    private IHoldInteractable _holdingTarget;

    private void Awake()
    {
        if (_camera == null)
            _camera = Camera.main;

        if (_promptGroup != null)
            _promptGroup.alpha = 0f;
    }

    private void Update()
    {
        if (!CanInteract)
        {
            StopHoldingIfAny();

            if (_previousTarget != null)
            {
                ToggleOutline(_previousTarget, false);
                _previousTarget = null;
            }

            if (_promptGroup != null)
            {
                _promptGroup.alpha = Mathf.MoveTowards(_promptGroup.alpha, 0f, _fadeSpeed * Time.deltaTime);
                _promptGroup.interactable = _promptGroup.blocksRaycasts = false;
            }

            _currentTarget = null;
            return;
        }

        Detect();

        IHoldInteractable holdable = _currentTarget as IHoldInteractable;

        if (holdable != null && _currentTarget != null && _currentTarget.CanInteract)
        {
            if (Input.GetKeyDown(_interactKey))
            {
                _holdingTarget = holdable;
                _holdingTarget.BeginHold();
            }

            if (Input.GetKey(_interactKey))
            {
                _holdingTarget?.TickHold(Time.deltaTime);
            }

            if (Input.GetKeyUp(_interactKey))
            {
                _holdingTarget?.EndHold();
                _holdingTarget = null;

                if (_promptTransform != null)
                {
                    AnimatePingPongPopUp(_promptTransform, new Vector3(0.9f, 0.9f, 0.9f), Vector3.one, 0.1f);
                }
            }
        }
        else
        {
            if (_currentTarget != null && _currentTarget.CanInteract && Input.GetKeyDown(_interactKey))
            {
                _currentTarget.Interact();
                AnimatePingPongPopUp(_promptTransform, new Vector3(0.9f, 0.9f, 0.9f), Vector3.one, 0.1f);
            }
        }
    }

    private void StopHoldingIfAny()
    {
        if (_holdingTarget != null)
        {
            _holdingTarget.EndHold();
            _holdingTarget = null;
        }
    }

    private void Detect()
    {
        Ray ray = _camera != null ? _camera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f)) : new Ray(transform.position, transform.forward);

        _currentTarget = null;

        if (Physics.Raycast(ray, out var hit, _maxDistance, _layerMask, QueryTriggerInteraction.Collide))
        {
            if (TryGetInteractable(hit.collider, out IInteractable interactable))
            {
                if (interactable.CanInteract)
                {
                    _currentTarget = interactable;
                }
            }
        }

        if (_currentTarget != _previousTarget)
        {
            if (_holdingTarget != null && !ReferenceEquals(_holdingTarget, _currentTarget))
            {
                _holdingTarget.EndHold();
                _holdingTarget = null;
            }

            ToggleOutline(_previousTarget, false);
            ToggleOutline(_currentTarget, true);
            _previousTarget = _currentTarget;
        }

        _targetAlpha = (_currentTarget != null && _currentTarget.CanInteract) ? 1f : 0f;

        if (_promptGroup != null)
        {
            _promptGroup.alpha = Mathf.MoveTowards(_promptGroup.alpha, _targetAlpha, _fadeSpeed * Time.deltaTime);
            _promptGroup.interactable = _promptGroup.blocksRaycasts = (_promptGroup.alpha > 0.99f);

            if (_currentTarget != null)
            {
                string str1 = $"{_currentTarget.Text}";
                string str2 = $"<color=#FFD800>$: </color>{_currentTarget.Money}";
                _text.text = string.Format(str1, str2);
            }
        }
    }

    private void AnimatePingPongPopUp(RectTransform rectTransform, Vector3 from, Vector3 to, float halfDuration)
    {
        if (rectTransform == null)
        {
            return;
        }

        rectTransform.DOScale(from, halfDuration)
            .OnComplete(() => rectTransform.DOScale(to, halfDuration))
            .SetEase(Ease.OutBack);
    }

    private bool TryGetInteractable(Collider col, out IInteractable interactable)
    {
        if (col.TryGetComponent(out interactable))
        {
            return true;
        }

        if (col.attachedRigidbody != null && col.attachedRigidbody.TryGetComponent(out interactable))
        {
            return true;
        }

        interactable = col.GetComponentInParent<IInteractable>();
        if (interactable != null)
        {
            return true;
        }

        interactable = col.GetComponentInChildren<IInteractable>();
        return interactable != null;
    }

    private void ToggleOutline(IInteractable target, bool enable)
    {
        if (target == null)
        {
            return;
        }

        MonoBehaviour mono = target as MonoBehaviour;
        if (mono == null)
        {
            return;
        }

        if (!mono.TryGetComponent(out Outlinable outline))
        {
            outline = mono.GetComponentInChildren<Outlinable>();
        }

        if (outline != null)
        {
            outline.enabled = enable;
        }
    }
}