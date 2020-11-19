using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGridAura : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] m_PsEffect;

    public void StartEffect(Color color)
    {
        for (int i = 0; i < m_PsEffect.Length; i++)
        {
            var ps = m_PsEffect[i];
            var main = ps.main;
            main.loop = true;
            main.startColor = color;
            ps.Play();
        }
    }

    public void DestroyEffect()
    {
        for (int i = 0; i < m_PsEffect.Length; i++)
        {
            var ps = m_PsEffect[i];
            var main = ps.main;
            main.loop = false;
        }

        StartCoroutine(OnDestroyMe());
    }

    private IEnumerator OnDestroyMe()
    {
        yield return new WaitForSeconds(3f);
        PrefabHelper.Destroy(gameObject);
    }

}
