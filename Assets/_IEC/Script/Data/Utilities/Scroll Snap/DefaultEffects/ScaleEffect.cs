using UnityEngine;

namespace LightScrollSnap
{
    [CreateAssetMenu(fileName = "ScaleEffect", menuName = "ScrollSnapEffect/Scale Effect")]
    public class ScaleEffect : BaseScrollSnapEffect
    {
        public Vector3 selectedItemScale = Vector3.one * 1.25f;
        public Vector3 unselectedItemScale = Vector3.one;

        public override void OnItemUpdated(RectTransform transform, float displacement)
        {
            Scale(transform, displacement);
        }

        private void Scale(RectTransform transform, float displacement)
        {
            var ratio = GetEffectRatioAbs(displacement);
            var diff = selectedItemScale - unselectedItemScale;
            transform.localScale = unselectedItemScale + diff * ratio;
        }
    }
}