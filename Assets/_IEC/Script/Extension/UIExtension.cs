using DG.Tweening;
using UnityEngine;

namespace _IEC.Features.Common.Extension
{
    public static class UIExtension
    {
        public static Sequence DoShowForDuration(this Transform transform, float time)
        {
            if (transform.gameObject.activeInHierarchy == false)
            {
                transform.gameObject.SetActive(true);
            }

            var id = GetTweenId(transform, "DoShowForDuration");
            if (DOTween.TweensById(id) == null)
            {
                transform.localScale = Vector3.one * 0.1f;
            }
            else
                DOTween.Kill(id);

            return DOTween.Sequence().SetId(id)
                .Append(transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack))
                .AppendInterval(time)
                .Append(transform.DOScale(Vector3.one * 0.1f, 0.25f).SetEase(Ease.InBack).OnComplete(() =>
                {
                    transform.gameObject.SetActive(false);
                }));
        }

        public static Sequence DoHideForDuration(this Transform transform, float time)
        {
            if (transform.gameObject.activeInHierarchy == false) return null;
            var id = GetTweenId(transform, "DoHideForDuration");
            DOTween.Kill(id);
            return DOTween.Sequence().SetId(id)
                .Append(transform.DOScale(Vector3.one * 0.1f, time))
                .AppendCallback(() => { transform.gameObject.SetActive(false); });
        }

        private static string GetTweenId(Transform transform, string prefix)
        {
            return prefix + transform.gameObject.GetInstanceID();
        }
    }
}