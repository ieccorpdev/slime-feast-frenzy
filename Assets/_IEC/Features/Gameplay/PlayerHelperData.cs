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
            switch (helperId)
            {
                case HelperId.Magnet:
                    return numberMagnet;
                case HelperId.FrozenTime:
                    return numberFrozenTime;
                case HelperId.ScaleUp:
                    return numberScaleUp;
                case HelperId.Compass:
                    return numberCompass;
                default:
                    return 0;
            }
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

        public void ConsumeHelper(HelperId helperId)
        {
            switch (helperId)
            {
                case HelperId.Magnet:
                    ConsumeNumber(ref numberMagnet);
                    break;
                case HelperId.FrozenTime:
                    ConsumeNumber(ref numberFrozenTime);
                    break;
                case HelperId.ScaleUp:
                    ConsumeNumber(ref numberScaleUp);
                    break;
                case HelperId.Compass:
                    ConsumeNumber(ref numberCompass);
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
        private void ConsumeNumber(ref int number)
        {
            number--;
            if (number < 0) number = 0;
        }
    }
}