using _IEC.Features.Common.Extension;
using IEC;
using UnityEngine;
using UnityEngine.UI;

namespace _IEC.Features.Gameplay
{
    public class BaseItemDirectorUI : MonoBehaviour
    {
        [SerializeField] private GameObject groupActive;
        [SerializeField] private Image imgIcon;
        [SerializeField] private float baseIconWidth;
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private RectTransform parentRectTransform;
        [SerializeField] private float heightRectMap;
        [SerializeField] private float angleOffset;
        [SerializeField] private Image imgPointer;
        private BaseItem m_BaseItem;
        private Transform m_HoleTransform;
        private State m_CurrentState;
        private float m_ScreenWidth;
        

        public void LoadData(Transform holeTransform, BaseItem item)
        {
            m_ScreenWidth = Screen.width;
            imgIcon.sprite = item.GetIcon();
            imgIcon.rectTransform.ForceToWidth(imgIcon.sprite,baseIconWidth);
            m_HoleTransform = holeTransform;
            parentRectTransform = transform.parent.GetComponent<RectTransform>();
            m_BaseItem = item;
        }

        public string GetID()
        {
            return m_BaseItem.GetItemId();
        }

        public bool IsNeededDirector()
        {
            return m_BaseItem != null && m_BaseItem.gameObject.activeInHierarchy && !m_BaseItem.IsCollected;
        }

        public void UpdateDirectToBaseItem()
        {
            if (m_BaseItem == null || m_BaseItem.gameObject.activeInHierarchy == false)
            {
                gameObject.SetActive(false);
                return;
            }
   
            UpdateCurrentState();
        }
        public void Reset()
        {
            m_CurrentState = State.None;
            m_BaseItem = null;
            transform.localScale = Vector3.one;
        }
        
        private void UpdateCurrentState()
        {
           
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.OutScreen:
                    var dir = m_BaseItem.transform.position - m_HoleTransform.transform.position;
                    var angle = Vector3.SignedAngle(Vector3.right * dir.x + Vector3.forward * dir.z,Vector3.right,Vector3.up);
                    transform.localEulerAngles = Vector3.forward * (angle - 90 + angleOffset);
                    //Debug.Log("Angle: " + angle);
                    if (dir.x > 0)
                    {
                        SnapRight();
                    }
                    else
                    {
                        SnapLeft();
                    }
                    UpdateAnchorPositionY(dir);
                    break;
                case State.InScreen:
         
                    imgPointer.gameObject.SetActive(m_BaseItem.GetCurrentState() == BaseItem.State.Idle || m_BaseItem.GetCurrentState() == BaseItem.State.None);
                    break;
            }
        }

        private void UpdateAnchorPositionY(Vector3 dir)
        {
            var y = dir.z / heightRectMap;
            if (y > 1) y = 1;
            if (y < -1) y = -1;
            y = y * parentRectTransform.sizeDelta.y/2;
            var currentAnchorPos = rectTransform.anchoredPosition;
            currentAnchorPos.y = y;
            rectTransform.anchoredPosition = currentAnchorPos;
        }

        private void SnapRight()
        {
            var currentAnchorPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(m_ScreenWidth/2 - rectTransform.sizeDelta.x/2 - 50f, currentAnchorPosition.y);
        }

        private void SnapLeft()
        {
            var currentAnchorPosition = rectTransform.anchoredPosition;
            rectTransform.anchoredPosition = new Vector2(-m_ScreenWidth/2 + rectTransform.sizeDelta.x/2 + 50f, currentAnchorPosition.y);
        }
        
        private void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            m_CurrentState = newState;
            groupActive.SetActive(newState == State.OutScreen);
            imgPointer.gameObject.SetActive(newState == State.InScreen);
        }
        
        private enum  State
        {
            None,
            OutScreen,
            InScreen
        }
    }
}