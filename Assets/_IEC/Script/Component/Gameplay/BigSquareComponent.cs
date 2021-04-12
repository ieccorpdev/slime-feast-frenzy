using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BigSquareComponent : _BaseBehaviour
    {
        private int[,] CurrentLine;

        private BaseComponent[,] grids;

        private bool m_IsRotating;
        private Tween m_Tween;

        private int m_RowStart, m_ColStart;

        private Vector3 CurrentWorldPos => transform.position;

        private int m_GridSize, m_LineSize;

        public void OnInit(BaseComponent[,] gridsInput, int row, int col)
        {
            OnResetAll();
            m_RowStart = row;
            m_ColStart = col;

            grids = gridsInput;

            m_GridSize = gridsInput.GetLength(0);
            m_LineSize = m_GridSize * 3;

            CurrentLine = new int[m_LineSize, m_LineSize];

            for (int i = 0; i < m_LineSize; i++)
            {
                for (int j = 0; j < m_LineSize; j++)
                {
                    CurrentLine[i, j] = gridsInput[i / 3, j / 3].CurrentLine[i % 3, j % 3];
                }
            }

            var posX = (grids[0, 0].CurrentWorldPos.x + grids[0, m_GridSize - 1].CurrentWorldPos.x) / 2f;
            var posZ = (grids[0, 0].CurrentWorldPos.z + grids[m_GridSize - 1, 0].CurrentWorldPos.z) / 2f;

            transform.position = new Vector3(posX, 20f, posZ);

            for (int i = 0; i < m_GridSize; i++)
            {
                for (int j = 0; j < m_GridSize; j++)
                {
                    grids[i, j].transform.SetParent(transform);
                    grids[i, j].SetLock(false);
                }
            }
        }

        private void OnMouseDown()
        {
            OnRotation();
        }

        private void OnRotation()
        {
            if (m_IsRotating) return;

            var rot = new Vector3(0f, 90, 0f);
            var time = Game.isFasterRotate ? .15f : .3f;
            Sequence sequence = DOTween.Sequence();
            var currentPos = transform.localPosition;

            m_Tween = sequence.Join(transform.DOLocalRotate(rot, time, RotateMode.FastBeyond360).SetRelative())
                .Join(transform.DOLocalMoveY(.02f, time / 2).SetLoops(1, LoopType.Yoyo))
                .OnComplete(() =>
                {
                    transform.localPosition = currentPos;
                    m_IsRotating = false;

                    grids = RotateMatrix(grids, m_GridSize);
                    for (int i = m_RowStart; i < m_RowStart + m_GridSize; i++)
                    {
                        for (int j = m_ColStart; j < m_ColStart + m_GridSize; j++)
                        {
                            var row = i - m_RowStart;
                            var col = j - m_ColStart;
                            Game.grids[i, j] = grids[row, col];
                            Game.grids[i, j].gameObject.name = "Row " + i + " | Col " + j;
                            Game.grids[i, j].SetNewPos(i, j);
                        }
                    }

                    CurrentLine = MathHelper.RotateMatrix(CurrentLine, m_LineSize);
                    SetNewLine();

                    EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);
                });
        }

        private void SetNewLine()
        {
            for (int i = 0; i < m_LineSize; i++)
            {
                for (int j = 0; j < m_LineSize; j++)
                {
                    grids[i / 3, j / 3].CurrentLine[i % 3, j % 3] = CurrentLine[i, j];
                }
            }
        }

        private BaseComponent[,] RotateMatrix(BaseComponent[,] grids, int n)
        {
            BaseComponent[,] ret = new BaseComponent[n, n];

            for (int i = 0; i < n; ++i)
            {
                for (int j = 0; j < n; ++j)
                {
                    ret[i, j] = grids[n - j - 1, i];
                }
            }

            return ret;
        }

        private void OnResetAll()
        {
            m_IsRotating = false;
            m_Tween?.Kill(true);
        }

        public void OnDestroyMe()
        {
            PrefabHelper.Destroy(gameObject);
        }

        public void OnTweenMove()
        {
            m_Tween?.Kill(true);

            var moveTime = Config.startEffectMoveTime;
            var delayTime = moveTime + CurrentWorldPos.x * .1f;

            m_Tween = transform.DOLocalMoveY(0f, moveTime).SetDelay(delayTime);
        }


        [NaughtyAttributes.Button]
        protected void CheckLine()
        {
            var str = "";
            for (int i = 0; i < m_LineSize; i++)
            {
                for (int j = 0; j < m_LineSize; j++)
                {
                    str += CurrentLine[i, j];
                }
                str += "\n";
            }

            Debug.LogError(str);
        }
    }
}
