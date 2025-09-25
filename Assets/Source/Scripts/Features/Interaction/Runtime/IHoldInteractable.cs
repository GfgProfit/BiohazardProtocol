public interface IHoldInteractable : IInteractable
{
    void BeginHold();
    void TickHold(float deltaTime);
    void EndHold();
    float HoldProgress01 { get; }
    bool IsOnCooldown { get; }
}