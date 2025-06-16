using IEC;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopupFeature : BasePopup
{
    [SerializeField] private Image topBarImg;
    [SerializeField] private TextMeshProUGUI des;
    
    protected override void Awake()
    {
        base.Awake();
        AudioEventOpen = EventList.Audio.NEW_ITEM;
    }


    public override void Show()
    {
        SetData();
        base.Show();
    }

    private void SetData()
    {
        
    }

}
