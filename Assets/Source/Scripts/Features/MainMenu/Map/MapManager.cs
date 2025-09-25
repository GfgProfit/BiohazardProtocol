using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    [SerializeField] private MapItem[] _mapItems;

    [Header("UI")]
    [SerializeField] private MapItemView _mapItemViewPrefab;
    [SerializeField] private RectTransform _mapHolder;
    [SerializeField] private bool _selectFirstByDefault = true;

    private readonly List<MapItemView> _views = new();
    private MapItemView _selected;

    private void Start()
    {
        _views.Clear();

        for (int i = 0; i < _mapItems.Length; i++)
        {
            var view = Instantiate(_mapItemViewPrefab, _mapHolder);
            view.Setup(_mapItems[i], this);
            _views.Add(view);
        }

        if (_selectFirstByDefault && _views.Count > 0)
        {
            Select(_views[0]);
        }
    }

    public void Select(MapItemView view)
    {
        if (view == null) return;
        if (_selected == view) return;

        if (_selected != null)
            _selected.SetSelected(false);

        _selected = view;
        _selected.SetSelected(true);
    }

    public void ClearSelection()
    {
        if (_selected == null) return;
        _selected.SetSelected(false);
        _selected = null;
    }

    public void StartGame()
    {
        if (_selected == null)
        {
            Debug.LogWarning("[MapManager] Нельзя стартовать игру — карта не выбрана.");
            return;
        }

        string scene = _selected.SceneName;
        if (string.IsNullOrWhiteSpace(scene))
        {
            Debug.LogError("[MapManager] У выбранной карты пустое SceneName.");
            return;
        }

        SceneLoader.Load(_selected.MapItem);
    }
}
