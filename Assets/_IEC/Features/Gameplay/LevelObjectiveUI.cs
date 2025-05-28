using System;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using nickeltin.SDF.Runtime;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace _IEC.Features.Gameplay
{
    public class LevelObjectiveUI : _BaseBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private float baseWidth;
        [SerializeField] private SDFImage sdfImage;
        [SerializeField] private Image imgCheckComplete;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Transform pivot;
        [SerializeField] private ParticleSystem effectCollect;

        public string itemId;
        
        private bool m_IsSpecial;
        private bool m_isLoaded;
        public bool IsLoaded => m_isLoaded;
        private RectTransform m_RectTransform;

        private string TWEEN_LEVEL_OBJECTIVE => "TWEEN_LEVEL_OBJECTIVE" + gameObject.GetInstanceID();
        //anim fly up
        private float offsetFlyX;
        private int dirFlyX;
        public bool isGoldObj => itemId == Config.specialObjectConfig.goldSpecialObjective.GetId();
        public bool isKingdomObj => itemId == Config.specialObjectConfig.kingdomSpecialObjective.GetId();
        [SerializeField] private Animator anim;
        [SerializeField] private Transform subIconPos;
        [SerializeField] private Image fillFuelBar;
        [SerializeField] private TMP_Text passTockenCounter;

        public RectTransform RectTransform
        {
            get
            {
                if (m_RectTransform == null)
                {
                    m_RectTransform = GetComponent<RectTransform>();
                }

                return m_RectTransform;
            }
        }

        public void PlayAnim()
        {
            var stateInfo = anim.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("Show") && stateInfo.normalizedTime < 1f)
            {
                anim.Play("Show", 0, 0.2f);
            }
            else
            {
                anim.Play("Show");
            }
        }


        public Vector3 GetIconPosition()
        {
            return subIconPos ? subIconPos.position : imgIcon.transform.position;
        }

        public float GetOffsetItemCurve()
        {
            offsetFlyX += 0.1f * dirFlyX;
            if (offsetFlyX is < -0.5f or > 0.5f)
            {
                dirFlyX *= -1;
                offsetFlyX = Mathf.Clamp(offsetFlyX, -0.5f, 0.5f);
            }
            return offsetFlyX;
        }

        public bool IsSpecialItem()
        {
            return m_IsSpecial;
        }

        public Vector2 GetIconSize()
        {
            return imgIcon.rectTransform.sizeDelta;
        }

        public void SetNumber(string newNumber, bool isCompleted)
        {
            if (isKingdomObj)
            {
                SetProgressFuel();
            }
            else
            {
                SetText(newNumber);
            }
            effectCollect.Play();
            if (isCompleted)
            {
                PlayDoneAnimation();
            }
            else
            {
                DOCollectItemEffect();
            }
        }
        
        public Sprite GetSprite()
        {
            if (isKingdomObj)
                return Config.specialObjectConfig.kingdomSpecialObjective.GetIcon();
            return imgIcon.sprite;
        }

        void SetProgressFuel()
        {
            int currentFuel = PlayerData.journeyData.currentFuelJourney;
            int targetFuel = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).fuelNeed;
            while (currentFuel >= targetFuel)
            {
                PlayerData.journeyData.currentFuelJourney -= targetFuel;
                currentFuel = PlayerData.journeyData.currentFuelJourney;
                PlayerData.journeyData.currentTargetJourney++;
                targetFuel = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).fuelNeed;
            }
            if (PlayerData.journeyData.currentTargetJourney >= Config.journeyTripConfig.listRewardsByJourneys.Count)
            {
                fillFuelBar.DOFillAmount(1, 0.5f);
                SetText(targetFuel + "/" + targetFuel);
                passTockenCounter.text = Config.journeyTripConfig.GetRewardsByLevelJourney(PlayerData.journeyData.currentTargetJourney).idJourney.ToString();
            }
            else
            {
                fillFuelBar.DOFillAmount((float)currentFuel / (float)targetFuel, 0.5f);
                SetText(currentFuel + "/" + targetFuel);
                passTockenCounter.text = PlayerData.journeyData.currentTargetJourney.ToString();
            }
        }

        public void LoadData(LevelObjective levelObjective, bool isSpecial)
        {
            Reset();
            itemId = levelObjective.itemId;
            m_IsSpecial = isSpecial;
            //SetText(isSpecial ? "0" : levelObjective.number.ToString());
            if (isSpecial)
            {
                if (isGoldObj) // MeoMew hard Code;
                {
                    SetText(GamePlay.levelController.GoldCollected.ToString());
                }
                else if (isKingdomObj) // FireFire hard Code;
                {
                    SetProgressFuel();
                }
                else
                {
                    int maxSpecialObject = GamePlay.levelController.AllSpecialItems.Count;
                    var totalNumberCollected = GamePlay.levelController.SpecialProgressRewardObjCollected;
                    SetText(totalNumberCollected+"/"+maxSpecialObject);
                }

            }
            else
            {
                SetText(levelObjective.number.ToString());  
            }
            if (isSpecial)
            {
                //Change method load 
                if (isGoldObj)// MeoMew hard Code;
                {
                    var itemDefinition = Config.specialObjectConfig.goldSpecialObjective;
                    imgIcon.sprite = itemDefinition.GetIcon();
                }
                else if (isKingdomObj) // FireFire hard Code;
                {
                    var itemDefinition = Config.specialObjectConfig.kingdomSpecialObjective;
                    //imgIcon.sprite = itemDefinition.GetIcon();
                }
                else
                {
                    var itemDefinition = Config.specialObjectConfig.listSpecialItemDefinition.Find(x => x.GetId() == levelObjective.itemId);
                    imgIcon.sprite = itemDefinition.GetIcon();
                }

            }
            else
            {
                imgIcon.sprite = LoadSpriteIconFromId(itemId);
            }
            //imgIcon.rectTransform.ForceToWidth(imgIcon.sprite, baseWidth);
            if (!isKingdomObj)
            {
                imgIcon.rectTransform.sizeDelta = new Vector2(100, 100);
                var delta = RectTransform.sizeDelta;
                delta.y = imgIcon.rectTransform.sizeDelta.y;
                RectTransform.sizeDelta = delta;
            }
            offsetFlyX = UnityEngine.Random.Range(-0.5f, 0.5f);
            dirFlyX = Random.value < 0.5 ? 1 : -1;
            m_isLoaded = true;
        }

        public void Reset()
        {
            DOTween.Kill(TWEEN_LEVEL_OBJECTIVE);
            pivot.localScale = Vector3.one;
            pivot.localEulerAngles = Vector3.zero;
            if (imgCheckComplete)
                imgCheckComplete.gameObject.SetActive(false);
            txtNumber.gameObject.SetActive(true);
            m_isLoaded = false;
        }

        private void PlayDoneAnimation()
        {
            DOCollectedAllObjectsEffect();
            if (!GamePlay.IsWin)
                EventController.RaiseEvent(EventList.Audio.OBJECTIVE_CLEAR);
            txtNumber.gameObject.SetActive(false);
            if (imgCheckComplete)
                imgCheckComplete.gameObject.SetActive(true);
        }

        [Button]
        private void DOCollectedAllObjectsEffect()
        {
            DOTween.Kill(TWEEN_LEVEL_OBJECTIVE);
            pivot.localScale = Vector3.one;
            pivot.localEulerAngles = Vector3.zero;
            txtNumber.gameObject.SetActive(false);
            var sequence = DOTween.Sequence().SetId(TWEEN_LEVEL_OBJECTIVE);
            sequence.Append(pivot.DOScale(new Vector3(1.2f, 1.2f, 1.2f), 0.35f).SetEase(Ease.OutQuad));
            sequence.Join(pivot.DOPunchRotation(new Vector3(0f, 0f, 25f), 0.7f, 6, elasticity: 0.85f).SetEase(Ease.OutQuad));
            sequence.Append(pivot.DORotate(new Vector3(0f, 900f, 0f), 0.7f, mode: RotateMode.FastBeyond360));
            sequence.Append(pivot.DOScale(Vector3.zero, 0.35f).SetEase(Ease.InQuad));
            sequence.AppendCallback(OnCompleteObjectiveUIAnim);
        }

        void OnCompleteObjectiveUIAnim()
        {
            gameObject.SetActive(false);
        }
        
        private static Sprite LoadSpriteIconFromId(string itemId)
        {
            //TODO: AAA
            if (itemId == "coin")
            {
                itemId = "coin_small";
            }
            var config = Config.itemPrefabConfig.GetListItemDefinition().Find(x => x.GetId() == itemId);
            if (config != null)
            {
                if (config.GetIcon() == null)
                {
                    Debug.LogError(config.GetId());
                }

                return config.GetIcon();
            }


            if (itemId.Contains("SpecialItem"))
            {
                //var itemIdInString = itemId.Replace("SpecialItem_", "");
                //Enum.TryParse<global::SpecialItemID>(itemIdInString, out var specialItemID);
                //var itemDefinition = Config.treasureHuntProgressConfig.GetSpecialItemDefinition(specialItemID);
                //if (itemDefinition != null)
                //{
                    //return itemDefinition.sprite;
                //}
            }

            return null;
        }

        private void DOCollectItemEffect()
        {
            DOTween.Kill(TWEEN_LEVEL_OBJECTIVE);
            pivot.localScale = Vector3.one;
            var sequence = DOTween.Sequence().SetId(TWEEN_LEVEL_OBJECTIVE);
            sequence.Append(pivot.DOPunchScale(new Vector3(0.25f, 0.25f, 0.25f), 0.35f, 1, elasticity: 1)).SetEase(Ease.OutQuad);
        }

        private void SetText(string text)
        {
            txtNumber.text = text;
        }
    }
}