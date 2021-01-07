using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class IslandComponent : MonoBehaviour
    {
        private Game m_Game;

        private Game Game => m_Game ?? Pool.Get<Game>();

        private Transform Transform => transform;

        private Tween m_Tween;

        private float startEffectMoveTime = 0.4f;

        [SerializeField]
        private Transform m_TransBg, m_TransMain, m_TransRot, m_TransGrassCorner;

        [SerializeField]
        private Transform m_TransGrass3, m_TransGrass4;

        [SerializeField]
        private Transform m_TransRockMain;

        [SerializeField]
        private Transform[] m_TransRocks;

        [SerializeField]
        private MeshRenderer m_TerrainMesh;

        [SerializeField]
        private Material[] m_GrassMats, m_RockMats;

        [SerializeField]
        private MeshRenderer[] m_GrassesMeshs;

        public void OnInit(HolePos holePos, int size)
        {
            var halfSize = size / 2f;

            var mat = Game.listTerrainType[Random.Range(0, 2)];
            m_TerrainMesh.material = mat;

            var posSpawn = GetPosIsland(holePos, size);
            posSpawn.y = 15f;
            Transform.position = posSpawn;
            m_TransBg.localScale = new Vector3(size, size, 1);
            m_TransRot.localRotation = Quaternion.Euler(0f, GetRotYIsland(holePos), 0f);

            OnTweenStart(size * 2);

            m_TransMain.localPosition = new Vector3(halfSize, 0f, -halfSize);

            m_TransGrassCorner.localPosition = new Vector3(-halfSize, 0f, halfSize);
            m_TransGrassCorner.localRotation = Quaternion.Euler(0f, Random.Range(-25f, 25f), 0f);

            var radSize1 = Random.Range(.8f, 1.25f);
            m_TransGrassCorner.localScale = new Vector3(radSize1, radSize1, radSize1);

            m_TransGrass3.gameObject.SetActive(size == 3);
            m_TransGrass4.gameObject.SetActive(!(size == 3));

            var transGroupGrass = (size == 3) ? m_TransGrass3 : m_TransGrass4;

            var count = transGroupGrass.childCount;

            var totalGrassMat = m_GrassMats.Length;
            for (int i = 0; i < count; i++)
            {
                var transGrass = transGroupGrass.GetChild(i);
                transGrass.localRotation = Quaternion.Euler(0f, Random.Range(-25f, 25f), 0f);
                var radSize = Random.Range(.5f, 1f);
                transGrass.localScale = new Vector3(radSize, radSize, radSize);
            }

            var grassCount = m_GrassesMeshs.Length;

            for (int i = 0; i < grassCount; i++)
            {
                var grassMat = m_GrassMats[Random.Range(0, totalGrassMat)];
                m_GrassesMeshs[i].material = grassMat;
            }


            m_TransRockMain.localRotation = Quaternion.Euler(0f, Random.Range(-180f, 180f), 0f);

            var rockActiveCount = 0;
            var rockCount = m_TransRocks.Length;

            var totalRockMat = m_RockMats.Length;

            var minPos = -halfSize + .25f;
            var maxPos = halfSize - .25f;
            for (int i = 0; i < rockCount; i++)
            {
                var isActive = Random.Range(0, 2) == 1;
                if(rockActiveCount >= 5)
                {
                    isActive = false;
                }
                m_TransRocks[i].gameObject.SetActive(isActive);
                if (!isActive) continue;
                rockActiveCount += 1;

                var radPosX = Random.Range(minPos, maxPos);
                var radPosZ = Random.Range(minPos, maxPos);
                m_TransRocks[i].localPosition = new Vector3(radPosX, 0f, radPosZ);

                var radSize = Random.Range(.3f, .5f);
                m_TransRocks[i].localScale = new Vector3(radSize, radSize, radSize);

                var radRotX = Random.Range(-25f, 25f);
                var radRotY = Random.Range(-25f, 25f);
                var radRotZ = Random.Range(-25f, 25f);

                m_TransRocks[i].localRotation = Quaternion.Euler(radRotX, radRotY, radRotZ);

                var rockMat = m_RockMats[Random.Range(0, totalRockMat)];
                m_TransRocks[i].GetComponent<MeshRenderer>().material = rockMat;
            }
        }

        private float GetRotYIsland(HolePos holePos)
        {
            if (holePos == HolePos.TopLeft)
            {
                return 180f;
            }
            else if (holePos == HolePos.TopRight)
            {
                return -90f;
            }
            else if (holePos == HolePos.BotLeft)
            {
                return 90f;
            }
            else
            {
                return 0f;
            }
        }

        private Vector3 GetPosIsland(HolePos holePos, int halfSize)
        {
            var pos = halfSize / 2f;

            if (holePos == HolePos.TopLeft)
            {
                return new Vector3(-pos, 0f, pos);
            }
            else if (holePos == HolePos.TopRight)
            {
                return new Vector3(pos, 0f, pos);
            }
            else if (holePos == HolePos.BotLeft)
            {
                return new Vector3(-pos, 0f, -pos);
            }
            if (holePos == HolePos.BotRight)
            {
                return new Vector3(pos, 0f, -pos);
            }
            else
            {
                return Vector3.zero;
            }
        }

        private void OnTweenStart(int delay)
        {
            m_Tween?.Kill(true);
            var delayTime = (delay * .1f) + .65f;
            m_Tween = transform.DOLocalMoveY(-.1f, startEffectMoveTime).SetDelay(delayTime);
        }

        public void OnDestroyMe()
        {
            PrefabHelper.Destroy(gameObject);
        }

    }
}
