using UnityEngine;

public interface ICrosshair
{
    CanvasGroup Group { get; }
    void Shoot();
}