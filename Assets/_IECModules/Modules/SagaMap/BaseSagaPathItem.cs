using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace IEC
{
    public class BaseSagaPathItem : _BaseBehaviour
    {
        public TMP_Text textLevel;
        public Button button;

        public int level;

        public virtual void SetData(int level)
        {
            this.level = level;

            gameObject.name = $"SagaMapItem-{level}";
            textLevel.text = $"{level}";

            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() =>
            {
                Debug.Log($"Click SagaMapItem {level}");
            });
        }
    }

}