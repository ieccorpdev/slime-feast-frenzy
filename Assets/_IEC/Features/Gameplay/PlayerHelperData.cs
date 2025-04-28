using System;
using IEC;

namespace _IEC.Features.Gameplay
{
    [System.Serializable]
    public class PlayerHelperData
    {
        public int numberMagnet;
        public int numberCompass;
        public int numberFrozenTime;
        public int numberScaleUp;

        public bool isCompleteMagnetTutorial;
        public bool isCompleteCompassTutorial;
        public bool isCompleteFrozenTimeTutorial;
        public bool isCompleteScaleUpTutorial;

        public int GetPool(HelperId helperId)
        {
            return helperId switch
            {
                HelperId.Magnet => numberMagnet,
                HelperId.FrozenTime => numberFrozenTime,
                HelperId.ScaleUp => numberScaleUp,
                HelperId.Compass => numberCompass,
                _ => 0
            };
        }

        public HelperId ConvertResourceType(ResourceType rType)
        {
            switch (rType)
            {
                case ResourceType.Compass:
                    return HelperId.Compass;
                case ResourceType.Vortex:
                    return HelperId.Magnet;
                case ResourceType.Enlarge:
                    return HelperId.ScaleUp;
                case ResourceType.TimeStop:
                    return HelperId.FrozenTime;
            }

            return HelperId.None;
        }
        public void CollectHelper(HelperId helperId, int number)
        {
            switch (helperId)
            {
                case HelperId.Magnet:
                    numberMagnet += number;
                    break;
                case HelperId.FrozenTime:
                    numberFrozenTime += number;
                    break;
                case HelperId.ScaleUp:
                    numberScaleUp += number;
                    break;
                case HelperId.Compass:
                    numberCompass += number;
                    break;
            }
        }
        
        public void SetHelperValue(HelperId helperId, int number)
        {
            switch (helperId)
            {
                case HelperId.Magnet:
                    numberMagnet = number;
                    break;
                case HelperId.FrozenTime:
                    numberFrozenTime = number;
                    break;
                case HelperId.ScaleUp:
                    numberScaleUp = number;
                    break;
                case HelperId.Compass:
                    numberCompass = number;
                    break;
            }
        }

        public void ConsumeHelper(HelperId helperId,int value = 1)
        {
            switch (helperId)
            {
                case HelperId.Magnet:
                    ConsumeNumber(ref numberMagnet,value);
                    break;
                case HelperId.FrozenTime:
                    ConsumeNumber(ref numberFrozenTime,value);
                    break;
                case HelperId.ScaleUp:
                    ConsumeNumber(ref numberScaleUp,value);
                    break;
                case HelperId.Compass:
                    ConsumeNumber(ref numberCompass,value);
                    break;
            }
        }

        public void SetCompleteTutorial(HelperId helperId)
        {
            switch (helperId)
            {
                case HelperId.Magnet:
                    isCompleteMagnetTutorial = true;
                    break;
                case HelperId.FrozenTime:
                    isCompleteFrozenTimeTutorial = true;
                    break;
                case HelperId.ScaleUp:
                    isCompleteScaleUpTutorial = true;
                    break;
                case HelperId.Compass:
                    isCompleteCompassTutorial = true;
                    break;
            }
        }

        public bool IsCompleteTutorial(HelperId helperId)
        {
            switch (helperId)
            {
                case HelperId.None:
                    return false;
                case HelperId.Magnet:
                    return isCompleteMagnetTutorial;
                case HelperId.FrozenTime:
                    return isCompleteFrozenTimeTutorial;
                case HelperId.ScaleUp:
                    return isCompleteScaleUpTutorial;
                case HelperId.Compass:
                    return isCompleteCompassTutorial;
                default:
                    throw new ArgumentOutOfRangeException(nameof(helperId), helperId, null);
            }
        }
        private void ConsumeNumber(ref int number,int value)
        {
            number-= value;
            if (number < 0) number = 0;
        }
    }
}