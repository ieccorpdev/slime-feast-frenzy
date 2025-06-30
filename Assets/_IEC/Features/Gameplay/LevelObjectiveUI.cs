using System;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using Sirenix.OdinInspector;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static IEC.EventList;

namespace _IEC.Features.Gameplay
{
    public class LevelObjectiveUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI txtNumber;
        [SerializeField] private float baseWidth;
        [SerializeField] private Image sdfImage;
        [SerializeField] private Image imgCheckComplete;
        [SerializeField] private Image imgIcon;
        [SerializeField] private Transform pivot;
        [SerializeField] private ParticleSystem effectCollect;

        public string itemId;

        private bool m_IsSpecial;
        private RectTransform m_RectTransform;

        private string TWEEN_LEVEL_OBJECTIVE => "TWEEN_LEVEL_OBJECTIVE" + gameObject.GetInstanceID();

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
        
        public Vector3 GetIconPosition()
        {
            return imgIcon.transform.position;
        }

        public bool IsSpecialItem()
        {
            return m_IsSpecial;
        }

        public Vector2 GetIconSize()
        {
            return imgIcon.rectTransform.sizeDelta;
        }

        public void SetNumber(int newNumber, bool isCompleted)
        {
            effectCollect.Play();
            SetText(newNumber.ToString());
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
            return imgIcon.sprite;
        }

        public void LoadData(LevelObjective levelObjective, bool isSpecial)
        {
            itemId = levelObjective.itemId;
            m_IsSpecial = isSpecial;
            //SetText(isSpecial ? "0" : levelObjective.number.ToString());
            SetText(levelObjective.number.ToString());
            if (isSpecial)
            {
    
            }
            else
            {
                imgIcon.sprite = LoadSpriteIconFromId(itemId);
            }
            imgIcon.rectTransform.ForceToWidth(imgIcon.sprite, baseWidth);

            var delta = RectTransform.sizeDelta;
            delta.y = imgIcon.rectTransform.sizeDelta.y;
            RectTransform.sizeDelta = delta;
        }

        public void Reset()
        {
            DOTween.Kill(TWEEN_LEVEL_OBJECTIVE);
            pivot.localScale = Vector3.one;
            pivot.localEulerAngles = Vector3.zero;
            
            imgCheckComplete.gameObject.SetActive(false);
            txtNumber.gameObject.SetActive(true);
        }

        private void PlayDoneAnimation()
        {
            DOCollectedAllObjectsEffect();
            if (!PlayableController.Instance.isEndGame)
                EventController.RaiseEvent(EventList.Audio.OBJECTIVE_CLEAR);
            txtNumber.gameObject.SetActive(false);
            imgCheckComplete.gameObject.SetActive(true);
        }

        [Button]
        private void DOCollectedAllObjectsEffect()
        {
            DOTween.Kill(TWEEN_LEVEL_OBJECTIVE);
            pivot.localScale = Vector3.one;
            pivot.localEulerAngles = Vector3.zero;

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
            var config = PlayableController.Instance.ListItemDefinition.Find(x => x.GetId() == itemId);
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