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

        [SerializeField]
        private GameObject[] m_Grasses;

        private List<int> m_ListUsed = new List<int>();

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
            OnDOShakePosition();
        }

        public void SetupWall(bool isHaveHole, bool isBotTop, bool isReverse)
        {
            m_WallObj.SetActive(!isHaveHole);
            m_WallHoleObj.SetActive(isHaveHole);

            m_WallHoleObj.transform.localRotation = Quaternion.Euler(0f, GetRotY(isBotTop, isReverse), 0f);
            CurrentLine = isHaveHole ? (isBotTop ? DefaultLineWallTopBot : DefaultLineWallLeftRight) : OriginLine;

            m_ListUsed.Clear();

            var total = m_Grasses.Length;

            for (int i = 0; i < total; i++)
            {
                m_Grasses[i].SetActive(false);
            }

            if(Game.abTheme == 0)
            {
                var radCount = Random.Range(3, Game.maxGrassOnWall);

                for (int i = 0; i < radCount; i++)
                {
                    var index = MathHelper.GetRandomIndexByList(m_ListUsed, total);
                    m_ListUsed.Add(index);

                    m_Grasses[index].SetActive(true);
                }
            }

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
