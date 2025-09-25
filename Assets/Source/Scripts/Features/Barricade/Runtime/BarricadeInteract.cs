using UnityEngine;

public class BarricadeInteract : MonoBehaviour, IHoldInteractable
{
    [Header("Reward")]
    [SerializeField] private int _moneyPerBuildOnePlank = 50;

    [Header("Links")]
    [SerializeField] private BarricadeController _controller;
    [SerializeField] private MoneySound _moneySound;

    [Header("UX")]
    [SerializeField] private float _requiredHoldSeconds = 1.0f;
    [SerializeField] private float _cooldownSeconds = 2.0f;

    [SerializeField] private string _text = "Repair Barricade";

    public string Text
    {
        get
        {
            return _text;
        }
        set { }
    }

    public int Money { get; set; }
    public bool CanInteract { get; set; } = true;

    [Inject] private IMoney _moneyService;

    private float _holdTimer;
    private float _cooldownUntil;
    private int _maxPlanksAtStart;

    public float HoldProgress01 => Mathf.Clamp01(_requiredHoldSeconds <= 0f ? 1f : (_holdTimer / _requiredHoldSeconds));
    public bool IsOnCooldown => Time.time < _cooldownUntil;

    private bool IsFullyRepaired => _controller != null && _controller.RemainingPlanks >= _maxPlanksAtStart;

    private void Awake()
    {
        _maxPlanksAtStart = _controller != null ? _controller.RemainingPlanks : 0;
    }

    private void Update()
    {
        CanInteract = !IsFullyRepaired;
    }

    public void Interact() { }

    public void BeginHold()
    {
        if (!CanInteract)
        {
            return;
        }

        _holdTimer = 0f;
    }

    public void TickHold(float deltaTime)
    {
        if (!CanInteract)
        {
            return;
        }

        if (IsFullyRepaired)
        {
            return;
        }

        if (IsOnCooldown)
        {
            return;
        }

        _holdTimer += deltaTime;

        if (_holdTimer >= _requiredHoldSeconds)
        {
            TryRepairOnce();

            _holdTimer = 0f;

            _cooldownUntil = Time.time + _cooldownSeconds;
        }
    }

    public void EndHold()
    {
        _holdTimer = 0f;
    }

    private void TryRepairOnce()
    {
        if (_controller == null)
        {
            return;
        }

        if (IsFullyRepaired)
        {
            return;
        }

        _controller.BuildPlank();
        _moneyService?.AddMoney(_moneyPerBuildOnePlank);
        _moneySound.PlaySound();
    }
}
