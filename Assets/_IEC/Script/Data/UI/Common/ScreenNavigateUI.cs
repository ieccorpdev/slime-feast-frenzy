using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace IEC
{
    public class ScreenNavigateUI : ScreenUI
    {
        public TabType tabType;
        public GameObject mainPanel;

        protected string TWEEN_ID => tabType.ToString() + GetInstanceID().ToString();
        protected float TWEEN_TIME => 0.3f;

        public override void Hide()
        {
            gameObject.SetActive(false);
        }

        public void MoveLeftHide(Action onCompleteCallback = null)
        {
            //bottomBar.SetActive(false);
            //mainPanel.transform.position = initMainPanelPos.position;

            DOTween.Kill(TWEEN_ID);

            mainPanel.GetComponent<RectTransform>().DOAnchorPos(GetScreenLeftPoint(), TWEEN_TIME)
                .SetId(TWEEN_ID)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    //onShowDone.Invoke();
                    Hide();
                    onCompleteCallback?.Invoke();
                    DOTween.Kill(TWEEN_ID);
                });
        }

        public void MoveMiddleShow(Action onCompleteCallback = null)
        {
            //bottomBar.SetActive(true);
            //Show();
            DOTween.Kill(TWEEN_ID);

            mainPanel.GetComponent<RectTransform>().DOAnchorPos(GetScreenMiddlePoint(), TWEEN_TIME)
                .SetId(TWEEN_ID)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    Show();
                    onCompleteCallback?.Invoke();
                    DOTween.Kill(TWEEN_ID);
                });
        }

        public void MoveRightHide(Action onCompleteCallback = null)
        {
            //bottomBar.SetActive(false);

            DOTween.Kill(TWEEN_ID);

            mainPanel.GetComponent<RectTransform>().DOAnchorPos(GetScreenRightPoint(), TWEEN_TIME)
                .SetId(TWEEN_ID)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
                {
                    Hide();
                    onCompleteCallback?.Invoke();
                    DOTween.Kill(TWEEN_ID);
                });
        }
        
        public void SetPosLeft()
        {
            if (!DOTween.IsTweening(TWEEN_ID)) ;
            mainPanel.GetComponent<RectTransform>().anchoredPosition = GetScreenLeftPoint();
        }

        public void SetPosRight()
        {
            if (!DOTween.IsTweening(TWEEN_ID)) ;
            mainPanel.GetComponent<RectTransform>().anchoredPosition = GetScreenRightPoint();
        }
        
        protected Vector3 GetScreenMiddlePoint()
        {
            var worldPos = new Vector3(0, 0,0);
            return worldPos;
        }

        protected Vector3 GetScreenLeftPoint()
        {
            var worldPos = new Vector3(GetComponent<RectTransform>().rect.width * -1, 0,0);
            return worldPos;
        }
        
        protected Vector3 GetScreenRightPoint()
        {
            var worldPos = new Vector3(GetComponent<RectTransform>().rect.width * 1, 0,0);
            return worldPos;
        }

        protected Vector3 GetScreenTopCenterPoint(float yOffset = 0f)
        {
            var worldPos = new Vector3(0, GetComponent<RectTransform>().rect.height * 0.5f + yOffset, 0);
            return worldPos;
        }
    }
}
