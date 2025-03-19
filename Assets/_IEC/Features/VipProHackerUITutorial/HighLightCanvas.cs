using System;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.VipProHackerUITutorial
{
    public class HighLightCanvas : _BaseBehaviour
    {
        public Image imgDark;
        public Canvas myCanvas;
        public RectTransform handTut;
        public int CanvasSortingOrder => myCanvas.sortingOrder;
        private Canvas m_TempCanvas;
        private GraphicRaycaster m_GraphicRaycaster;
        private RectTransform m_CurrentHighLightUI;
        private bool m_OldIsOverrideSorting;
        private int m_OldRenderSorting;
        private Canvas m_OldCanvas;
        [Header("Custom")] 
        public GameObject tapToCollectTut;
        public GameObject needSpaceTut;
        public GameObject clearHelperTut;
        public void Awake()
        {
            Pool.Set(this);
        }
        public void HighLightUI(RectTransform rectTransform,bool isRaycastTarget,bool isUsingHandTut = false,Vector2? positionOffset = null)
        {
            //Debug.LogError("highligjht");
            if (m_CurrentHighLightUI != null)
            {
                RemoveCurrentHighLight();
            }

            m_OldCanvas = null;
            m_GraphicRaycaster = null;
            m_TempCanvas = null;
            m_CurrentHighLightUI = rectTransform;
            imgDark.gameObject.SetActive(true);
            var currentCanvas = rectTransform.GetComponent<Canvas>();

            var currentGraphicTarget = rectTransform.GetComponent<GraphicRaycaster>();
            if (currentCanvas == null)
            {
                m_TempCanvas = rectTransform.gameObject.AddComponent<Canvas>();
                m_TempCanvas.overrideSorting = true;
                m_TempCanvas.sortingOrder = CanvasSortingOrder + 1;
                TweenControl.GetInstance().DelayCall(transform,Time.deltaTime*2, () =>
                {
                    m_TempCanvas.overrideSorting = true;
                });
            }
            else
            {
                currentCanvas.enabled = true;
                m_OldRenderSorting = currentCanvas.sortingOrder;
                m_OldIsOverrideSorting = currentCanvas.overrideSorting;
                currentCanvas.overrideSorting = true;
                currentCanvas.sortingOrder = CanvasSortingOrder + 1;
                m_OldCanvas = currentCanvas;
            }

            if (isRaycastTarget)
            {
                if (currentGraphicTarget == null)
                {
                    m_GraphicRaycaster = rectTransform.gameObject.AddComponent<GraphicRaycaster>();
                }
                else
                {
                   currentGraphicTarget.enabled = true;
                }
            }

            if (isUsingHandTut)
            {
                handTut.gameObject.SetActive(true);
                //if (positionOffset != null)
                //{
                //    handTut.transform.position = rectTransform.transform.position;
                //    handTut.GetComponent<RectTransform>().anchoredPosition += positionOffset.Value;
                //}
                //else
                //{
                //    handTut.position = rectTransform.position;
                //}
            }
        }
        
        public void RemoveCurrentHighLight()
        {
            tapToCollectTut.gameObject.SetActive(false);
            needSpaceTut.gameObject.SetActive(false);
            clearHelperTut.gameObject.SetActive(false);
            if (m_CurrentHighLightUI == null) return;
            
            if (m_GraphicRaycaster != null)
            {
                m_GraphicRaycaster.enabled = false;
                DestroyImmediate(m_GraphicRaycaster);
            }
            
            if (m_TempCanvas != null)
            {
                m_TempCanvas.enabled = false;
                DestroyImmediate(m_TempCanvas);
            }
            imgDark.gameObject.SetActive(false);
            handTut.gameObject.SetActive(false);
            m_CurrentHighLightUI = null;
            if (m_OldCanvas != null)
            {
                m_OldCanvas.overrideSorting = m_OldIsOverrideSorting;
                m_OldCanvas.sortingOrder = m_OldRenderSorting;
            }

        }

        public void SetHandTutToPos(Vector3 worldPos)
        {
            handTut.transform.position = worldPos;
        }
    }
}
