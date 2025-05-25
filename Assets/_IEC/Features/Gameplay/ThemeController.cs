using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;
using UnityUtils;

public class ThemeController :  _BaseBehaviour, IGroup
{
    private WaterThemeConfig dataTheme;
    [SerializeField] private MeshRenderer waterGround;
    [SerializeField] private GameObject whirlPoolHole;
    [SerializeField] private ParticleSystem splashEffect;
    public void LocateGroup()
    {
        Pool.Set(this);
    }

    public void LoadWaterTheme(WaterThemeConfig dataTheme)
    {
        this.dataTheme = dataTheme;
        waterGround.material = dataTheme.waterSeaMaterial;
        if (whirlPoolHole != null)
        {
            var newWhirlPool = Instantiate(dataTheme.whirlPoolHole, whirlPoolHole.transform.parent, true);
            newWhirlPool.transform.CopyTransform(whirlPoolHole.transform);
            Destroy(whirlPoolHole.gameObject);
            whirlPoolHole = newWhirlPool;
        }
        if (splashEffect != null)
        {
            var newSplashEffect = Instantiate(dataTheme.splashEffect, splashEffect.transform.parent, true);
            newSplashEffect.transform.CopyTransform(splashEffect.transform);
            Destroy(splashEffect.gameObject);
            splashEffect = newSplashEffect;
            GamePlay.levelController.GetHolePlayer().waterSplashFx = splashEffect;
        }
    }
}
