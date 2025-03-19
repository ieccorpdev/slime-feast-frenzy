using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class GroupFxCoin : BasePopup
    {
        [SerializeField] private EffectCoinClaim fxCoin;
        private TMP_Text m_TxtMoney;
        private RectTransform m_TransCoinIcon;

        public override void Show()
        {
            base.Show();
        }

        public void RunFXAddCoin(int coinAdd, Vector2 posSpawn, System.Action callback = null,
            TMP_Text txtCoinCount = null, RectTransform positionEnd = null)
        {
            // Show();
            if (txtCoinCount != null)
            {
                m_TxtMoney = txtCoinCount;
                m_TransCoinIcon = positionEnd;
            }
            else
            {
                m_TxtMoney = Locator.Service.MoneyPanel.txtGold;
                m_TransCoinIcon = Locator.Service.MoneyPanel.recIconCoin;
            }

            EffectCoinClaim(posSpawn, coinAdd, PlayerData.hybridData.GetResourceCount(ResourceType.Gold),
                delegate { callback?.Invoke(); });
        }

        public void EffectCoinClaim(Vector2 posSpawn, int moneyAdd, int endMoney, System.Action callback = null)
        {
            fxCoin.SpawnEffect(posSpawn, m_TxtMoney, m_TransCoinIcon.anchoredPosition, moneyAdd, endMoney, callback);
        }
    }
}