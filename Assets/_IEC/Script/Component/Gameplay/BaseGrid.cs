using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int style = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, style);
            SetGridLine(TrueRot);
        }

        public override void FixLevelData(int newCurrentRot)
        {
            base.FixLevelData(newCurrentRot);
            m_CurrentRot = newCurrentRot;
            //Debug.LogError("newCurrentRot: " + newCurrentRot);
            //SetGridLine(m_CurrentRot);
        }

        public override void AfterInit(bool isMultiColor)
        {
            base.AfterInit(isMultiColor);
            SetGridLine(m_CurrentRot);
            transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);
            //Debug.LogError("m_CurrentRot: " + m_CurrentRot);
        }



        private void SetGridLine(int rot)
        {
            CurrentLine = DefaultLine;
            for (int i = 0; i < rot; i++)
            {
                CurrentLine = MathHelper.RotateMatrix(CurrentLine, 3);
            }
        }

        protected override void OnMouseDown()
        {
            if (Game.isTutorial && ID != Game.gridTutId) return;
            if (Game.isShowPopup) return;
            if (!Game.isPlaying) return;
            if (m_IsRotating) return;

            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.GRID_CLICK));
            EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK);
            m_IsRotating = true;
            m_Tween?.Kill(true);

            m_CurrentRot = (m_CurrentRot + 1) % 4;
            var rot = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);

            var time = Game.isFasterRotate ? .15f : .3f;

            Sequence sequence = DOTween.Sequence();

            var currentPos = transform.localPosition;

            m_Tween = sequence.Join(transform.DOLocalRotateQuaternion(rot, time))
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    m_IsRotating = false;
                    SetGridLine(m_CurrentRot);

                    EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);

                    if (Game.isTutorial)
                    {
                        EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK_TUTORIAL);
                    }
                });
        }

        public override void OnHint()
        {
            base.OnHint();
            m_IsRotating = true;
            m_Tween?.Kill(true);

            m_CurrentRot = TrueRot;
            var rot = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);

            var time = Game.isFasterRotate ? .15f : .3f;

            Sequence sequence = DOTween.Sequence();

            var currentPos = transform.localPosition;

            m_Tween = sequence.Join(transform.DOLocalRotateQuaternion(rot, time))
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    m_IsRotating = false;
                    SetGridLine(m_CurrentRot);
                });
        }
    }
}
