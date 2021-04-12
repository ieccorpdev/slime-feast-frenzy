using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BigSquareComponent : _BaseBehaviour
    {
        private int[,] CurrentLine = new int[6, 6];

        private BaseComponent[,] grids;

        private bool m_IsRotating;
        private Tween m_Tween;

        private int m_RowStart, m_ColStart;

        public void OnInit(BaseComponent[,] gridsInput, int row, int col)
        {
            OnResetAll();
            m_RowStart = row;
            m_ColStart = col;

            grids = gridsInput;

            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    CurrentLine[i, j] = gridsInput[i / 3, j / 3].CurrentLine[i % 3, j % 3];
                }
            }

            var posX = (grids[0, 0].CurrentWorldPos.x + grids[0, 1].CurrentWorldPos.x) / 2f;
            var posZ = (grids[0, 0].CurrentWorldPos.z + grids[1, 0].CurrentWorldPos.z) / 2f;

            transform.position = new Vector3(posX, -.11f, posZ);

            for (int i = 0; i < 2; i++)
            {
                for (int j = 0; j < 2; j++)
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
                    CurrentLine = MathHelper.RotateMatrix(CurrentLine, 6);
                    grids = RotateMatrix(grids, 2);
                    SetNewLine();

                    for (int i = m_RowStart; i < m_RowStart + 2; i++)
                    {
                        for (int j = m_ColStart; j < m_ColStart + 2; j++)
                        {
                            var newRow = i - m_RowStart;
                            var newCol = j - m_ColStart;
                            Game.grids[i, j] = grids[newRow, newCol];
                            Game.grids[i, j].gameObject.name = "Row " + i + " | Col " + j;
                        }
                    }

                    //EventController.RaiseEvent(EventList.GamePlay.GAME_CHECK_CONNECT);

                    //if (Game.isTutorial)
                    //{
                    //    EventController.RaiseEvent(EventList.GamePlay.GAME_GRID_CLICK_TUTORIAL);
                    //}
                });
        }

        private void SetNewLine()
        {
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
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


        [NaughtyAttributes.Button]
        protected void CheckLine()
        {
            var str = "";
            for (int i = 0; i < 6; i++)
            {
                for (int j = 0; j < 6; j++)
                {
                    str += CurrentLine[i, j];
                }
                str += "\n";
            }

            Debug.LogError(str);
        }
    }
}
