using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BaseHomeFeatureButton : _BaseBehaviour
    {
        public Button btn;
        public GameObject lockGroup, unlockGroup;

        [Header("Noti")] 
        public GameObject notiObj;

        [Header("Progress")]
        public ProgressBarFillEffect progressFillEffect;

        public void SetLock(bool isLock)
        {
            btn.interactable = !isLock;
            lockGroup.SetActive(isLock);
            unlockGroup.SetActive(!isLock);
        }

        public void SetNoti(bool showNoti)
        {
            notiObj.SetActive(showNoti);
        }

        public void SetProgressBar(float currentProgress, float maxProgress)
        {
            if (progressFillEffect == null)
                return;
            
            progressFillEffect.SetProgressText($"{currentProgress}/{maxProgress}");
            progressFillEffect.DoFillAnim((float)currentProgress / maxProgress);
        }
    }
}
