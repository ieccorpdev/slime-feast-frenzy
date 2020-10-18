using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        protected Tween m_Tween;
        protected static int[] TotalRots = new int[] { 0, 90, 180, 270 };

        public override void OnInit(int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(row, col, rot, color);
            m_RotY = rot;
            transform.localRotation = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);

            for (int i = 0; i < rot; i++)
            {
                CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);
            }
        }

        private void OnMouseDown()
        {
            if (!IsCanRotate()) return;
            if (IsRotating) return;
            IsRotating = true;
            m_Tween?.Kill(true);

            m_RotY = (m_RotY + 1) % 4;
            var rot = Quaternion.Euler(0f, TotalRots[m_RotY], 0f);
            transform.DOLocalRotateQuaternion(rot, .3f).OnComplete(() => {
                IsRotating = false;
                CurerentLine = MathHelper.RotateMatrix(CurerentLine, 3);

                //for (int i = 0; i < 3; i++)
                //{
                //    var str = "";
                //    for (int j = 0; j < 3; j++)
                //    {
                //        str += CurerentLine[i, j];
                //    }
                //    Debug.LogError(str + "\n");
                //}
                //EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
            });
        }
    }
}
