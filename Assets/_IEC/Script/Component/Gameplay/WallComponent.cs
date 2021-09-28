using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class WallComponent : BaseComponent
    {
        [SerializeField]
        private GameObject m_WallObj, m_WallHoleObj, m_TeleportPipe;

        [SerializeField]
        private MeshRenderer[] m_WallMesh;

        [SerializeField]
        private MeshRenderer m_HoleMesh;

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

        public void SetupWall(bool isHaveHole, bool isBotTop, bool isReverse)
        {
            m_WallObj.SetActive(!isHaveHole && !Game.isHaveIsLand);
            m_WallHoleObj.SetActive(isHaveHole && !Game.isHaveIsLand);

            m_TeleportPipe.SetActive(isHaveHole && Game.isHaveIsLand);

            var rot = GetRotY(isBotTop, isReverse);
            m_WallHoleObj.transform.localRotation = Quaternion.Euler(0f, rot, 0f);
            m_TeleportPipe.transform.localRotation = Quaternion.Euler(0f, rot + 180f, 0f);
            CurrentLine = isHaveHole ? (isBotTop ? DefaultLineWallTopBot : DefaultLineWallLeftRight) : OriginLine;

            OnUpdateSetTerrain();
        }

        public override void OnUpdateSetTerrain()
        {
            for (int i = 0; i < m_WallMesh.Length; i++)
            {
                m_WallMesh[i].material = Game.enviromentConfig.wallMat;
            }

            m_HoleMesh.material = Game.enviromentConfig.holeMat;
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
