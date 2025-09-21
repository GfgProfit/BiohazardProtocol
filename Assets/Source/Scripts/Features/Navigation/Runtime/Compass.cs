using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

namespace Compass
{
    public class Compass : MonoBehaviour
    {
        public static Compass Instance;

        [SerializeField] private RawImage _compassRawImage;
        [SerializeField] private Transform _playerTransform;
        [SerializeField] private RectTransform _elementsHolder;
        [SerializeField] private TMP_Text _compassAngleText;
        [SerializeField] private CompassElement _compassElementIconPrefab;

        private List<CompassElement> _compassElements = new List<CompassElement>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this);
            }
            else
            {
                Instance = this;
            }
        }

        private void Update()
        {
            _compassRawImage.uvRect = new Rect(_playerTransform.localEulerAngles.y / 360, 0, 1, 1);
            _compassAngleText.text = _playerTransform.localEulerAngles.y.ToString("F0");// + "°";

            foreach (CompassElement element in _compassElements)
            {
                element.Image.rectTransform.anchoredPosition = GetElementPositionInCompass(element);
            }
        }

        public void AddCompassElement(CompassElement element)
        {
            CompassElement newElement = Instantiate(_compassElementIconPrefab, _elementsHolder);
            newElement.transform.localPosition = Vector3.zero;

            element.Image = newElement.transform.GetChild(0).GetComponent<Image>();
            element.Image.sprite = element._icon;
            element.Image.color = element._iconColor;

            _compassElements.Add(element);
        }

        public void RemoveCompassElement(CompassElement element)
        {
            _compassElements.Remove(element);
            Destroy(element.Image);
        }

        private Vector2 GetElementPositionInCompass(CompassElement element)
        {
            Vector2 playerPosition = new Vector2(_playerTransform.position.x, _playerTransform.position.z);
            Vector2 playerForward = new Vector2(_playerTransform.forward.x, _playerTransform.forward.z);

            float angle = Vector2.SignedAngle(element.GetVector2Pos() - playerPosition, playerForward);

            return new Vector2(angle * _compassRawImage.rectTransform.rect.width / 360, 0);
        }
    }
}