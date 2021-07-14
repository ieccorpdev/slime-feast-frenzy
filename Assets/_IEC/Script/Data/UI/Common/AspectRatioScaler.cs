using UnityEngine.EventSystems;

namespace UnityEngine.UI
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public class AspectRatioScaler : UIBehaviour, ILayoutSelfController
    {
        public enum AspectMode
        {
            None,
            FitInParent
        }

        [SerializeField] private AspectMode m_AspectMode = AspectMode.None;

        [SerializeField] private Vector2 m_ParentOriginSize;

        [SerializeField] private bool m_IsScaleScrollGrid = false;

        [SerializeField] private Vector2 m_CellSizeOrigin;
        [SerializeField] private Vector2 m_CellSpaceOrigin;

        [System.NonSerialized] private RectTransform m_Rect;

        // This "delayed" mechanism is required for cases 713684 and 988706.
        private bool m_DelayedSetDirty = false;

        Vector2 m_LastParentSize = Vector2.zero;

        private RectTransform rectTransform
        {
            get
            {
                if (m_Rect == null)
                    m_Rect = GetComponent<RectTransform>();
                return m_Rect;
            }
        }

        private DrivenRectTransformTracker m_Tracker;

        protected AspectRatioScaler()
        {
        }

        public void Copy(AspectRatioScaler other)
        {
            m_AspectMode = other.m_AspectMode;
            m_ParentOriginSize = other.m_ParentOriginSize;
        }

        public void ResetParentOriginSize(Vector2 size)
        {
            m_ParentOriginSize = size;
        }

        public Vector2 GetParentOriginSize()
        {
            return m_ParentOriginSize;
        }

        protected override void OnEnable()
        {
            base.OnEnable();
            m_DelayedSetDirty = true;
        }

        protected override void OnDisable()
        {
            m_Tracker.Clear();
            LayoutRebuilder.MarkLayoutForRebuild(rectTransform);
            base.OnDisable();
        }

        protected virtual void Update()
        {
            if (m_DelayedSetDirty)
            {
                m_DelayedSetDirty = false;
                SetDirty();
            }

            if (m_LastParentSize != GetParentSize())
            {
                UpdateRect();
            }
        }

        protected override void OnRectTransformDimensionsChange()
        {
        }

        public void UpdateRect()
        {
            if (!IsActive())
                return;

            m_Tracker.Clear();

            switch (m_AspectMode)
            {
#if UNITY_EDITOR
                case AspectMode.None:
                {
                    m_ParentOriginSize = GetParentSize();
                    break;
                }
#endif
                case AspectMode.FitInParent:
                {
                    m_Tracker.Add(this, rectTransform,
                        DrivenTransformProperties.Scale);

                    Vector2 parentSize = GetParentSize();

                    var scale = Vector3.one;
                    scale.x = parentSize.x / m_ParentOriginSize.x;
                    scale.y = parentSize.y / m_ParentOriginSize.y;
                    var min = Mathf.Min(scale.x, scale.y);
                    scale.x = min;
                    scale.y = min;
                    rectTransform.localScale = Vector3.one;
                    m_LastParentSize = parentSize;

                    if (m_IsScaleScrollGrid)
                    {
                        var scroll = gameObject.GetComponent<GridLayoutGroup>();
                        scroll.cellSize = m_CellSizeOrigin * min;
                        scroll.spacing = m_CellSpaceOrigin * min;
                    }
                    else
                    {
                        rectTransform.localScale = scale;
                    }

                    break;
                }
            }
        }

        private Vector2 GetParentSize()
        {
            RectTransform parent = rectTransform.parent as RectTransform;
            if (!parent)
                return Vector2.zero;
            return parent.rect.size;
        }

        public virtual void SetLayoutHorizontal()
        {
        }

        public virtual void SetLayoutVertical()
        {
        }

        protected void SetDirty()
        {
            UpdateRect();
        }

#if UNITY_EDITOR
        protected override void OnValidate()
        {
            SetDirty();
        }

#endif
    }
}