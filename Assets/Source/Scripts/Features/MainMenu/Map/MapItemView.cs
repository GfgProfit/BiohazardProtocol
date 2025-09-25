using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapItemView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [Header("UI")]
    [SerializeField] private Image _iconImage;
    [SerializeField] private TMP_Text _nameText;

    [Header("Anim")]
    [SerializeField] private float _animationDuration = 0.15f;
    [SerializeField] private float _hoverIconScale = 1.1f;
    [SerializeField] private float _clickPunch = 0.04f;

    [Header("Colors")]
    [SerializeField] private Color _normalTextColor = Color.white;
    [SerializeField] private Color _selectedTextColor = new(0.91f, 0.51f, 0.0f); // #E78300

    public bool Selected { get; private set; }
    public MapItem MapItem => _mapItem ?? null;
    public string SceneName => _mapItem?.SceneName;

    private MapManager _mapManager;
    private MapItem _mapItem;
    private Vector3 _iconDefaultScale;
    private Vector3 _rootDefaultScale;

    public void Setup(MapItem mapItem, MapManager mapManager)
    {
        _mapItem = mapItem;
        _mapManager = mapManager;

        _iconDefaultScale = _iconImage.rectTransform.localScale;
        _rootDefaultScale = transform.localScale;

        _iconImage.sprite = _mapItem.Icon;
        _nameText.text = _mapItem.Name;
        _nameText.color = _normalTextColor;

        Selected = false;
        ApplySelectedVisual();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        transform.DOKill(true);
        transform.DOPunchScale(Vector3.one * _clickPunch, _animationDuration, 1, 0.5f);

        _mapManager.Select(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        _iconImage.rectTransform.DOKill(true);
        _iconImage.rectTransform.DOScale(_iconDefaultScale * _hoverIconScale, _animationDuration).SetEase(Ease.OutCubic);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        _iconImage.rectTransform.DOKill(true);
        _iconImage.rectTransform.DOScale(_iconDefaultScale, _animationDuration).SetEase(Ease.OutCubic);
    }

    public void SetSelected(bool value)
    {
        if (Selected == value) return;
        Selected = value;
        ApplySelectedVisual();
    }

    private void ApplySelectedVisual()
    {
        _nameText.color = Selected ? _selectedTextColor : _normalTextColor;

        transform.DOKill(true);
        transform.DOScale(Selected ? _rootDefaultScale * 1.02f : _rootDefaultScale, _animationDuration).SetEase(Ease.OutCubic);
    }

    private void OnDisable()
    {
        _iconImage.rectTransform.DOKill(true);
        transform.DOKill(true);

        _iconImage.rectTransform.localScale = _iconDefaultScale;
        transform.localScale = _rootDefaultScale;
    }
}