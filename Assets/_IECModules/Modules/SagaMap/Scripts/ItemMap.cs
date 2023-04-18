using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class ItemMap : _BaseBehaviour
    {
        [Header("Background")]
        [SerializeField] private Image m_Bg;

        [SerializeField] private Image m_ChildBg;

        [SerializeField] private Sprite m_GrayBg;
        [SerializeField] private Sprite m_BlueBg;
        [SerializeField] private Sprite m_RedBg;
        [SerializeField] private Sprite m_YellowBg;

        [Header("Text")][SerializeField] private TMP_Text m_Txt;
        [SerializeField] private TMP_Text m_ChildTxt;
        [SerializeField] private Color m_TxtColorYellow;


        [Header("Button")][SerializeField] private Button m_Btn;
        [SerializeField] private Button m_ChildBtn;

        [SerializeField] private GameObject m_Child;
        [SerializeField] private GameObject m_Boss;

        PlayerSecretSagaMap PlayerDataSagaMap => Pool.Get<PlayerSecretSagaMap>();

        public void SetData(int level, Action<int> click = null, Action<int> childClick = null)
        {
            m_Btn.onClick.RemoveAllListeners();
            m_ChildBtn.onClick.RemoveAllListeners();
            m_Txt.text = $"{level + 1}";
            m_Child.SetActive(PlayerDataSagaMap.IsBonusLevel(level + 2));
            var isLevelLocked = level > PlayerDataSagaMap.unlockLevel;
            var isLevelBonusLocked = level > PlayerDataSagaMap.unlockLevel - 1;
            m_Bg.sprite = isLevelLocked ? m_GrayBg : level == PlayerDataSagaMap.unlockLevel ? m_YellowBg : m_BlueBg;
            m_ChildBg.sprite = isLevelBonusLocked
                ? m_GrayBg
                : PlayerDataSagaMap.listBonusLevelPlayed.Contains(level) ? m_BlueBg : m_RedBg;
            m_ChildTxt.text = "?";

            m_Txt.color = (m_Bg.sprite == m_YellowBg) ? m_TxtColorYellow : Color.white;

            m_Boss.SetActive(PlayerDataSagaMap.IsBossLevel(level + 1));

            m_Btn.onClick.AddListener(() =>
            {
                if (isLevelLocked) return;
                click?.Invoke(level);
            });

            m_ChildBtn.onClick.AddListener(() =>
            {
                if (isLevelBonusLocked) return;
                childClick?.Invoke(level);
            });
        }
    }

}