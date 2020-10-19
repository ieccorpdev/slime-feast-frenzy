using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class BaseGrid : BaseComponent
    {
        protected Tween m_Tween;

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0)
        {
            base.OnInit(gridType, row, col, rot, color);
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
                EventController.RaiseEvent(EventList.GamePlay.CHECK_GAME_OVER);
            });
        }
    }
}
