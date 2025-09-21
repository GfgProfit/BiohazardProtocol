using UnityEngine;

public interface IPlayerInput
{
    Vector2 GetMovementInput();
    Vector2 GetMouseDelta();
    bool IsJumpPressed();
    bool IsSprintHeld();
}