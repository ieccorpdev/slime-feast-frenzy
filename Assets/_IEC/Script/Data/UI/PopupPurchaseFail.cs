using DG.Tweening;
using UnityEngine.UI;

namespace IEC
{
    public class PopupPurchaseFail : BasePopup
    {
        public Button btnContinue;

        private void Start()
        {
            SetEvent(btnContinue, () =>
            {
                Hide();
            });
        }

        public override void Show()
        {
            base.Show();
            DOVirtual.DelayedCall(2f, Hide);
        }
    }
}
