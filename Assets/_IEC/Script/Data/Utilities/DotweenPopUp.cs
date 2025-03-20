using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

namespace IEC
{
    public class DotweenPopUp : MonoBehaviour
    {
        #region Properties

        public enum AnimationType
        {
            Scale,
            Fade
        }

        public AnimationType animationType;

        #region Pop Up

        public RectTransform activeObject;
        public RectTransform tweenObject;

        [ShowIf("animationType", AnimationType.Scale)]
        public bool popUpOffset;

        [ShowIf("animationType", AnimationType.Scale)]
        public bool scaleToTarget;
        
        [ShowIf("animationType", AnimationType.Fade)]
        public CanvasGroup canvasGroup;

        [Header("Pop Up")]
        public float popUpDuration = 0.4f;

        [ShowIf("animationType", AnimationType.Scale)]
        public Vector3 from;

        [ShowIf("animationType", AnimationType.Scale)]
        public Vector3 to = Vector3.one * 1.25f;

        public Ease easePopUp = Ease.OutQuad;

        #endregion

        #region Dismiss

        [Header("Dismiss")] 
        public float dismissDuration = 0.3f;
        public Ease easeDismiss = Ease.InSine;

        #endregion

        public UnityEvent onGainFocus;
        public UnityEvent onLostFocus;
        public UnityEvent onShowComplete;
        public UnityEvent onHideComplete;

        private string m_TweenEffectID;

        #endregion

#if UNITY_EDITOR

        private void OnValidate()
        {
            if (tweenObject == null)
                tweenObject = GetComponent<RectTransform>();

            switch (animationType)
            {
                case AnimationType.Scale:
                    if (canvasGroup != null)
                    {
                        UnityEditor.EditorApplication.delayCall += () =>
                        {
                            DestroyImmediate(canvasGroup, true);
                            canvasGroup = null;
                        };
                    }

                    break;
                case AnimationType.Fade:
                    if (canvasGroup == null)
                    {
                        var tweenObjectCanvasGroup = tweenObject.GetComponent<CanvasGroup>();
                        canvasGroup = tweenObjectCanvasGroup != null
                            ? tweenObjectCanvasGroup
                            : tweenObject.gameObject.AddComponent<CanvasGroup>();
                    }

                    break;
            }
        }

#endif
        
        private void Awake()
        {
            m_TweenEffectID = $"TweenEffectID_{GetInstanceID()}";
        }

        public void SetActiveObject(bool isActive)
        {
            if (activeObject != null)
                activeObject.gameObject.SetActive(isActive);
            else
                tweenObject.gameObject.SetActive(isActive);
        }

        public bool IsShow() => activeObject != null
            ? activeObject.gameObject.activeInHierarchy
            : tweenObject.gameObject.activeInHierarchy;
        

        [Button]
        public void Show()
        {
            if (IsShow()) return;
            DOTween.Kill(m_TweenEffectID);
            SetActiveObject(true);
            onGainFocus?.Invoke();
            
            switch (animationType)
            {
                case AnimationType.Scale:
                    ScaleUp();
                    break;
                case AnimationType.Fade:
                    FadeIn();
                    break;
            }
        }

        [Button]
        public void Hide()
        {
            DOTween.Kill(m_TweenEffectID);
            onLostFocus?.Invoke();
            
            switch (animationType)
            {
                case AnimationType.Scale:
                    ScaleDown();
                    break;
                case AnimationType.Fade:
                    FadeOut();
                    break;
            }
        }

        private void ScaleUp()
        {
            tweenObject.transform.localScale = popUpOffset ? from : Vector3.zero;

            if (to == Vector3.one || scaleToTarget)
            {
                ScaleUp(popUpDuration, easePopUp);
            }
            else
            {
                ScaleUp(popUpDuration / 2, easePopUp, Vector3.one);
            }
        }

        private void ScaleUp(float duration, Ease curve)
        {
            tweenObject.transform.DOScale(to, duration).SetEase(curve).SetId(m_TweenEffectID)
                .OnComplete(onShowComplete.Invoke);
        }

        private void ScaleUp(float duration, Ease curve, Vector3 finalTarget)
        {
            DOTween.Sequence().SetId(m_TweenEffectID)
                .Append(tweenObject.transform.DOScale(to, duration / 2).SetEase(curve))
                .Append(tweenObject.transform.DOScale(finalTarget, duration).SetEase(curve))
                .OnComplete(onShowComplete.Invoke);
        }

        private void ScaleDown()
        {
            if (!tweenObject.gameObject.activeInHierarchy)
                return;

            tweenObject.transform.DOScale(0, dismissDuration).SetEase(easeDismiss).SetId(m_TweenEffectID).OnComplete(OnHideComplete);
        }

        private void FadeIn()
        {
            canvasGroup.alpha = 0;
            canvasGroup.DOFade(1, popUpDuration).SetEase(easePopUp).SetId(m_TweenEffectID).OnComplete(onShowComplete.Invoke);
        }

        private void FadeOut()
        {
            if (!tweenObject.gameObject.activeInHierarchy)
                return;

            canvasGroup.DOFade(0, dismissDuration).SetEase(easeDismiss).SetId(m_TweenEffectID).OnComplete(OnHideComplete);
        }
        
        private void OnHideComplete()
        {
            SetActiveObject(false);
            onHideComplete?.Invoke();
        }
    }
}