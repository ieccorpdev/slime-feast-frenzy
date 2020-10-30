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


        private Color m_TempColor, m_NewColor;

        private bool m_IsFading, m_IsIncrease;

        private float m_TempColorAlpha;

        public void OnInit(Color color, bool isShowDefault)
        {
            materialPropertyBlock = new MaterialPropertyBlock();

            m_TempColor = Color.white;

            m_IsFading = false;
            m_IsIncrease = false;
            m_TempColorAlpha = isShowDefault ? 1 : 0;
            m_TempColor.a = m_TempColorAlpha;

            SetColor(color, true);
        }

        public void SetColor(Color color, bool isUpdateNow)
        {
            m_NewColor = color;
            if(isUpdateNow)
            {
                waterColor = color;
                SetUpPropertyBlock(materialPropertyBlock);
            }

            if (waterRenderers != null)
            {
                for (var i = 0; i < waterRenderers.Length; i++)
                {
                    waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
                }
            }
        }

        public void SetFading(bool IsIncrease)
        {
            m_IsIncrease = IsIncrease;
            m_IsFading = true;
        }

        
        public void OnUpdate(float deltaTime)
        {
            if (Application.isEditor)
            {
                SetUpPropertyBlock(materialPropertyBlock);

                if (waterRenderers != null)
                {
                    for (var i = 0; i < waterRenderers.Length; i++)
                    {
                        waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
                    }
                }
            }

            if (m_IsFading)
            {
                m_TempColorAlpha = m_TempColor.a;
                m_TempColorAlpha = Mathf.Clamp(m_TempColorAlpha + (m_IsIncrease ? deltaTime : -deltaTime), 0f, 1f);
                m_TempColor.a = m_TempColorAlpha;
                if (m_TempColorAlpha <= 0 || m_TempColorAlpha >= 1)
                {
                    m_IsFading = false;
                }
                materialPropertyBlock.SetColor("_Tint", m_TempColor);

                if (waterRenderers != null)
                {
                    for (var i = 0; i < waterRenderers.Length; i++)
                    {
                        waterRenderers[i].SetPropertyBlock(materialPropertyBlock);
                    }
                }
            }

            if(waterColor != m_NewColor)
            {
                waterColor = Color.Lerp(waterColor, m_NewColor, deltaTime * 2f);
                materialPropertyBlock.SetColor("_WaterColor", waterColor);
            }
        }

        private void SetUpPropertyBlock(MaterialPropertyBlock propertyBlock)
        {
            propertyBlock.SetColor("_WaterColor", waterColor);
            propertyBlock.SetColor("_Tint", m_TempColor);
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
        }
    }
}