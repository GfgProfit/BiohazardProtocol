using UnityEngine;

public class MainMenuControl : MonoBehaviour
{
    [SerializeField] private MapManager _mapManager;

    public void DeselectMap()
    {
        _mapManager.ClearSelection();
    }

    public void Exit()
    {
        Application.Quit();
    }
}