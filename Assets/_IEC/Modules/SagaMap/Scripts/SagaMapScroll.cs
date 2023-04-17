using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SagaMapScroll : MonoBehaviour, IEnhancedScrollerDelegate
{
    public PopupSagaMapScreen popup;
    public EnhancedScroller scroll;
    public RectTransform cloud;
    public GameObject lastTxt;
    private PlayerSecretSagaMap m_PlayerData => Pool.Get<PlayerSecretSagaMap>();

    [SerializeField] private Vector2 m_ContentCapPos;
    public void ReloadData()
    {
        StartCoroutine(IE_WaitResetPos());
    }

    IEnumerator IE_WaitResetPos()
    {
        // yield return new WaitForEndOfFrame();
        if (scroll.Delegate == null)
        {
            scroll.Delegate = this;
            Debug.LogWarning("reset layout");
            var layout = scroll.Container.GetComponent<HorizontalOrVerticalLayoutGroup>();
            layout.childAlignment = TextAnchor.LowerCenter;
            scroll.Container.pivot = new Vector2(0.5f, 0f);
            scroll.Container.anchorMin = Vector2.zero;
            scroll.Container.anchorMax = Vector2.right;
            // yield return new WaitForEndOfFrame();
        }
        else
        {
            scroll.ReloadData();
        }
        yield return null;
        cloud.transform.SetAsLastSibling();
        scroll.JumpToDataIndex(GetNumberOfCells(scroll) - 1 - m_PlayerData.unlockLevel / 5);
        if ((m_PlayerData.unlockLevel + 1) % 5 == 0)
        {
            var anchorPos = scroll.Container.anchoredPosition;
            anchorPos -= 400f * Vector2.up;
            scroll.Container.anchoredPosition = anchorPos;
        }
    }

    private void Update()
    {
        var pos = scroll.Container.anchoredPosition;
        pos.y = Mathf.Clamp(pos.y, -m_ContentCapPos.x - scroll.Container.sizeDelta.y + GetCellViewSize(scroll, 0) / 2, m_ContentCapPos.y);
        scroll.Container.anchoredPosition = pos;
        if (scroll.Container.anchoredPosition.y > - GetCellViewSize(scroll, 0) * (GetNumberOfCells(scroll) - 1) + 200f)
        {
            scroll.ScrollRect.movementType = ScrollRect.MovementType.Clamped;
            var sizeCloud = cloud.sizeDelta;
            sizeCloud.y = 380f;
            cloud.sizeDelta = sizeCloud;
            lastTxt.SetActive(false);
        }
        else
        {
            scroll.ScrollRect.movementType = ScrollRect.MovementType.Elastic;
            var sizeCloud = cloud.sizeDelta;
            sizeCloud.y = 600f;
            cloud.sizeDelta = sizeCloud;
            lastTxt.SetActive(true);
        }
    }

    public int GetNumberOfCells(EnhancedScroller scroller)
    {
        return (m_PlayerData.unlockLevel + 50) / 5;
    }

    public float GetCellViewSize(EnhancedScroller scroller, int dataIndex)
    {
        return 1920f;
    }

    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int dataIndex, int cellIndex)
    {
        return popup.GetCellView(scroll, dataIndex);
    }
}
