using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Compass : MonoBehaviour
{
    [SerializeField] private RawImage _compassRawImage;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private RectTransform _elementsHolder;
    [SerializeField] private TMP_Text _compassAngleText;
    [SerializeField] private CompassElement _compassElementIconPrefab;

    private readonly List<CompassElement> _compassElements = new List<CompassElement>();

    private void Update()
    {
        _compassRawImage.uvRect = new Rect(_playerTransform.localEulerAngles.y / 360, 0, 1, 1);
        _compassAngleText.text = _playerTransform.localEulerAngles.y.ToString("F0");// + "°";

        foreach (CompassElement element in _compassElements)
        {
            element.Image.rectTransform.anchoredPosition = GetElementPositionInCompass(element);
        }
    }

    private Vector2 GetElementPositionInCompass(CompassElement element)
    {
        Vector2 playerPosition = new(_playerTransform.position.x, _playerTransform.position.z);
        Vector2 playerForward = new(_playerTransform.forward.x, _playerTransform.forward.z);

        float angle = Vector2.SignedAngle(element.GetVector2Pos() - playerPosition, playerForward);

        return new Vector2(angle * _compassRawImage.rectTransform.rect.width / 360, 0);
    }

    public void AddCompassElement(CompassElement element)
    {
        CompassElement iconInstance = Instantiate(_compassElementIconPrefab, _elementsHolder);
        iconInstance.transform.localPosition = Vector3.zero;

        element.Setup(iconInstance);

        _compassElements.Add(element);
    }

    public void RemoveCompassElement(CompassElement element)
    {
        _compassElements.Remove(element);

        if (element.IconRoot != null)
            Destroy(element.IconRoot.gameObject);
    }
}