using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        public override void OnInit(GridType gridType, int row, int col, int rotTrue = 0, int rot = 0, int color = 0, int lineType = 0)
        {
            base.OnInit(gridType, row, col, rotTrue, rot, color, lineType);
            SetGridLine(TrueRot);
            Transform.localRotation = Quaternion.Euler(0f, TotalRots[TrueRot], 0f);
        }

        public override void BeforeInit()
        {
            base.BeforeInit();
            if (m_IsLock) return;
            SetGridLine(m_CurrentRot);
            Transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);
        }

        public override void SetLock()
        {
            base.SetLock();
            m_CurrentRot = TrueRot;
            Transform.localRotation = Quaternion.Euler(0f, TotalRots[m_CurrentRot], 0f);
        }

        private void SetGridLine(int rot)
        {
            Inline = (StartPoint)((m_CurrentRot - m_InlineOffset + 4) % 4);
            CurrentLine = DefaultLine;
            for (int i = 0; i < rot; i++)
            {
                CurrentLine = MathHelper.RotateMatrix(CurrentLine, 3);
            }
        }

        protected override void OnHandleMouseDown()
        {
            base.OnHandleMouseDown();
            OnRotation(false);
        }

        private void OnRotation(bool isUndo)
        {
            if (Game.isTutorial && ID != Game.gridTutId) return;
            if (Game.isShowPopup) return;
            if (!Game.isPlaying) return;
            if (m_IsRotating) return;
            if (m_IsLock) return;

            if (!isUndo && Game.isHaveBossLimitMoveLevels)
            {
                EventController.RaiseEvent(EventList.GamePlay.GAME_ADD_MOVE, new EventParam(this));
            }

            EventController.RaiseEvent(EventList.GamePlay.PLAY_SOUND, new EventParam(SoundName.GRID_CLICK));
            EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK);

            m_IsRotating = true;
            m_Tween?.Kill(true);


            var rot = new Vector3(0f, 90, 0f);
            var time = Game.isFasterRotate ? .15f : .3f;
            Sequence sequence = DOTween.Sequence();

            var currentPos = transform.localPosition;

            m_CurrentRot = isUndo ? LevelHelper.CalculatorUndoRot(m_CurrentRot, 1) : (m_CurrentRot + 1) % 4;

            m_Tween = sequence.Join(transform.DOLocalRotate(rot, time, RotateMode.FastBeyond360).SetRelative())
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    m_IsRotating = false;
                    SetGridLine(m_CurrentRot);

                    EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);

                    if (Game.isTutorial)
                    {
                        EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK_TUTORIAL);
                    }
                });
        }

        public override void OnHint()
        {
            base.OnHint();
            if (Game.isHintFixedBlockStyle)
            {
                SetLock();
            }
            m_IsRotating = true;
            m_Tween?.Kill(true);

            var rotRemains = LevelHelper.CalculatorMissingRot(m_CurrentRot, GetTrueRotHint());
            var newRot = rotRemains * 90;
            var rot = new Vector3(0f, newRot, 0f);
            var time = Game.isFasterRotate ? .15f : .3f;

            Sequence sequence = DOTween.Sequence();
            var currentPos = transform.localPosition;

            m_CurrentRot = GetTrueRotHint();

            m_Tween = sequence.Join(transform.DOLocalRotate(rot, time, RotateMode.FastBeyond360).SetRelative())
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    m_IsRotating = false;
                    SetGridLine(m_CurrentRot);
                });
        }

        public override void OnUndo()
        {
            base.OnUndo();
            OnRotation(true);
        }
    }
}
