using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WaterComponent : _BaseBehaviour
    {
        [SerializeField] private Renderer waterRenderers;
        [SerializeField] private Color waterColor;
        [SerializeField] private float waterHeight;
        private MaterialPropertyBlock materialPropertyBlock;

        private Color m_NewColor;
        public bool m_IsScale, m_IsGrowUp, isChanged;
        public float m_FillAmount;

        private System.Action m_GrowUpDoneCallBack;
        private System.Action<float> m_ScaleCallBack;

        private bool isMainWater = true;

        public void OnInit(System.Action callBack, System.Action<float> scaleCallBack)
        {
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            m_GrowUpDoneCallBack = callBack;
            m_ScaleCallBack = scaleCallBack;
            m_FillAmount = 0f;
            m_IsScale = false;
            m_IsGrowUp = false;

            SetUpPropertyBlock();
        }

        public void SetMainWater(bool isMainWater)
        {
            this.isMainWater = isMainWater;
        }

        public void SetColor(Color color, bool isUpdateNow)
        {
            m_NewColor = color;
            
            if (isUpdateNow)
            {
                waterColor = color;
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
            materialPropertyBlock.SetVector(GameShaderValue.SHADER_POS, pos);
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

                        if (isMainWater)
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

                if (isMainWater)
                {
                    m_ScaleCallBack?.Invoke(m_FillAmount * 1.1f);
                }
            }

            if (waterColor != m_NewColor)
            {
                isChanged = true;
                waterColor = Color.Lerp(waterColor, m_NewColor, deltaTime * 2f);
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
            materialPropertyBlock.SetVector(GameShaderValue.SHADER_TILING, GamePlay.waterEffectData.waterTile);
            materialPropertyBlock.SetVector(GameShaderValue.SHADER_DIST_TILING, GamePlay.waterEffectData.distortionTile);
            materialPropertyBlock.SetVector(GameShaderValue.SHADER_MOVE_DIRECTIONG, GamePlay.waterEffectData.moveDir);
            materialPropertyBlock.SetTexture(GameShaderValue.SHADER_WATER_TEX, GamePlay.waterEffectData.waterTex);
            materialPropertyBlock.SetTexture(GameShaderValue.SHADER_DIST_TEX, GamePlay.waterEffectData.distTex);
            materialPropertyBlock.SetFloat(GameShaderValue.SHADER_VERSION, Game.waterEffectVersion);
            OnUpdatePropertyBlock();
        }

        private void OnUpdatePropertyBlock()
        {
            materialPropertyBlock.SetColor(GameShaderValue.SHADER_COLOR, waterColor);
            materialPropertyBlock.SetFloat(GameShaderValue.SHADER_FILL_AMOUNT, m_FillAmount);

            ApplyData();
        }

        private void ApplyData()
        {
            waterRenderers.SetPropertyBlock(materialPropertyBlock);
        }
    }
}