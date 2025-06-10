using System;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;
using UnityEngine.UIElements;

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
        [Header("Effect Freeze")]
        [SerializeField] private ParticleSystem muzzleFrozen;
        [SerializeField] private ParticleSystem missleFrozen;
        [SerializeField] private ParticleSystem explosionFrozen;

        public void Start()
        {
            frozenHelper.actionEffect += AnimationFrozen;
            missleFrozen.Stop();
            //gameplayTimer.OnUpdateFrozenState += isFrozen => { groupMark.SetActiveFrozenMark(isFrozen); };
        }

        public void OnDestroy()
        {
            frozenHelper.actionEffect -= AnimationFrozen;
            DOTween.Kill(missleFrozen.gameObject.GetInstanceID() + "_Frozen");
        }

        public PopupHelper GetPopupHelper(HelperId helperId)
        {
            return helperId switch
            {
                HelperId.Magnet => popupHelperMagnet,
                HelperId.FrozenTime => popupHelperFrozenTime,
                HelperId.ScaleUp => popupHelperScaleUp,
                HelperId.Compass => popupHelperCompass,
                _ => null
            };
        }

        public void Show()
        {
            frozenHelper.SetPopup(popupHelperFrozenTime);
            magnetHelper.SetPopup(popupHelperMagnet);
            scaleUpHelper.SetPopup(popupHelperScaleUp);
            compassHelper.SetPopup(popupHelperCompass);
            frozenHelper.OnShow();
            magnetHelper.OnShow();
            scaleUpHelper.OnShow();
            compassHelper.OnShow();
        }

        public void AnimationFrozen()
        {
            muzzleFrozen.Play();
            missleFrozen.transform.position = frozenHelper.transform.position;
            missleFrozen.Play();
            missleFrozen.transform
                .DOMove(Locator.Game.ScreenInGame.GetGameplayTimer().timeTextTMP.transform.position, 0.5f)
                .SetId(missleFrozen.gameObject.GetInstanceID() + "_Frozen").OnComplete(() =>
                {
                    explosionFrozen.Play();
                    missleFrozen.Stop();
                    frozenHelper.completeActionEffect?.Invoke();
                    Locator.Game.ScreenInGame.groupMark.SetActiveFrozenMark(true);
                    Locator.Game.ScreenInGame.gameplayTimer.StartFreezeTime();
                });
        }
    }
}