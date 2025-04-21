using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace IEC
{
    public class EffectUI : MonoBehaviour
    {
        public enum EffectType
        {
            INSTANTLY,
            GROUP_CANVAS_FADE,
            SLIDE_RIGHT_LEFT,
        }

        public enum ActionType
        {
            SHOW,
            HIDE
        }

        public ActionType action;
        public EffectType effect;

        public Ease ease = Ease.InQuad;
        public float time = 0.3f;

        public System.Action<bool> onDone;

        string EffectID => "EffectUI_" + action + gameObject.GetInstanceID();

        private void OnDestroy()
        {
            DOTween.Kill(EffectID);
        }

        public void TestEffect()
        {
            switch (action)
            {
                case ActionType.SHOW:
                {
                    DoShowHide(gameObject, true);
                    break;
                }
                case ActionType.HIDE:
                {
                    DoShowHide(gameObject, false);
                    break;
                }
            }
        }

        public static void DoShowHide(GameObject obj, bool isShow, System.Action<bool> onDone = null)
        {
            var action = isShow ? ActionType.SHOW : ActionType.HIDE;
            var component = obj.GetComponents<EffectUI>().Where(x => x.action == action).LastOrDefault();
            if (component != null)
            {
                component.onDone = onDone;
                component.DoShowHide(isShow);
            }
            else
            {
                obj.SetActive(isShow);
                onDone?.Invoke(isShow);
            }
        }

        Sequence SetUpEffect()
        {
            DOTween.Kill(EffectID);

            if (!gameObject.activeSelf)
            {
                gameObject.SetActive(true);
            }

            var sequence = DOTween.Sequence();
            sequence.SetId(EffectID);
            return sequence;
        }

        void DoShowHide(bool isShow)
        {
            if (effect == EffectType.INSTANTLY)
            {
                gameObject.SetActive(isShow);
                onDone?.Invoke(isShow);
                return;
            }

            var sequence = SetUpEffect();
            switch (effect)
            {
                case EffectType.GROUP_CANVAS_FADE:
                {
                    var from = isShow ? 0f : 1f;
                    var to = isShow ? 1f : 0f;
                    sequence.Join(GroupCanvasFade(from, to, time, ease));
                    break;
                }
                case EffectType.SLIDE_RIGHT_LEFT:
                {
                    sequence.Join(SlideRightToLeft(isShow, time, ease));
                    break;
                }
            }

            sequence.OnComplete(() =>
            {
                if (!isShow)
                {
                    gameObject.SetActive(false);
                }
                onDone?.Invoke(isShow);
            });
        }

        Tween GroupCanvasFade(float from, float to, float time, Ease ease)
        {
            var groupCanvas = GetComponent<CanvasGroup>();
            var sequence = DOTween.Sequence();
            groupCanvas.alpha = from;
            sequence.Append(groupCanvas.DOFade(from, 0))
                    .Append(groupCanvas.DOFade(to, time).SetEase(ease));
            return sequence;
        }

        Tween SlideRightToLeft(bool isShow, float time, Ease ease)
        {
            var r = GetComponent<RectTransform>();
            var width = r.rect.width;
            var height = r.rect.height;
            r.anchorMin = 0.5f * Vector3.one;
            r.anchorMax = 0.5f * Vector3.one;
            r.sizeDelta = new Vector2(width, height);
            var from = isShow ? r.sizeDelta.x : 0f;
            var to = isShow ? 0f : -1f * r.sizeDelta.x;

            var sequence = DOTween.Sequence();
            sequence.Append(r.DOAnchorPosX(from, 0))
                    .Append(r.DOAnchorPosX(to, time).SetEase(ease));
            return sequence;
        }
    }
}