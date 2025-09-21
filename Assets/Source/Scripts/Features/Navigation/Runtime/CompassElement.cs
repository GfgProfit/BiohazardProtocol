using UnityEngine.UI; 
using UnityEngine;

namespace Compass
{
    public class CompassElement : MonoBehaviour
    {
        public Sprite _icon;
        public Color _iconColor = Color.white;

        [SerializeField] private bool _addOnStart;

        public Image Image { get; set; }

        private void Start()
        {
            if (_addOnStart)
            {
                Add();
            }
        }

        public Vector2 GetVector2Pos() => new (transform.position.x, transform.position.z);

        public void Add() => Compass.Instance.AddCompassElement(this);
        public void Remove() => Compass.Instance.RemoveCompassElement(this);
    }
}