using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class IslandComponent : MonoBehaviour
{
    private Transform Transform => transform;

    private Tween m_Tween;

    private float startEffectMoveTime = 0.4f;

    public void OnInit(Vector3 posSpawn, int size)
    {
        posSpawn.y = 15f;
        //posSpawn.y = -.1f;
        Transform.position = posSpawn;
        Transform.localScale = new Vector3(size, 1, size);

        OnTweenStart(size * 2);
    }

    private void OnTweenStart(int delay)
    {
        m_Tween?.Kill(true);
        var delayTime = (delay * .1f) + .65f;
        m_Tween = transform.DOLocalMoveY(-.1f, startEffectMoveTime).SetDelay(delayTime);
    }

    public void OnDestroyMe()
    {
        PrefabHelper.Destroy(gameObject);
    }

}
