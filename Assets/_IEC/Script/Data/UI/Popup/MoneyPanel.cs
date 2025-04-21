using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class MoneyPanel : BasePopup
    {
        [SerializeField] private TMP_Text m_TxtMoney;
        [SerializeField] private EffectCoinClaim fxCoin;
        [SerializeField] private RectTransform m_TransCoinIcon;
        private bool isShop;

        public TMP_Text txtGold => m_TxtMoney;
        public RectTransform recIconCoin => m_TransCoinIcon;

        protected override void Awake()
        {
            base.Awake();
            isPlayAudioPopup = false;
        }

        public override void Show()
        {
            if (isShop)
            {
                isShop = false;
                mainPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(206.8f, -157f);
            }
            else
            {
                SetPositionDefault();
            }

            m_TxtMoney.text = CurrencyFormat.FormatMoney(PlayerData.hybridData.GetResourceCount(ResourceType.Gold));
            base.Show();
        }

        public void UpdateGold()
        {
            m_TxtMoney.text = CurrencyFormat.FormatMoney(PlayerData.hybridData.GetResourceCount(ResourceType.Gold));
        }

        public MoneyPanel isInShop()
        {
            isShop = true;
            return this;
        }

        public void EffectCoinClaim(Vector2 posSpawn, int moneyAdd, int endMoney, System.Action callback = null)
        {
            Locator.Service.GroupFxCoin.RunFXAddCoin(moneyAdd, posSpawn, callback, m_TxtMoney, m_TransCoinIcon);
            //  fxCoin.SpawnEffect(posSpawn, m_TxtMoney, m_TransCoinIcon.anchoredPosition, moneyAdd, endMoney, callback);
            // soundController.PlaySound(new AudioInfo(Config.soundConfig.coinFly, false));
        }

        public void TestFXCoin()
        {
            fxCoin.SpawnEffect(new Vector2(350, -1050), m_TxtMoney, m_TransCoinIcon.anchoredPosition, 10, PlayerData.hybridData.GetResourceCount(ResourceType.Gold), null);
        }
        public void SetPositionDefault()
        {
            mainPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(219.3f, -153.2f);
        }

        public override void Hide()
        {
            base.Hide();
        }
    }
}