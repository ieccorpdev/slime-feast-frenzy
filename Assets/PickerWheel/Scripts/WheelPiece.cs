using UnityEngine ;
using IEC;

namespace EasyUI.PickerWheelUI
{
   [System.Serializable]
   public class WheelPiece
   {
      public UnityEngine.Sprite Icon;
      public string Label;
      public RewardsLuckyWheel dataReward;
      [Tooltip("Reward amount")] public int Amount;

      [Tooltip("Probability in %")] [Range(0f, 100f)]
      public float Chance = 100f;

      [HideInInspector] public int Index;
      [HideInInspector] public double _weight = 0f;
   }
}
