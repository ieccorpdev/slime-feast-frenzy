using System;
using _IEC.Features.VipProHackerUITutorial;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class MagicPouchTutorial : _BaseBehaviour
    {
        [SerializeField] private Button infoBtn;
        [SerializeField] private RectTransform infoBtnRect;
        
        private HighLightCanvas HighLightCanvas => Pool.Get<HighLightCanvas>();

        private void Start()
        {
            infoBtn.onClick.AddListener(CompleteTutorial);
        }

        void CompleteTutorial()
        {
            HighLightCanvas.RemoveCurrentHighLight();
        }
        
        public void RunTutorial()
        {
            HighLightCanvas.HighLightUI(infoBtnRect,true,true);
        }
    }
}