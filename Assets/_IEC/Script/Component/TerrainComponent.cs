using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace IEC
{
    public class TerrainComponent : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer m_Mesh, m_MeshIsLand;

        [SerializeField]
        private GameObject m_LockObj;

        [SerializeField]
        private Transform[] m_TransArrows;

        private MaterialPropertyBlock m_TerrainMat;

        private bool m_IsTweenArrow, m_IsInit;

        private int m_ArrowCount, m_ArrowIndex;

        public float m_FillAmount;

        private Vector3 m_WaterPos;

        public void SetupTerrain(Material mat, bool isHaveGray, bool isFyingIsLand, bool isInit)
        {
            if (isInit)
            {
                ResetAll();
                if (isHaveGray)
                {
                    SetPos(Vector3.one * 30);
                }
            }
            else
            {
                OnUpdate(m_FillAmount);
                SetPos(m_WaterPos);
            }

            if (isFyingIsLand)
            {
                m_MeshIsLand.material = mat;
            }
            else
            {
                m_Mesh.material = mat;
                m_TerrainMat.SetFloat(GameShaderValue.SHADER_VERSION, isHaveGray ? 1 : 0);
                ApplyData();
            }

            m_Mesh.gameObject.SetActive(!isFyingIsLand);
            m_MeshIsLand.gameObject.SetActive(isFyingIsLand);
        }

        public void SetPos(Vector3 pos)
        {
            m_WaterPos = pos;
            m_TerrainMat.SetVector(GameShaderValue.SHADER_POS, m_WaterPos);
            ApplyData();
        }

        public void OnUpdate(float fillAmount)
        {
            m_FillAmount = fillAmount;
            m_TerrainMat.SetFloat(GameShaderValue.SHADER_FILL_AMOUNT, m_FillAmount);
            ApplyData();
        }

        private void ApplyData()
        {
            m_Mesh.SetPropertyBlock(m_TerrainMat);
        }

        public void SetActiveLock(bool isActive)
        {
            m_LockObj.SetActive(isActive);
        }

        private Vector3 m_StartPos1, m_EndPos1, m_StartPos2, m_EndPos2;
        private Vector3[] arrayPos = new Vector3[3];
        private List<Vector3> listPos = new List<Vector3>();

        public void SetArrowPos(Vector3 startPos1, Vector3 endPos1, Vector3 startPos2, Vector3 endPos2)
        {
            listPos.Clear();

            m_StartPos1 = GetLocalPos(startPos1);
            m_EndPos1 = GetLocalPos(endPos1);
            m_StartPos2 = GetLocalPos(startPos2);
            m_EndPos2 = GetLocalPos(endPos2);

            arrayPos[0] = m_EndPos1;
            arrayPos[1] = m_StartPos2;
            arrayPos[2] = m_EndPos2;
        }

        public void SetActiveArrow()
        {
            StartCoroutine(IETweenArrow());
        }

        private IEnumerator IETweenArrow()
        {
            m_IsTweenArrow = true;
            while (m_IsTweenArrow)
            {
                SpawnArrow(m_ArrowIndex);

                m_ArrowIndex += 1;
                if (m_ArrowIndex == m_ArrowCount)
                {
                    m_ArrowIndex = 0;
                }

                yield return new WaitForSeconds(.75f);
            }
        }

        private void SpawnArrow(int index)
        {
            var transArrow = m_TransArrows[index];
            transArrow.gameObject.SetActive(true);
            transArrow.localPosition = m_StartPos1;

            transArrow.DOLocalPath(arrayPos, 2f, PathType.CatmullRom, PathMode.Full3D)
                .SetLookAt(.01f).SetEase(Ease.Linear)
                .OnComplete(() => {
                    transArrow.gameObject.SetActive(false);
                });
        }

        private Vector3 GetLocalPos(Vector3 posInput)
        {
            var pos = transform.InverseTransformPoint(posInput);
            pos.y = 0.015f;
            return pos;
        }

        public void ResetAll()
        {
            if (!m_IsInit)
            {
                m_IsInit = true;
                m_ArrowCount = m_TransArrows.Length;
                m_TerrainMat = new MaterialPropertyBlock();
            }

            OnUpdate(0f);
            gameObject.SetActive(true);
            OnKillTween();
            SetActiveLock(false);
        }

        private void OnKillTween()
        {
            m_IsTweenArrow = false;
            for (int i = 0; i < m_ArrowCount; i++)
            {
                m_TransArrows[i].gameObject.SetActive(false);
            }
        }
    }
}