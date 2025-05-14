using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using DG.Tweening;
using EnhancedUI.EnhancedScroller;
using IEC.Helper;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public class PopupStarChase : PopupGameEvent
    {
        public List<Transform> listInfoUI = new();

        private Sequence showInfoSequence;

        protected override void OnShowInfo()
        {
            base.OnShowInfo();

            if (infoGroup != null && infoGroup.activeInHierarchy)
            {
                DOTween.Kill(showInfoSequence);
                showInfoSequence = DOTween.Sequence();

                foreach (var ui in listInfoUI)
                {
                    ui.localScale = Vector3.zero;
                }

                closeInfoButton.transform.localScale = new Vector3(0, 1, 1);

                float delay = 0f;
                foreach (var ui in listInfoUI)
                {
                    showInfoSequence.Insert(delay, ui.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack));
                    delay += 0.1f;
                }
                
                showInfoSequence.Insert(delay, closeInfoButton.transform.DOScaleX(1, 0.2f).SetEase(Ease.OutBack));

                showInfoSequence.Play();
            }
        }
    }
}
