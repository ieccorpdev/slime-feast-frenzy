using System.Collections;
using UnityEngine;

public class SplashVFX : MonoBehaviour
{
    public ParticleSystem effectParticle;
    private Coroutine recycleCoroutine;

    public void Show()
    {
        effectParticle.Play();

        if (recycleCoroutine != null)
        {
            StopCoroutine(recycleCoroutine);
        }

        recycleCoroutine = StartCoroutine(DelayedRecycle(2f));
    }

    private IEnumerator DelayedRecycle(float delay)
    {
        yield return new WaitForSeconds(delay);
        gameObject.Recycle();
        recycleCoroutine = null;
    }

    private void OnDisable()
    {
        if (recycleCoroutine != null)
        {
            StopCoroutine(recycleCoroutine);
            recycleCoroutine = null;
        }
    }

    public void Reset()
    {
        gameObject.SetActive(false);
    }
}
