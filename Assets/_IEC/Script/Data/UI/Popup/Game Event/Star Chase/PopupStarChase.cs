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
        protected override void ShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartStarChase)
                return;
            
            if (Locator.Main.PopupStartStarChaseEvent.gameObject.activeInHierarchy)
                return;
            Locator.Main.PopupStartStarChaseEvent.Show();
        }

        protected override bool CheckShowStartPopup()
        {
            if (!PlayerData.isShownPopupStartStarChase)
                return false;
            return base.CheckShowStartPopup();
        }

        public override void InitEvent()
        {
            base.InitEvent();
            
            if (!PlayerData.isShownPopupStartSharkRace)
            {
                PlayerData.isShownPopupStartSharkRace = true;
            }
        }
    }
}
