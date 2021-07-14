using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using NaughtyAttributes;

namespace IEC
{
    public class EffectUI : MonoBehaviour
    {
        public enum EffectType
        {
            INSTANTLY,
            GROUP_CANVAS_FADE
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

        string EffectID => "EffectUI_" + action + gameObject.GetInstanceID();

        [Button]
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

        public static void DoShowHide(GameObject obj, bool isShow)
        {
            var action = isShow ? ActionType.SHOW : ActionType.HIDE;
            var component = obj.GetComponents<EffectUI>().Where(x => x.action == action).LastOrDefault();
            if (component != null)
            {
                component.DoShowHide(isShow);
            }
            else
            {
                obj.SetActive(isShow);
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
            }

            sequence.OnComplete(() =>
            {
                if (!isShow)
                {
                    gameObject.SetActive(false);
                }
            });
        }

        Tween GroupCanvasFade(float from, float to, float time, Ease ease)
        {
            var groupCanvas = GetComponent<CanvasGroup>();
            groupCanvas.alpha = from;
            return groupCanvas.DOFade(to, time).SetEase(ease);
        }

    }
}