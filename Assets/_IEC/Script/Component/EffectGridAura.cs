using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectGridAura : MonoBehaviour
{
    [SerializeField]
    private ParticleSystem[] m_PsEffect;

    public void StartEffect(Color color)
    {
        for (int i = 0; i < 3; i++)
        {
            var ps = m_PsEffect[i];
            var main = ps.main;
            main.loop = true;
            main.startColor = color;
            ps.Play();
        }
    }

    public void DisableEffect()
    {
        for (int i = 0; i < 3; i++)
        {
            var ps = m_PsEffect[i];
            var main = ps.main;
            main.loop = false;
        }
    }

    public void OnDestroyMe()
    {
        for (int i = 0; i < 3; i++)
        {
            m_PsEffect[i].Stop();
        }
    }

}
