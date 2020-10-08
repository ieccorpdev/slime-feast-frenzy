using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class GridComponent : _BaseBehaviour
    {
        private Tween m_Tween;
        [HideInInspector]
        public bool IsRotating;

        [SerializeField]
        private int[] m_TrueRots;

        private int[] m_TotalRots = new int[] { 0, 90, 180, 270};

        private int angleY => (int)transform.eulerAngles.y;

        private List<int> m_TempListRot = new List<int>();

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

        public void OnInit()
        {
            m_Tween?.Kill(true);
            IsRotating = false;

            m_TempListRot.Clear();

            for (int i = 0; i < m_TotalRots.Length; i++)
            {
                var rot = m_TotalRots[i];
                if (!IsSameRot(rot))
                {
                    m_TempListRot.Add(rot);
                }
            }

            var rdRot = m_TempListRot[Random.Range(0, m_TempListRot.Count)];

            transform.localRotation = Quaternion.Euler(0f, rdRot, 0f);
        }

        private bool IsSameRot(int rot)
        {
            for (int i = 0; i < m_TrueRots.Length; i++)
            {
                if (rot == m_TrueRots[i]) return true;
            }
            return false;
        }

        public bool IsTrueRot()
        {
            if (IsRotating) return false;
            for (int i = 0; i < m_TrueRots.Length; i++)
            {
                if (angleY == m_TrueRots[i]) return true;
            }
            return false;
        }
    }
}