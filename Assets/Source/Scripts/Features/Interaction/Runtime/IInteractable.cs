using EPOOutline;

public interface IInteractable
{
    string Text { get; set; }
    int Money { get; set; }
    void Interact();
    bool CanInteract { get; set; }
}
