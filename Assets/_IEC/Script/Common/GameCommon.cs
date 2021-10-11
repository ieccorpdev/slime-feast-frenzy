using UnityEngine;

namespace IEC
{
    public interface IShaderMask
    {
        void UpdatePos(Vector3 pos);
        void UpdateFillAmount(float fillAmount);
        void ApplyData();
    }
}