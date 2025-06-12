using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class CharacterTabStatDisplay : _BaseBehaviour
    {
        public CharacterStatType statType;
        public Image fillImage;

        public void Setup(CharacterStat stat)
        {
            if (stat == null)
                return;
            
            DOTween.Kill(fillImage);
            fillImage.DOFillAmount(stat.statValue, 0.2f).SetEase(Ease.OutQuad);
        }
    }
}
