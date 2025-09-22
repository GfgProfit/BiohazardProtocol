using UnityEngine;
using UnityEngine.UI;

public class CompassElement : MonoBehaviour
{
    [SerializeField] private Sprite _icon;
    [SerializeField] private Color _iconColor = Color.white;
    [SerializeField] private bool _addOnStart;

    private Image _image;
    private RectTransform _iconRoot;
    public Image Image => _image;
    public RectTransform IconRoot => _iconRoot;

    [Inject] private Compass _compass;

    private void Start()
    {
        if (_addOnStart)
            Add();
    }

    public void Setup(CompassElement iconInstance)
    {
        _iconRoot = iconInstance.GetComponent<RectTransform>();
        _image = iconInstance.transform.GetChild(0).GetComponent<Image>();
        _image.sprite = _icon;
        _image.color = _iconColor;
    }

    public Vector2 GetVector2Pos() => new(transform.position.x, transform.position.z);

    public void Add() => _compass.AddCompassElement(this);
    public void Remove() => _compass.RemoveCompassElement(this);
}