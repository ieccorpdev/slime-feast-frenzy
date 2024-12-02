using DG.Tweening;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRestorePurchaseFail : BasePopup
    {
        public override void Show()
        {
            base.Show();
            DOVirtual.DelayedCall(2f, Hide);
        }
    }
}
