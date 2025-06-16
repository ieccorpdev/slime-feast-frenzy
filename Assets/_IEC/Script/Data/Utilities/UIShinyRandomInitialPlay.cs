//using Coffee.UIEffects;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

namespace IEC
{
    public class UIShinyRandomInitialPlay : MonoBehaviour
    {
        //public UIShiny shiny;
        public Vector2 playDelayRange = new Vector2(0, 3);
        public Vector2 loopDelayRange = new Vector2(2, 3);
        
        private void OnValidate()
        {
            //if (shiny == null)
            //{
            //    shiny = GetComponent<UIShiny>();
            //    SetDelay();
            //}
        }
        
        [Button]
        public void SetDelay()
        {
            //shiny.effectPlayer.initialPlayDelay = Random.Range(playDelayRange.x, playDelayRange.y);
            //shiny.effectPlayer.loopDelay = Random.Range(loopDelayRange.x, loopDelayRange.y);
        }
    }
}