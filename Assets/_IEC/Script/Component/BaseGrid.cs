using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        protected Tween m_Tween;

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rot, color, style);
            for (int i = 0; i < rot; i++)
            {
                CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);
            }
        }

        private void OnMouseDown()
        {
            if (!Game.isPlaying) return;
            if (IsRotating) return;
            IsRotating = true;
            m_Tween?.Kill(true);

            m_RotY = (m_RotY + 1) % 4;
            var rot = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);

            var time = .3f;

            Sequence sequence = DOTween.Sequence();

            var currentPos = transform.localPosition;

            //var tempPos = currentPos;
            //tempPos.y += .02f;
            //transform.localPosition = tempPos;

            m_Tween = sequence.Join(transform.DOLocalRotateQuaternion(rot, time))
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    IsRotating = false;
                    CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);
                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
                });

            //transform.DOLocalRotateQuaternion(rot, .3f).OnComplete(() => {
            //    transform.localPosition = currentPos;

            //    IsRotating = false;
            //    CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);
            //    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
            //});
        }
    }
}
