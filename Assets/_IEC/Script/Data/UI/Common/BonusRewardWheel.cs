using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace IEC
{
    public class BonusRewardWheel : _BaseBehaviour
    {
        [SerializeField] Animator animWheel;
        [SerializeField] List<TextMeshProUGUI> lstTxtBonusReward;
        [SerializeField] List<float> lstBonusValue;
        public float valueBonus;
        public int endLevelGoldGet;
        
        // Start is called before the first frame update
        void Start()
        {

        }

        public void InitWheel()
        {
            animWheel.enabled = true;
            valueBonus = 0;
        }

        public void StopWheel()
        {
            animWheel.enabled = false;
        }

        private void Update()
        {
            if (!animWheel.enabled) return;
            for (int i = 0; i < lstTxtBonusReward.Count; i++)
            {
                if (lstTxtBonusReward[i].rectTransform.localScale != Vector3.one)
                {
                    valueBonus = lstBonusValue[i];
                    break;
                } 
            }
        }

        public int GetFinalRewardCount(int initRewardCount)
        {
            return Mathf.RoundToInt(initRewardCount * valueBonus);
        }
    }
}
