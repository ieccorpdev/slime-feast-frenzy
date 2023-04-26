using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
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
    }

}