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

        private bool m_IsScale, m_IsGrowUp, isChanged;

        private float m_FillAmount;

        private Vector3 m_GrowingPos;

        public void OnInit()
        {
            if(materialPropertyBlock == null)
            {
                materialPropertyBlock = new MaterialPropertyBlock();
            }
            
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

        public void SetGrowing(bool isGrowUp, Vector3 pos)
        {
            m_GrowingPos = pos;
            m_IsGrowUp = isGrowUp;
            m_IsScale = true;

            m_FillAmount = isGrowUp ? 0f : m_FillAmount;
        }

        public void OnUpdate(float deltaTime)
        {
            isChanged = false;
            if (m_IsScale)
            {
                isChanged = true;
                m_FillAmount += m_IsGrowUp ? deltaTime : -deltaTime;
                if (m_FillAmount <= 0 || m_FillAmount >= 1.5f)
                {
                    m_IsScale = false;
                }
            }

            if (waterColor != m_NewColor)
            {
                isChanged = true;
                waterColor = Color.Lerp(waterColor, m_NewColor, deltaTime * 2f);
            }

            //if (isChanged)
            //{
            //    OnUpdatePropertyBlock(materialPropertyBlock);
            //}

            OnUpdatePropertyBlock(materialPropertyBlock);
        }

        private void SetUpPropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetColor("_BorderColor", borderColor);

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

            ApplyData(propertyBlock);
        }

        private void OnUpdatePropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetColor("_WaterColor", waterColor);
            propertyBlock.SetVector("_GLOBALMaskPosition", m_GrowingPos);
            propertyBlock.SetFloat("_GLOBALMaskRadius", m_FillAmount);

            ApplyData(propertyBlock);
        }

        private void ApplyData(MaterialPropertyBlock propertyBlock)
        {
            waterRenderers[0].SetPropertyBlock(propertyBlock);
        }
    }
}