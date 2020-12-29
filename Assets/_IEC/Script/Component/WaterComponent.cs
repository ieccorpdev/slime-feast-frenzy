using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class WaterComponent : MonoBehaviour
    {
        [SerializeField] private Renderer[] waterRenderers;

        [Space]
        [SerializeField] private Color waterColor;
        [SerializeField] private Texture waterTex;
        [SerializeField] private Vector2 waterTile;
        [Range(0, 1)] [SerializeField] private float textureVisibility;

        [Space]
        [SerializeField] private Texture distortionTex;
        [SerializeField] private Vector2 distortionTile;

        [Space]
        [SerializeField] private float waterHeight;
        [SerializeField] private float waterDeep;
        [Range(0, 0.1f)] [SerializeField] private float waterDepthParam;
        [Range(0, 1)] [SerializeField] private float waterMinAlpha;

        [Space]
        [SerializeField] private Color borderColor;
        [Range(0, 1)] [SerializeField] private float borderWidth;

        [Space]
        [SerializeField] private Vector2 moveDirection;

        private MaterialPropertyBlock materialPropertyBlock;

        public MaterialPropertyBlock MaterialPropertyBlock
        {
            get { return materialPropertyBlock; }
        }

        private Color m_NewColor;

        public bool m_IsScale, m_IsGrowUp, isChanged;

        public float m_FillAmount;

        private System.Action growUpCallBack;

        private static int Shader_FillAmount = Shader.PropertyToID("_FillAmount");
        private static int Shader_WaterColor = Shader.PropertyToID("_WaterColor");

        public void OnInit(System.Action callBack, bool isNewEffect)
        {
            if (isNewEffect)
            {
                waterTile = new Vector2(.1f, .1f);
                distortionTile = new Vector2(.4f, .4f);
            }
            else
            {
                waterTile = new Vector2(.2f, .2f);
                distortionTile = new Vector2(.06f, .06f);
            }

            if (materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }

            growUpCallBack = callBack;

            m_FillAmount = 0f;
            m_IsScale = false;
            m_IsGrowUp = false;

            SetUpPropertyBlock(MaterialPropertyBlock);
        }

        public void SetColor(Color color, bool isUpdateNow)
        {
            m_NewColor = color;
            
            if (isUpdateNow)
            {
                waterColor = color;
                OnUpdatePropertyBlock(MaterialPropertyBlock);
            }
        }

        public void SetGrowing(bool isGrowUp, Vector3 pos, int delay)
        {
            SetGrowingPos(MaterialPropertyBlock, pos);
            m_FillAmount = isGrowUp ?  (-delay * MAX_AMOUNT / 1.5f) : m_FillAmount;

            m_IsGrowUp = isGrowUp;
            m_IsScale = true;
        }

        private void SetGrowingPos(MaterialPropertyBlock propertyBlock, Vector3 pos)
        {
            propertyBlock.SetVector("_GLOBALMaskPosition", pos);
            ApplyData(propertyBlock);
        }

        float deltaAmount;
        float ratioTime;

        float RATIO_TIME_UP = .3f;
        float RATIO_TIME_DOWN = .6f;
        float MAX_AMOUNT = 1.1f;

        public void OnUpdate(float deltaTime)
        {
            //isChanged = Application.isEditor;
            isChanged = false;

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
                        growUpCallBack?.Invoke();
                    }
                }
                else
                {
                    if (m_FillAmount <= 0)
                    {
                        m_IsScale = false;
                    }
                }
            }

            if (waterColor != m_NewColor)
            {
                isChanged = true;
                waterColor = Color.Lerp(waterColor, m_NewColor, deltaTime * 2f);
            }

            if (isChanged)
            {
                OnUpdatePropertyBlock(materialPropertyBlock);
            }
        }

        private void SetUpPropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetVector("_Tiling", waterTile);
            propertyBlock.SetVector("_DistTiling", distortionTile);
            propertyBlock.SetVector("_MoveDirection", new Vector4(moveDirection.x, 0f, moveDirection.y, 0f));

            if (waterTex != null)
            {
                propertyBlock.SetTexture("_WaterTex", waterTex);
            }

            if (distortionTex != null)
            {
                propertyBlock.SetTexture("_DistTex", distortionTex);
            }

            propertyBlock.SetFloat("_TextureVisibility", textureVisibility);
            propertyBlock.SetFloat("_WaterHeight", waterHeight);
            propertyBlock.SetFloat("_WaterDeep", waterDeep);
            propertyBlock.SetFloat("_WaterDepth", waterDepthParam);
            propertyBlock.SetFloat("_WaterMinAlpha", waterMinAlpha);
            propertyBlock.SetFloat("_BorderWidth", borderWidth);

            OnUpdatePropertyBlock(propertyBlock);

            ApplyData(propertyBlock);
        }

        private void OnUpdatePropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetColor(Shader_WaterColor, waterColor);
            propertyBlock.SetFloat(Shader_FillAmount, m_FillAmount);

            ApplyData(propertyBlock);
        }

        private void ApplyData(MaterialPropertyBlock propertyBlock)
        {
            waterRenderers[0].SetPropertyBlock(propertyBlock);
        }
    }
}