using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        private Tween m_Tween;

        [HideInInspector]
        public bool IsRotating;

        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        private int angleY => (int)transform.eulerAngles.y;

        private void OnMouseDown()
        {
            if (!Game.isPlaying) return;
            if (IsRotating) return;
            IsRotating = true;
            m_Tween?.Kill(true);
            var angle = angleY;
            angle += 90;
            if (angle >= 360)
            {
                angle = 0;
            }

            var rot = Quaternion.Euler(0f, angle, 0f);
            transform.DOLocalRotateQuaternion(rot, .3f).OnComplete(() => {
                IsRotating = false;
                EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
            });
        }

        public void OnInit(int rot)
        {
            transform.localRotation = Quaternion.Euler(0f, TotalRots[rot], 0f);
        }
    }
}
