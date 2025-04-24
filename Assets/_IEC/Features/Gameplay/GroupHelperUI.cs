using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class GroupHelperUI : _BaseBehaviour
    {
        [SerializeField] private HelperButton frozenHelper;
        [SerializeField] private HelperButton magnetHelper;
        [SerializeField] private HelperButton scaleUpHelper;
        [SerializeField] private HelperButton compassHelper;
        
        [SerializeField] private PopupHelper popupHelperMagnet;
        [SerializeField] private PopupHelper popupHelperCompass;
        [SerializeField] private PopupHelper popupHelperFrozenTime;
        [SerializeField] private PopupHelper popupHelperScaleUp;

        // public PopupHelper GetPopupHelper(HelperId helperId)
        // {
        //     return helperId switch
        //     {
        //         HelperId.Magnet => popupHelperMagnet,
        //         HelperId.FrozenTime => popupHelperFrozenTime,
        //         HelperId.ScaleUp => popupHelperScaleUp,
        //         HelperId.Compass => popupHelperCompass,
        //         _ => null
        //     };
        //}

        public void Show()
        {
            // frozenHelper.SetPopup(popupHelperFrozenTime);
            // magnetHelper.SetPopup(popupHelperMagnet);
            // scaleUpHelper.SetPopup(popupHelperScaleUp);
            // compassHelper.SetPopup(popupHelperCompass);
            // frozenHelper.OnShow();
            // magnetHelper.OnShow();
            // scaleUpHelper.OnShow();
            // compassHelper.OnShow();
        }
    }
}