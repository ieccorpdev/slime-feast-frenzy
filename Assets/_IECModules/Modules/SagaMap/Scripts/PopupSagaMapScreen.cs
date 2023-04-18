using EnhancedUI.EnhancedScroller;
using IEC.TrackingEvent;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class PopupSagaMapScreen : BasePopup
    {
        public SagaPath sagaPathPrefab;
        public SagaMapScroll scrollView;

        public Button btnPlay;

        public TMP_Text btnPlayTxt;
        public Sprite btnBlue;
        public Sprite btnYellow;
        public Color txtColorYellow;
        public TMP_FontAsset fontNormal;
        public TMP_FontAsset fontYellow;

        public EnhancedScroller Scroller => scrollView.scroll;

        private int m_LevelSelect;
        private bool m_IsSelectBonusLevel;
        private PlayerSecretSagaMap PlayerDataSagaMap => Pool.Get<PlayerSecretSagaMap>();
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
                //PlayerData.isInBonusLevel = m_IsSelectBonusLevel;
                PlayerData.level = m_LevelSelect;
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                Services.PushEvent(
                    new ClickBtnPlaySaga(m_IsSelectBonusLevel ? m_LevelSelect : m_LevelSelect + 1, m_IsSelectBonusLevel));
            });
        }

        public override void Show()
        {
            Init();
            base.Show();
            scrollView.popup = this;
            scrollView.ReloadData();
            SetBtnPlay(PlayerDataSagaMap.unlockLevel, false);
        }

        private void SetBtnPlay(int level, bool isBonusLevel)
        {
            var isLevelPlayed = isBonusLevel
                ? PlayerDataSagaMap.listBonusLevelPlayed.Contains(level)
                : level < PlayerDataSagaMap.unlockLevel;
            btnPlay.image.sprite = (isLevelPlayed) ? btnBlue : btnYellow;
            btnPlayTxt.text = isBonusLevel ? "Special Level" : $"Level {level + 1}";
            btnPlayTxt.color = (btnPlay.image.sprite == btnYellow) ? txtColorYellow : Color.white;
            btnPlayTxt.font = (btnPlay.image.sprite == btnYellow) ? fontYellow : fontNormal;
            m_LevelSelect = isBonusLevel ? level + 1 : level;
            m_IsSelectBonusLevel = isBonusLevel;
        }
    }

}
