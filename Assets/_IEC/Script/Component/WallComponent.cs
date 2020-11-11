using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class WallComponent : BaseComponent
    {
        [SerializeField]
        private GameObject m_WallObj, m_WallHoleObj;

        private Tween m_Tween;

        protected override int[,] DefaultLine => new int[3, 3]
        {
            { 0, 0, 0},
            { 0, 1, 0},
            { 0, 0, 0}
        };

        protected int[,] DefaultLineWallTopBot => new int[3, 3]
        {
            { 0, 1, 0},
            { 0, 1, 0},
            { 0, 1, 0}
        };

        protected int[,] DefaultLineWallLeftRight => new int[3, 3]
        {
            { 0, 0, 0},
            { 1, 1, 1},
            { 0, 0, 0}
        };

        public override void OnInit(GridType gridType, int row, int col, int rot = 0, int color = 0, int type = 0)
        {
            base.OnInit(gridType, row, col, rot, color, type);
            IsRotating = false;
        }

        protected override void OnMouseDown()
        {
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.075f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, false);
            EventController.RaiseEvent(EventList.GamePlay.GAME_WALL_CLICK, new EventParam(this, CurrentPos));
        }

        public void SetupWall(bool isHaveHole, bool isBotTop, bool isReverse)
        {
            m_WallObj.SetActive(!isHaveHole);
            m_WallHoleObj.SetActive(isHaveHole);

            m_WallHoleObj.transform.localRotation = Quaternion.Euler(0f, GetRotY(isBotTop, isReverse), 0f);
            CurerentLine = isHaveHole ? (isBotTop ? DefaultLineWallTopBot : DefaultLineWallLeftRight) : DefaultLine;
        }

        private float GetRotY(bool isBotTop, bool isReverse)
        {
            if (isBotTop)
            {
                return isReverse ? 180f : 0f;
            }
            else
            {
                return isReverse ? 0f : 180f;
            }
        }
    }
}
