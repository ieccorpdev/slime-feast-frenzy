using DG.Tweening;
using UnityEngine;

public class DotweenSettings : MonoBehaviour
{
    [SerializeField] private int tweenersCapacity = 200;
    [SerializeField] private int sequencesCapacity = 50;

    private void Awake()
    {
        DOTween.SetTweensCapacity(tweenersCapacity, sequencesCapacity);
    }
}
