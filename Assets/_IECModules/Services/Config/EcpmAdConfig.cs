using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    [CreateAssetMenu(fileName = "EcpmAd", menuName = "Services/EcpmAd")]
    public class EcpmAdConfig : ScriptableObject
    {
        public AdsConfig Config;
        public double Ecpm;
    }
}
