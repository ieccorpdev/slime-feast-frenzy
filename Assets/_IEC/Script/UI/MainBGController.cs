using System.Collections;
using System.Collections.Generic;
using IEC;
using UnityEngine;
using UnityEngine.UI;

public class MainBGController : _BaseBehaviour
{
    public Image staticImage;

    public SpineAnimOT spineImage;
    // Start is called before the first frame update
    void Start()
    {
        SetupBGByTheme();
    }

    public void SetupBGByTheme()
    {
       var themeConfig = Config.themeConfig.GetWaterThemeForLevel(PlayerData.level);
       if (themeConfig.themeId == 0)
       {
           //theme id 0 have a spine background, hard code, update if update another spine bg
           staticImage.sprite = themeConfig.bgSprite;
           staticImage.gameObject.SetActive(!Game.iosFeature);
           spineImage.gameObject.SetActive(Game.iosFeature);
       }
       if (themeConfig.bgSprite != null)
       {
           spineImage.gameObject.SetActive(false);
           staticImage.gameObject.SetActive(true);
           staticImage.sprite = themeConfig.bgSprite;
       }
    }
}
