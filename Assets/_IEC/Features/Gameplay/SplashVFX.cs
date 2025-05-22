using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class SplashVFX : MonoBehaviour
{
    public ParticleSystem effectParticle;
    private string TweenID => gameObject.GetInstanceID() + "_Particle";

    public void Show()
    {
        effectParticle.Play();
        DOTween.Sequence().AppendInterval(2f).OnComplete(() =>
        {
            gameObject.Recycle();
        });
    }

    private void OnDisable()
    {
        DOTween.Kill(TweenID);
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}
