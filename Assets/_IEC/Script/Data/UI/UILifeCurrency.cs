using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class UILifeCurrency : UICurrency
    {
        public TextMeshProUGUI lifeCountDownText;
        public Image infinityLifeImage;
        public Button button;

        private void Update()
        {
            UpdateCurrency();
        }

        public override void UpdateCurrency()
        {
            if (PlayerData.IsInUnlimitedLife())
            {
                currencyTMP.gameObject.SetActive(false);
                infinityLifeImage.gameObject.SetActive(true);
                lifeCountDownText.text = PlayerData.GetTimeLeftUnlimitedLife();
            }
            else
            {
                currencyTMP.gameObject.SetActive(true);
                infinityLifeImage.gameObject.SetActive(false);
                currencyTMP.text = PlayerData.playerLife.ToString();
                lifeCountDownText.text = PlayerData.GetTimeToNextLife();
            }
            button.interactable = PlayerData.playerLife < Config.maxHealth && !PlayerData.IsInUnlimitedLife();
            addButtonDisplay.SetActive(PlayerData.playerLife < Config.maxHealth && !PlayerData.IsInUnlimitedLife());
        }
    }
}
