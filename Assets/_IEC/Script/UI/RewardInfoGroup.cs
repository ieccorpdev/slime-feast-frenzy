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
    private bool needSetup = true;
    private bool isFirstSetup;
    
    public override void Show()
    {
        if (mainPopup == null)
            return;
        mainPopup.transform.DOKill();
        mainPopup.transform.localScale = Vector3.zero;
        mainPopup.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        gameObject.SetActive(true);
        StartCoroutine(WaitToHide());
        foreach (var rewardUI in listRewardInfoUI)
        {
            rewardUI.gameObject.transform.localScale = Vector3.one;
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            Hide();
        }
    }

    IEnumerator WaitToHide()
    {
        yield return new WaitForSeconds(3f);
        Hide();
    }

    private void OnDisable()
    {
        this.gameObject.SetActive(false);
    }

    public override void Hide()
    {
        if (mainPopup != null)
        {
            mainPopup.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                this.gameObject.SetActive(false);
            });
        }
        else
        {
            this.gameObject.SetActive(false);
        }
    }

    private void FirstSetup()
    {
        if (!isFirstSetup)
        {
            needSetup = mainPopup == null;
            isFirstSetup = true;
        }
    }


    public void SetupDataItem(List<Reward> dataItem)
    {
        FirstSetup();
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
        if (needSetup)
            mainPopup = parent;
        parent.SetActive(true);

        foreach (var rewardUI in listRewardInfoUI)
        {
            Reward rewardData;
            if ((rewardData = dataItem.Find(data => data.typeReward == rewardUI.resourceType)) != null)
            {
                rewardUI.gameObject.SetActive(true);
                rewardUI.gameObject.transform.localScale = Vector3.one;
                if (rewardUI.transform.parent != parent.transform.GetChild(0))
                {
                    rewardUI.transform.SetParent(parent.transform.GetChild(0));
                    rewardUI.transform.SetAsFirstSibling();
                }
                rewardUI.UpdateAmountText(rewardData.rewardAmount.ToString());
            }
            else
            {
                rewardUI.gameObject.SetActive(false);
            }
        }
    }
}
