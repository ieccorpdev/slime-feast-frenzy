using IEC;
using TMPro;
using UnityEngine;
using System.Collections;

public class PopupVoyagePassIAPInfor : BasePopup
{
    public GameObject activeGroup, inactiveGroup;
    public BundlePackUI configBundle;
    public TextMeshProUGUI timerTxt;

    protected override void Awake()
    {
        base.Awake();
        configBundle.OnPurchaseSuccess += () =>
        {
            Hide();
            activeGroup.SetActive(true);
            inactiveGroup.SetActive(false);
        };
    }

    public override void Show()
    {
        base.Show();
        activeGroup.SetActive(PlayerData.journeyData.isBuyIAP);
        inactiveGroup.SetActive(!PlayerData.journeyData.isBuyIAP);
        StartCoroutine(UpdateTimer());
    }

    public override void Hide()
    {
        base.Hide();
        StopAllCoroutines();
    }

    IEnumerator UpdateTimer()
    {
        timerTxt.text = PlayerData.journeyData.GetTimeRemainingString();
        bool value = PlayerData.journeyData.isBuyIAP;
        activeGroup.gameObject.SetActive(value);
        inactiveGroup.gameObject.SetActive(!value);
        yield return new WaitForSeconds(1f);
    }
}
