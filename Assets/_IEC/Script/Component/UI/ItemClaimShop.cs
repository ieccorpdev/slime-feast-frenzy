using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using TMPro;

public class ItemClaimShop : MonoBehaviour
{
    [SerializeField]
    private Button m_BtnClaim;

    [SerializeField]
    private Image m_ImgIcon;

    [SerializeField]
    private TextMeshProUGUI m_TxtName;

    [SerializeField]
    private RectTransform m_RectItem;

    private Tween m_TweenBuyItem;

    [SerializeField]
    private GameObject m_EffectObj;

    [SerializeField]
    private GameObject m_LockObj, m_ItemObj;

    private float m_TimeEffect = .3f;

    public void OnInit()
    {
        m_BtnClaim.onClick.RemoveAllListeners();
        m_BtnClaim.onClick.AddListener(() => {
            gameObject.SetActive(false);
        });
    }

    public void ShowEffect(Sprite sprIcon, string name)
    {
        m_ImgIcon.sprite = sprIcon;
        m_TxtName.text = name;
        OnResetAll();

        Sequence sequence = DOTween.Sequence();

        m_TweenBuyItem = sequence.Insert(0f, m_RectItem.DOAnchorPosY(250f, m_TimeEffect).SetEase(Ease.Linear))
            .Insert(m_TimeEffect + .1f, m_RectItem.DOScaleX(0f, m_TimeEffect))
            .Insert((m_TimeEffect * 2f) + .1f, m_RectItem.DOScaleX(1f, m_TimeEffect))
            .InsertCallback(m_TimeEffect * 2f, () => {
                m_LockObj.SetActive(false);
                m_ItemObj.SetActive(true);
            });
        sequence.OnComplete(() => {
                m_BtnClaim.gameObject.SetActive(true);
                m_EffectObj.SetActive(true);
            });

        sequence.Play();
    }

    public void OnResetAll()
    {
        m_TweenBuyItem?.Kill(true);
        m_RectItem.anchoredPosition = new Vector2(0f, -2000f);
        m_BtnClaim.gameObject.SetActive(false);
        m_EffectObj.SetActive(false);

        m_LockObj.SetActive(true);
        m_ItemObj.SetActive(false);
    }
}
