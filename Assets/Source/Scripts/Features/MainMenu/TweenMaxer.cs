using DG.Tweening;
using UnityEngine;

[DefaultExecutionOrder(-1000)]
public class TweenMaxer : MonoBehaviour
{
    private void Awake()
    {
        DOTween.SetTweensCapacity(500, 100);
    }
}