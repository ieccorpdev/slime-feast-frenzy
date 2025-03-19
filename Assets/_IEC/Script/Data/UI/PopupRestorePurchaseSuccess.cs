using DG.Tweening;
using UnityEngine.UI;

namespace IEC
{
    public class PopupRestorePurchaseSuccess : BasePopup
    {
        public Button OkButton;

        public void Start()
        {
            SetEvent(OkButton,Hide);
        }
    
        public override void Show()
        {
            base.Show();
            //DOVirtual.DelayedCall(2f, Hide);
        }
    }
}
