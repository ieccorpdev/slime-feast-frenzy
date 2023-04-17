using System;
using System.Collections;
using System.Collections.Generic;
using EnhancedUI.EnhancedScroller;
using IEC;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Game = IEC.Game;

public class PopupSagaMapScreen : BasePopup
{
    public SagaPath sagaPathPrefab;
    public SagaMapScroll scrollView;
    
    [Header("Button Play")]
    public Button btnPlay;

    public TMP_Text btnPlayTxt;
    public Sprite btnBlue;
    public Sprite btnYellow;
    public Color txtColorYellow;
    public TMP_FontAsset fontNormal;
    public TMP_FontAsset fontYellow;
    
    private EnhancedScroller m_SagaScroller => scrollView.scroll;

    private int m_LevelSelect;
    private bool m_IsSelectBonusLevel;
    private PlayerSecretSagaMap m_PlayerData => Pool.Get<PlayerSecretSagaMap>();
    private Game m_Game => Pool.Get<Game>();
    private bool m_IsInit;
    public EnhancedScrollerCellView GetCellView(EnhancedScroller scroller, int index)
    {
        var path = scroller.GetCellView(sagaPathPrefab).GetComponent<SagaPath>();
        path.gameObject.SetActive(true);
        path.SetData(scrollView.GetNumberOfCells(scroller) - index - 1, (level) =>
        {
            SetBtnPlay(level, false);
        }, levelChild =>
        {
            SetBtnPlay(levelChild, true);
        });
        return path;
    }

    void Init()
    {
        if (m_IsInit) return;
        m_IsInit = true;
        btnPlay.onClick.AddListener(() =>
        {
            Hide();
            //Pool.Get<PlayerSecret>().isInBonusLevel = m_IsSelectBonusLevel;
            Pool.Get<PlayerSecret>().level = m_LevelSelect;
            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
            Controllers.Get<ServicesController>().PushEvent(
                new ClickBtnPlaySaga(m_IsSelectBonusLevel ? m_LevelSelect : m_LevelSelect + 1, m_IsSelectBonusLevel));
        });
    }

    public override void Show()
    {
        Init();
        base.Show();
        scrollView.ReloadData();
        SetBtnPlay(m_PlayerData.unlockLevel, false);
    }

    private void SetBtnPlay(int level, bool isBonusLevel)
    {
        var isLevelPlayed = isBonusLevel
            ? m_PlayerData.listBonusLevelPlayed.Contains(level)
            : level < m_PlayerData.unlockLevel;
        btnPlay.image.sprite = (isLevelPlayed) ? btnBlue : btnYellow;
        btnPlayTxt.text = isBonusLevel ? "Special Level" : $"Level {level + 1}";
        btnPlayTxt.color = (btnPlay.image.sprite == btnYellow) ? txtColorYellow : Color.white;
        btnPlayTxt.font = (btnPlay.image.sprite == btnYellow) ? fontYellow : fontNormal;
        m_LevelSelect = isBonusLevel ? level + 1 : level;
        m_IsSelectBonusLevel = isBonusLevel;
    }
}
