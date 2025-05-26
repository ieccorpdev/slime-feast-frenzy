using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Gameplay;
using IEC;
using UnityEngine;
using UnityUtils;

public enum WeatherTheme
{
    NONE,
    SNOW,
    SNOW_STORM,
    RAIN,
    WIND,
    LEAVES
}
[Serializable]
public class WeatherConfig
{
    public WeatherTheme WeatherName;
    public ParticleSystem weatherParticle;
}
public class ThemeController :  _BaseBehaviour, IGroup
{
    private WaterThemeConfig dataTheme;
    [SerializeField] private MeshRenderer waterGround;
    [SerializeField] private GameObject whirlPoolHole;
    [SerializeField] private ParticleSystem splashEffect;
    [Header("Weather")] 
    public List<WeatherConfig> dataWeatherScene;
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
            var newWhirlPool = Instantiate(dataTheme.whirlPoolHole, whirlPoolHole.transform.parent);
            newWhirlPool.transform.CopyLocalTransform(whirlPoolHole.transform);
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
    public void DeactiveAllWeather()
    {
        foreach (var dt in dataWeatherScene)
        {
            dt.weatherParticle.gameObject.SetActive(false);   
        }
    }
    public void ActiveWeatherEffect(WeatherTheme name)
    {
        DeactiveAllWeather();
        var dataWeather = dataWeatherScene.Find(x => x.WeatherName == name);
        if (dataWeather !=null)
        {
            dataWeather.weatherParticle.gameObject.SetActive(true);
            dataWeather.weatherParticle.Play();
        }
    }
}
