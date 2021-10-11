using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class WallComponent : BaseComponent
    {
        [SerializeField]
        private GameObject[] m_WallTypeObj, m_WallPipeTypeObj;

        [SerializeField]
        private Transform m_TransGroupHole;

        [SerializeField]
        private MeshRenderer m_WallMesh1;

        [SerializeField]
        private MeshRenderer m_HoleMesh1;

        protected override int[,] OriginLine => new int[3, 3]
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

        private int m_WallTypeCount, m_PipeTypeCount;

        private StartPoint m_StartPoint;
        private bool m_IsHavePipe;

        protected override void OnHandleMouseDown()
        {
            base.OnHandleMouseDown();
            if (Game.isFlyingIsland) return;
            OnDOShakePosition();
        }

        private void OnDOShakePosition()
        {
            if (!Game.isPlaying) return;
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_LEAVES_EFFECT, new EventParam(this, CurrentWorldPos));
        }

        public void SetupWall(bool isHavePipe, StartPoint point, bool isReverse)
        {
            m_StartPoint = point;
            m_IsHavePipe = isHavePipe;

            bool isBotTop = m_StartPoint == StartPoint.BOT || m_StartPoint == StartPoint.TOP;

            m_WallTypeCount = m_WallTypeObj.Length;
            m_PipeTypeCount = m_WallPipeTypeObj.Length;

            if (m_IsHavePipe)
            {
                var rot = GetRotY(isBotTop, isReverse);
                m_TransGroupHole.localRotation = Quaternion.Euler(0f, rot, 0f);
            }

            CurrentLine = m_IsHavePipe ? (isBotTop ? DefaultLineWallTopBot : DefaultLineWallLeftRight) : OriginLine;

            OnUpdateSetTerrain();
        }

        public override void OnUpdateSetTerrain()
        {
            var themeConfig = Game.themeConfig;
            var wallType = themeConfig.wallType;
            var wallPipeType = themeConfig.wallPipeType;

            var deltaZ = (m_StartPoint == StartPoint.TOP || m_StartPoint == StartPoint.RIGHT) ? -.12f : .12f;

            if(wallType == 0 || wallType == 3)
            {
                deltaZ = 0;
            }

            for (int i = 0; i < m_WallTypeCount; i++)
            {
                var wall = m_WallTypeObj[i];
                wall.SetActive(i == wallType);

                var transWall = wall.transform;
                var curPos = transWall.localPosition;
                curPos.z = deltaZ;

                transWall.localPosition = curPos;
            }

            for (int i = 0; i < m_PipeTypeCount; i++)
            {
                m_WallPipeTypeObj[i].SetActive(m_IsHavePipe && i == wallPipeType);
            }

            if (wallType == 0)
            {
                m_WallMesh1.material = themeConfig.wallMat;
                m_HoleMesh1.material = themeConfig.holeMat;
            }
        }

        private float GetRotY(bool isBotTop, bool isReverse)
        {
            if (isBotTop)
            {
                return isReverse ? 0f : 180f;
            }
            else
            {
                return isReverse ? 180f : 0f;
            }
        }
    }
}
