using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC.SagaMap
{
    public class BaseSagaPathItem : _BaseBehaviour
    {
        public TMP_Text textLevel;
        public Button button;
        public Image imageState;

        public virtual int Level {get; private set; }

        public virtual void SetData(int level)
        {
            Level = level;

            gameObject.name = $"SagaMapItem-{level}";
            textLevel.text = $"{level}";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log($"Click SagaMapItem {level}");
            });
        }

        public virtual void SetLevelState(bool isLock, bool isCurrentPlay)
        {
            if (imageState != null)
            {
                if (isLock)
                    imageState.color = Color.gray;
                else if (isCurrentPlay)
                    imageState.color = Color.yellow;
                else
                    imageState.color = Color.green;
            }
        }

        public virtual float GetJumpOffset()
        {
            var rectTrans = GetComponent<RectTransform>();
            var parent = rectTrans.parent.GetComponent<RectTransform>();
            var parentSize = parent.rect.size;

            var itemOffset = (parentSize.y * 0.5f - rectTrans.anchoredPosition.y) / parentSize.y;
            var cellOffset = itemOffset - 0.5f;
            return cellOffset;
        }
    }

}