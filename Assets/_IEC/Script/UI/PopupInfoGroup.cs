using DG.Tweening;
using IEC;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupInfoGroup : ScreenUI
{
    [SerializeField] private GameObject mainPopup;
    
    public override void Show()
    {
        if (mainPopup == null)
            return;
        mainPopup.transform.DOKill();
        mainPopup.transform.localScale = Vector3.zero;
        mainPopup.transform.DOScale(Vector3.one, 0.4f).SetEase(Ease.OutBack);
        gameObject.SetActive(true);
        StartCoroutine(WaitToHide());
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
    
}
