using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace IEC
{
    public class UIMoveOutAnimation
    {
        public static float TWEEN_TIME = 0.3f;
        
        public RectTransform target;
        public UIElementMoveDir dir;
        public Vector3 defaultPos;
        
        public UIMoveOutAnimation(RectTransform trg,UIElementMoveDir dir)
        {
            target = trg;
            this.dir = dir;
            defaultPos = trg.anchoredPosition;
        }

        public bool IsDefaultPos()
        {
            return Vector3.Distance(target.anchoredPosition, defaultPos) < 1f;
        }
        public void MoveOut(string idTwween,Action onComplete = null)
        {
            target.DOAnchorPos(GetDestinationPos(), 0.3f)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                }).SetId(idTwween);
        }
        public void MoveBack(string idTwween,Action onComplete = null)
        {
            target.DOAnchorPos(defaultPos, 0.3f)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    onComplete?.Invoke();
                }).SetId(idTwween);
        }

        public void SetObjPosOut()
        {
            target.anchoredPosition = GetDestinationPos();
        }

        public void SetOutMoveBack(string idTwween,Action onComplete = null)
        {
            target.anchoredPosition = GetDestinationPos();
            MoveBack(idTwween,onComplete);
        }
        public Vector3 GetDestinationPos()
        {
            float range = 1000;
            switch (dir)
            {
                case UIElementMoveDir.UP:
                    return defaultPos + Vector3.up * range;
                case UIElementMoveDir.DOWN:
                    return defaultPos + Vector3.down * range;
                case UIElementMoveDir.LEFT:
                    return defaultPos + Vector3.left * range;
                case UIElementMoveDir.RIGHT:
                    return defaultPos + Vector3.right * range;
            }
            return defaultPos + Vector3.up * 500;
        }
    }
    public enum UIElementMoveDir
    {
        UP,
        RIGHT,
        LEFT,
        DOWN,
        SCALE
    }
}
