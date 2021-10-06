using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using IEC;

public class AutoTest : _BaseBehaviour
{
    public TMPro.TMP_Text helloTxt;

    private ABTestConfig m_LastAb;
    public ABTestConfig LastAb
    {
        get
        {
            if (m_LastAb == null) m_LastAb = Services.config.abTestConfigs.LastOrDefault();
            return m_LastAb;
        }
    }

    public int GetAmountOfAb()
    {
        return LastAb.ListEventsAB.Length;
    }

    public void SetAbValue(int abValue)
    {
        Services.DataNormal().SetInt(LastAb.KeyName, abValue);
        ReloadTxt();
    }

    void Start()
    {
        ReloadTxt();
    }

    void ReloadTxt()
    {
        var version = Services.DataNormal().GetInt(LastAb.KeyName, 0);
        helloTxt.text = LastAb.ListEventsAB[version];
    }
}
