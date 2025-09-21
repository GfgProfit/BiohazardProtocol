using UnityEngine;
using TMPro;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private float _walkSpeed = 3.0f;
    [SerializeField] private float _sprintSpeed = 6.0f;
    [SerializeField] private float _percentStaminaReset = 33.0f;

    [SerializeField] private float _jumpForce = 5f;
    [SerializeField] private float _gravity = 9.81f;

    [SerializeField] private Vector2 _cameraClampLimit = new(-90.0f, 90.0f);

    [Space]
    [SerializeField] private float _defaultCameraFieldOfView = 60.0f;
    [SerializeField] private float _sprintCameraFieldOfView = 70.0f;
    [SerializeField] private float _fieldOfViewSmooth = 5.0f;

    [Space]
    [SerializeField] private float _mouseSensitivity = 2.0f;

    [SerializeField] private CharacterController _characterController;
    [SerializeField] private CrosshairController _crosshairController;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private StatusStat _staminaStat;
    #endregion

    #region Private Fields
    private float _xRotation;
    private float _currentSpeed;
    private Vector3 _rawInput;
    private float _verticalVelocity;
    private bool _sprintLocked;
    private IPlayerInput _input;
    #endregion

    #region Properties
    public bool IsSprinting { get; private set; }
    public bool CanSprinting { get; private set; }
    public bool IsWalking { get; private set; }
    #endregion

    #region Unity Methods
    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        _input = new LegacyPlayerInput();

        _currentSpeed = _walkSpeed;
    }

    private void OnValidate()
    {
        _characterController = GetComponent<CharacterController>();
        _mainCamera = Camera.main;
        _cameraTransform = _mainCamera.transform;
    }

    private void Update()
    {
        Look();
        SetRawInput();
        UpdateMotionState();
        Move();
        UpdateCameraFieldOfView();
    }
    #endregion

    #region Core Logic
    private void Look()
    {
        Vector2 mouseDelta = _input.GetMouseDelta();
        float mouseX = mouseDelta.x * _mouseSensitivity;
        float mouseY = mouseDelta.y * _mouseSensitivity;

        _xRotation = Mathf.Clamp(_xRotation - mouseY, _cameraClampLimit.x, _cameraClampLimit.y);

        _cameraTransform.localRotation = Quaternion.Euler(_xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX);
    }

    private void SetRawInput()
    {
        Vector2 input = _input.GetMovementInput();
        _rawInput = new Vector3(input.x, _rawInput.y, input.y);
    }

    private void Move()
    {
        float speed = CalculateTargetSpeed();

        if (_characterController.isGrounded)
        {
            if (_verticalVelocity < 0f)
            {
                _verticalVelocity = -2f;
            }

            if (_input.IsJumpPressed())
            {
                _verticalVelocity = _jumpForce;
            }
        }

        _verticalVelocity -= _gravity * Time.deltaTime;

        Vector3 move = (transform.right * _rawInput.x + transform.forward * _rawInput.z).normalized * speed;
        move.y = _verticalVelocity;

        _characterController.Move(move * Time.deltaTime);
    }

    private float CalculateTargetSpeed()
    {
        float targetSpeed = IsSprinting ? _sprintSpeed : _walkSpeed;
        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, _fieldOfViewSmooth * Time.deltaTime);
        
        return _currentSpeed;
    }

    private void UpdateCameraFieldOfView()
    {
        float targetFOV = IsSprinting ? _sprintCameraFieldOfView : _defaultCameraFieldOfView;
        _mainCamera.fieldOfView = Mathf.Lerp(_mainCamera.fieldOfView, targetFOV, _fieldOfViewSmooth * Time.deltaTime);
    }

    private void UpdateMotionState()
    {
        IsWalking = _rawInput.x != 0f || _rawInput.z != 0f;

        float unlockThreshold = _staminaStat.maxValue * (_percentStaminaReset / 100.0f);

        if (_sprintLocked && _staminaStat.value >= unlockThreshold)
        {
            _sprintLocked = false;
        }

        if (_staminaStat.value < unlockThreshold && !_input.IsSprintHeld())
        {
            _sprintLocked = true;
        }

        CanSprinting = !_sprintLocked;
        IsSprinting = CanSprinting && IsWalking && _input.IsSprintHeld() && _rawInput.z > 0;

        if (IsSprinting)
        {
            _staminaStat.Decrease(7.5f);
            _crosshairController.Sprint();
        }
        else
        {
            _staminaStat.Increase(5.0f);
        }

        if (IsWalking && !IsSprinting)
        {
            _crosshairController.Walk();
        }

        if (!IsSprinting && !IsWalking)
        {
            _crosshairController.SetIdle();
        }
    }
    #endregion
}