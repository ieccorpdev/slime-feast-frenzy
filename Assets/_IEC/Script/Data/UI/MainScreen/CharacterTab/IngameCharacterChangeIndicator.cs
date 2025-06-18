using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class IngameCharacterChangeIndicator : _BaseBehaviour
    {
        public Image indicatorBg;
        public Image indicatorFillImg;
        public GameObject fireFxGroup;

        public CharacterConfig CurrentCharacterConfig { get; private set; }

        private int _currentComboCount = 0;
        private bool _isCharacterChanged;
        
        private void OnEnable()
        {
            this.RegisterListener(EventList.GamePlay.COMBO_REACHED,OnIncreaseCombo);
            this.RegisterListener(EventList.GamePlay.COMBO_LOST,OnComboLost);
            this.RegisterListener(EventList.GamePlay.CHANGE_CHARACTER_ON_COMBO_REACHED,OnChangeCharacter);
        }

        private void OnDisable()
        {
            this.UnregisterListener(EventList.GamePlay.COMBO_REACHED,OnIncreaseCombo);
            this.UnregisterListener(EventList.GamePlay.COMBO_LOST,OnComboLost);
            this.UnregisterListener(EventList.GamePlay.CHANGE_CHARACTER_ON_COMBO_REACHED,OnChangeCharacter);
        }

        private void OnIncreaseCombo(object param)
        {
            if (CurrentCharacterConfig == null)
                return;
            
            if (_isCharacterChanged)
                return;
            
            _currentComboCount++;
            SetFillImage();
        }

        private void OnComboLost(object param)
        {
            if (CurrentCharacterConfig == null)
                return;
            
            if (_isCharacterChanged)
                return;
            
            _currentComboCount = 0;
            SetFillImage();
        }

        private void OnChangeCharacter(object param)
        {
            if (CurrentCharacterConfig == null)
                return;
            
            _isCharacterChanged = true;
            fireFxGroup.SetActive(true);
            //Set fx
        }

        private void SetFillImage()
        {
            indicatorFillImg.DOFillAmount((float)_currentComboCount / CurrentCharacterConfig.comboCountToChange, 0.2f).SetEase(Ease.OutQuad);
        }

        public void Setup(CharacterConfig characterConfig)
        {
            if (characterConfig.comboCountToChange <= 0)
            {
                CurrentCharacterConfig = null;
                return;
            }
            
            fireFxGroup.SetActive(false);

            CurrentCharacterConfig = characterConfig;
            
            indicatorFillImg.fillAmount = 0f;
            _currentComboCount = 0;
            _isCharacterChanged = false;

            indicatorBg.sprite = CurrentCharacterConfig.characterChangeIndicatorBg;
            indicatorFillImg.sprite = CurrentCharacterConfig.characterChangeIndicatorSprite;
        }
    }
}