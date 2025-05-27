using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using IEC;
using UnityEngine;
using UnityEngine.UI;

public class PopupRelicHunt : BasePopup
{
    public Button infoGroup;
    private Sequence showInfoSequence = null;
    public Button closeInfoButton;

    [Header("Info ui")] public List<Transform> listInfoUI = new();
    

    private void Start()
    {
        SetEvent(infoGroup, OnShowInfo);
    }

    // Start is called before the first frame update
    private void OnShowInfo()
    {
        if (infoGroup != null)
        {
            infoGroup.gameObject.SetActive(!infoGroup.gameObject.activeInHierarchy);
        }

        if (infoGroup != null && infoGroup.gameObject.activeInHierarchy)
        {
            if (showInfoSequence != null) DOTween.Kill(showInfoSequence);
            showInfoSequence = DOTween.Sequence();

            foreach (var ui in listInfoUI)
            {
                ui.localScale = Vector3.zero;
            }

            closeInfoButton.transform.localScale = new Vector3(0, 1, 1);

            float delay = 0f;
            foreach (var ui in listInfoUI)
            {
                showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                delay += 0.1f;
            }

            showInfoSequence.Insert(delay, closeInfoButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));

            showInfoSequence.Play();
        }
    }
}