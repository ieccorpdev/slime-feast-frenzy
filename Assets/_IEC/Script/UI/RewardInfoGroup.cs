using DG.Tweening;
using IEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardInfoGroup : ScreenUI
{
    [Header("Reward Info Group 6 item")]
    [SerializeField] List<GameObject> bgRewardInfos;
    [SerializeField] List<GameEventRewardInfoUI> listRewardInfoUI = new();
    [SerializeField] private GameObject mainPopup;

    public override void Show()
    {
        if (ItemPassJourney.currentInfo != null)
        {
            ItemPassJourney.currentInfo.SetActive(false);
        }
        mainPopup.transform.localScale = Vector3.zero;
        mainPopup.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        this.transform.parent = Locator.Main.PopupJourney.contentPanel;
        this.transform.SetAsLastSibling();
        gameObject.SetActive(true);
        StartCoroutine(WaitToHide());
        ItemPassJourney.currentInfo = gameObject;
    }

    IEnumerator WaitToHide()
    {
        yield return new WaitForSeconds(3f);
        Hide();
    }

    public override void Hide()
    {
        if (mainPopup != null)
        {
            mainPopup.transform.DOScale(Vector3.zero, 0.4f).SetEase(Ease.InBack).OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
    }

    public void SetupDataItem(List<Reward> dataItem)
    {
        if (dataItem == null || dataItem.Count == 0)
        {
            return;
        }
        GameObject parent = this.gameObject;
        foreach(var bg in bgRewardInfos)
        {
            bg.SetActive(false);
        }
        switch (dataItem.Count)
        {
            case > 6:
                parent = bgRewardInfos[0];
                break;
            case > 4:
                parent = bgRewardInfos[1];
                break;
            case < 5:
                parent = bgRewardInfos[6 - dataItem.Count];
                break;
        }
        if (mainPopup == null)
            mainPopup = parent;
        parent.SetActive(true);

        foreach (var rewardUI in listRewardInfoUI)
        {
            Reward rewardData;
            if ((rewardData = dataItem.Find(data => data.typeReward == rewardUI.resourceType)) != null)
            {
                rewardUI.gameObject.SetActive(true);
                rewardUI.transform.SetParent(parent.transform.GetChild(0));
                rewardUI.UpdateAmountText(rewardData.rewardAmount.ToString());
            }
            else
            {
                rewardUI.gameObject.SetActive(false);
            }
        }
    }
}
