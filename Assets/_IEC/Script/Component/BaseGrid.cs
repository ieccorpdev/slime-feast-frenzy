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

        protected override void OnMouseDown()
        {
            if (Game.isTutorial && ID != Game.gridTutId) return;
            if (Game.isShowPopup) return;
            if (!Game.isPlaying) return;
            if (IsRotating) return;

            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.GRID_CLICK));
            IsRotating = true;
            m_Tween?.Kill(true);

            m_RotY = (m_RotY + 1) % 4;
            var rot = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);

            var time = Game.isFasterRotate ? .15f : .3f;

            Sequence sequence = DOTween.Sequence();

            var currentPos = transform.localPosition;

            m_Tween = sequence.Join(transform.DOLocalRotateQuaternion(rot, time))
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    IsRotating = false;
                    CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);
                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);

                    if (Game.isTutorial)
                    {
                        EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK_TUTORIAL);
                    }
                });
        }
    }
}
