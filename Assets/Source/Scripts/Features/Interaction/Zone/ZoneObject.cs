using Compass;
using DG.Tweening;
using UnityEngine;

public class ZoneObject : MonoBehaviour
{
    [SerializeField] private CompassElement _compassElement;

    [Space]
    [SerializeField] private float _animationDuration = 0.35f;

    public void Interact()
    {
        transform.DOLocalMoveY(-4, _animationDuration)
            .OnComplete(() =>
            {
                _compassElement.Remove();

                Destroy(gameObject);
            });
    }
}