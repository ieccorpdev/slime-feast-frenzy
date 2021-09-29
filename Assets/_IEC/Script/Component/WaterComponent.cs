using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WaterComponent : _BaseBehaviour
    {
        [SerializeField] private Renderer waterRenderers;

        [SerializeField]
        private bool m_IsMainWater;

        private MaterialPropertyBlock m_WaterMat;

        private Color m_CurColor, m_NewColor;
        public bool m_IsScale, m_IsGrowUp, isChanged;
        public float m_FillAmount;

        private System.Action m_GrowUpDoneCallBack;
        private System.Action<float> m_ScaleCallBack;

        public void OnInit(System.Action callBack, System.Action<float> scaleCallBack)
        {
            if (m_WaterMat == null)
            {
                m_WaterMat = new MaterialPropertyBlock();
            }

            m_GrowUpDoneCallBack = callBack;
            m_ScaleCallBack = scaleCallBack;
            m_FillAmount = 0f;
            m_IsScale = false;
            m_IsGrowUp = false;

            SetUpPropertyBlock();
        }

        public void SetColor(Color color, bool isUpdateNow)
        {
            m_NewColor = color;
            
            if (isUpdateNow)
            {
                m_CurColor = color;
                OnUpdatePropertyBlock();
            }
        }

        public void SetGrowing(bool isGrowUp, Vector3 pos, int delay)
        {
            UpdatePos(pos);
            m_FillAmount = isGrowUp ?  (-delay * MAX_AMOUNT / 1.5f) : m_FillAmount;

            m_IsGrowUp = isGrowUp;
            m_IsScale = true;
        }

        public void UpdatePos(Vector3 pos)
        {
            m_WaterMat.SetVector(GameShaderValue.SHADER_POS, pos);
            ApplyData();
        }

        float deltaAmount;
        float ratioTime;

        float RATIO_TIME_UP = .3f;
        float RATIO_TIME_DOWN = .6f;
        float MAX_AMOUNT = 1.1f;

        public void OnUpdate(float deltaTime)
        {
            isChanged = Application.isEditor;
            if (m_IsScale)
            {
                isChanged = true;

                deltaAmount = m_IsGrowUp ? deltaTime : -deltaTime;
                ratioTime = m_IsGrowUp ? RATIO_TIME_UP : RATIO_TIME_DOWN;
                m_FillAmount += 1.0f / ratioTime * deltaAmount;

                if (m_IsGrowUp)
                {
                    if (m_FillAmount >= MAX_AMOUNT)
                    {
                        m_IsScale = false;

                        if (m_IsMainWater)
                        {
                            m_GrowUpDoneCallBack?.Invoke();
                        }
                    }
                }
                else
                {
                    if (m_FillAmount <= 0)
                    {
                        m_IsScale = false;
                    }
                }

                if (m_IsMainWater)
                {
                    m_ScaleCallBack?.Invoke(m_FillAmount * 1.1f);
                }
            }

            if (m_CurColor != m_NewColor)
            {
                isChanged = true;
                m_CurColor = Color.Lerp(m_CurColor, m_NewColor, deltaTime * 2f);
            }

            if (isChanged)
            {
                OnUpdatePropertyBlock();
            }

            if (Application.isEditor)
            {
                SetUpPropertyBlock();
            }
        }

        private void SetUpPropertyBlock()
        {
            m_WaterMat.SetVector(GameShaderValue.SHADER_TILING, GamePlay.waterEffectData.waterTile);
            m_WaterMat.SetVector(GameShaderValue.SHADER_DIST_TILING, GamePlay.waterEffectData.distortionTile);
            m_WaterMat.SetVector(GameShaderValue.SHADER_MOVE_DIRECTIONG, GamePlay.waterEffectData.moveDir);
            m_WaterMat.SetTexture(GameShaderValue.SHADER_WATER_TEX, GamePlay.waterEffectData.waterTex);
            m_WaterMat.SetTexture(GameShaderValue.SHADER_DIST_TEX, GamePlay.waterEffectData.distTex);
            m_WaterMat.SetFloat(GameShaderValue.SHADER_VERSION, Game.waterEffectVersion);
            OnUpdatePropertyBlock();
        }

        private void OnUpdatePropertyBlock()
        {
            m_WaterMat.SetColor(GameShaderValue.SHADER_COLOR, m_CurColor);
            m_WaterMat.SetFloat(GameShaderValue.SHADER_FILL_AMOUNT, m_FillAmount);

            ApplyData();
        }

        public void ApplyData()
        {
            waterRenderers.SetPropertyBlock(m_WaterMat);
        }
    }
}