using DG.Tweening;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace IEC
{
#if UNITY_EDITOR
    public class AddButtonClickEffectEditor : EditorWindow
    {
        [MenuItem("Tools/Add ButtonClickEffect to All Buttons")]
        public static void AddEffectToAllButtons()
        {
            int addedCount = 0;
            int removedAnimators = 0;

            Button[] allButtons = GameObject.FindObjectsOfType<Button>(true); // include inactive

            foreach (Button button in allButtons)
            {
                if (button == null || button.gameObject == null)
                    continue;

                GameObject go = button.gameObject;

                // Record undo before making any changes
                Undo.RegisterCompleteObjectUndo(go, "Modify Button");

                // ✅ Remove Animator or Animation if exists
                Animator animator = go.GetComponent<Animator>();
                if (animator != null)
                {
                    Object.DestroyImmediate(animator, true);
                    removedAnimators++;
                }

                Animation animation = go.GetComponent<Animation>();
                if (animation != null)
                {
                    Object.DestroyImmediate(animation, true);
                    removedAnimators++;
                }
                var duplicates = go.GetComponents<ButtonClickEffect>();
                if (duplicates.Length > 1)
                {
                    for (int i = 1; i < duplicates.Length; i++)
                    {
                        DestroyImmediate(duplicates[i]);
                    }
                }

                if (go.GetComponent<ButtonClickEffect>() == null)
                {
                    go.AddComponent<ButtonClickEffect>();
                    addedCount++;
                }

                EditorUtility.SetDirty(go);
                EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
            }

            Debug.Log($"✅ Added ButtonClickEffect to {addedCount} button(s), removed {removedAnimators} Animator/Animation component(s).");
        }
    }
#endif

    [DisallowMultipleComponent]
    public class ButtonClickEffect : _BaseBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        public GameObject buttonDisplayImage;
        private Image btnDisplayImage => buttonDisplayImage.GetComponent<Image>();
        [Header("Button Sprite")] 
        public bool setNative = true;
        public bool isChangeButtonSpriteOnClick = false;
        public Sprite pressSprite;
        public Sprite releaseSprite;
        public bool isChangeButtonSpriteOnInactive = false;
        public Sprite activeSprite;
        public Sprite inactiveSprite;
        public GameObject activeGroup, inactiveGroup;
        [Header("Button scale")] 
        private bool isScaleButtonOnClick = true;
        [Range(0,1)]private float buttonScaleOnPressDown = 0.96f;
        
        // [Header("Button sound")] 
        // public bool isPlayDefaultSoundOnClick = true;
        
        private Button m_Button;
        private Vector3 baseScale;
        public ButtonState ButtonState { get; private set; } = ButtonState.Active;
        
        private void Start()
        {
            m_Button = GetComponent<Button>();
            baseScale = buttonDisplayImage == null ? gameObject.transform.localScale : buttonDisplayImage.gameObject.transform.localScale;
            if (m_Button == null)
            {
                DestroyImmediate(this);
            }
            else
            {
                m_Button.transition = Selectable.Transition.None;
            }
        }

        private void OnDisable()
        {
            ChangeSprite(false);
        }

        public void SetState(ButtonState state)
        {
            ButtonState = state;
            
            if(activeGroup != null)
                activeGroup.SetActive(state == ButtonState.Active);
            
            if(inactiveGroup != null)
                inactiveGroup.SetActive(state == ButtonState.Inactive);
            
            ChangeSpriteOnInactive(ButtonState == ButtonState.Inactive);
        }

        private void ChangeSprite(bool isPress)
        {
            if (!isChangeButtonSpriteOnClick)
                return;

            if (ButtonState == ButtonState.Inactive)
                return;
            if (pressSprite == null || releaseSprite == null || btnDisplayImage == null)
                return;
            if (isPress)
                btnDisplayImage.sprite = pressSprite;
            else
                btnDisplayImage.sprite = releaseSprite;
            
            if(setNative) btnDisplayImage.SetNativeSize();
        }

        private void ChangeSpriteOnInactive(bool isInactive)
        {
            if (!isChangeButtonSpriteOnInactive)
                return;
            if (pressSprite == null || releaseSprite == null)
                return;
            if (isInactive)
                btnDisplayImage.sprite = inactiveSprite;
            else
                btnDisplayImage.sprite = activeSprite;
            
            //if(setNative) buttonDisplayImage.SetNativeSize();
        }

        private void ChangeScale(bool isPress)
        {
            if (!isScaleButtonOnClick)
                return;
            
            if (m_Button ==null || !m_Button.interactable || ButtonState == ButtonState.Inactive)
                return;
            GameObject go = buttonDisplayImage == null ? gameObject : buttonDisplayImage.gameObject;
            if (isPress)
                go.transform.DOScale(baseScale * buttonScaleOnPressDown, 0.1f).SetEase(Ease.InQuad);
            else
                go.transform.DOScale(baseScale, 0.1f).SetEase(Ease.OutQuad);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            switch (ButtonState)
            {
                case ButtonState.Active:
                    EventController.RaiseEvent(EventList.Audio.ACTIVE_BUTTON_CLICK);
                    break;
                case ButtonState.Inactive:
                    EventController.RaiseEvent(EventList.Audio.INACTIVE_BUTTON_CLICK);
                    break;
            }
            
            ChangeSprite(true);
            ChangeScale(true);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            ChangeSprite(false);
            ChangeScale(false);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            ChangeSprite(false);
        }
    }

    public enum ButtonState
    {
        Active,
        Inactive
    }
    
}
