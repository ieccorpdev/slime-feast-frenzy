using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WaterComponent : _BaseBehaviour
    {
        [SerializeField] private Renderer waterRenderers;
        [Space]
        [SerializeField] private Color waterColor;
        [SerializeField] private Vector2 waterTile;

        [Space]
        [SerializeField] private Vector2 distortionTile;

        [Space]
        [SerializeField] private float waterHeight;
        private MaterialPropertyBlock materialPropertyBlock;

        private Color m_NewColor;
        public bool m_IsScale, m_IsGrowUp, isChanged;
        public float m_FillAmount;

        private System.Action m_GrowUpDoneCallBack;
        private System.Action<float> m_ScaleCallBack;

        private static int Shader_FillAmount = Shader.PropertyToID("_FillAmount");
        private static int Shader_WaterColor = Shader.PropertyToID("_WaterColor");
        private static int Shader_MaskPos = Shader.PropertyToID("_MaskPos");

        private bool isMainWater = true;

        public void OnInit(System.Action callBack, System.Action<float> scaleCallBack)
        {
            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            m_GrowUpDoneCallBack = callBack;
            m_ScaleCallBack = scaleCallBack;

            waterTile = Game.isNewWaterEffect ? new Vector2(3f, 3f) : new Vector2(.2f, .2f);
            distortionTile = Game.isNewWaterEffect ? new Vector2(.1f, .1f) : new Vector2(.06f, .06f);

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
            materialPropertyBlock.SetVector(Shader_MaskPos, pos);
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
            //isChanged = false;

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
            materialPropertyBlock.SetVector("_Tiling", waterTile);
            materialPropertyBlock.SetVector("_DistTiling", distortionTile);
            var moveDir = Game.isNewWaterEffect ? new Vector4(-.1f, 0f, .1f, 0f) : new Vector4(-.7f, 0f, .3f, 0f);
            materialPropertyBlock.SetVector("_MoveDirection", moveDir);
            materialPropertyBlock.SetTexture("_WaterTex", Game.isNewWaterEffect ? Config.waterTexNew : Config.waterTexNormal);
            materialPropertyBlock.SetTexture("_DistTex", Game.isNewWaterEffect ? Config.distTexNew : Config.distTexNormal);
            materialPropertyBlock.SetFloat("_WaterHeight", waterHeight);
            materialPropertyBlock.SetFloat("_IsNewWater", Game.isNewWaterEffect ? 1f : 0f);
            OnUpdatePropertyBlock();
        }

        private void OnUpdatePropertyBlock()
        {
            materialPropertyBlock.SetColor(Shader_WaterColor, waterColor);
            materialPropertyBlock.SetFloat(Shader_FillAmount, m_FillAmount);

            ApplyData();
        }

        private void ApplyData()
        {
            waterRenderers.SetPropertyBlock(materialPropertyBlock);
        }
    }
}