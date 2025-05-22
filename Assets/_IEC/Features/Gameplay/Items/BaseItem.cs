using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using _IEC.Script.GamePlay;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [RequireComponent(typeof(ConditionalSelectionBase))]
    public class BaseItem : _BaseBehaviour, IManagedLateUpdate
    {
        [SerializeField] private Rigidbody rb;
        [SerializeField] protected List<GameObject> itemColliders;
        [SerializeField] private Renderer myRenderer;
        [SerializeField] private ItemDefinition itemDefinition;

        [SerializeField] private ItemData m_ItemData;
        [SerializeField] private ItemDefinition m_ItemDefinition;
        [SerializeField] private State m_CurrentState;
        [SerializeField] protected bool _isCollected = false;
        public TCP2_GetPosOnWater posOnWater;
        public SpringOscillation springOscillation;
        private int LayerMainWorld => LayerMask.NameToLayer("MainWorld");
        private int LayerHole => LayerMask.NameToLayer("Hole");
        protected string RecycleTweenId => gameObject.GetInstanceID() + "recycle";
        public ItemData ItemData => m_ItemData;
        public Renderer MyRenderer => myRenderer;
        public ItemDefinition ItemDefinition => m_ItemDefinition;
        private Camera mainCam;
        public bool IsCollected => _isCollected;
        private bool hasRenderer;
        private bool hasRigidbody;
        private bool hasSpringOscillation;
        private bool hasPosOnWater;
        private Game GameG;
        public Rigidbody Rb
        {
            get
            {
                if (rb == null)
                {
                    RestoreRigidbody();
                }
                return rb;
            }
        }
        private static bool CanPlaySFX = true;

        private SpecialItemParticleEffectHighlight _specialItemHighlightEffect;
        private RigidbodyData savedData;

        private void Start()
        {
            this.GameG = Pool.Get<Game>();
            if (m_ItemDefinition == null && this.itemDefinition != null)
            {
                m_ItemDefinition = this.itemDefinition;
            }
            mainCam = Camera.main;
            rb.drag = Config.gamePlayConfig.ItemDrag;
            rb.angularDrag = Config.gamePlayConfig.ItemAngularDrag;
            _specialItemHighlightEffect = GetComponentInChildren<SpecialItemParticleEffectHighlight>();
        }
        void OnEnable()
        {
            UpdateManager.Instance.RegisterLateUpdate(this);
        }

        void OnDisable()
        {
            DOTween.Kill(RecycleTweenId);
            UpdateManager.Instance.UnregisterLateUpdate(this);
        }

        [SerializeField] private float minImpactVelocity = 0.2f;
        [SerializeField] private float splashYOffset = 0.15f; // Tùy chỉnh chiều cao splash
        private bool hasSplashed = false;
        private Vector3 lastVelocity;


        private void OnCollisionEnter(Collision collision)
        {
            if (!Game.dropSplashWater) return;
            if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
            {
                if (collision.contactCount == 0) return;
                if (hasSplashed) return;
                float velocityY = rb.velocity.y;
                float relVelocityY = collision.relativeVelocity.y;
                if (velocityY < -minImpactVelocity && relVelocityY > 0)
                {
                    // Splash VFX
                    Vector3 contactPoint = collision.contacts[0].point;
                    contactPoint.y = splashYOffset;

                    var splashVFX = Config.prefabConfig.vfxItemSplash.Spawn(transform);
                    if (splashVFX != null)
                    {
                        splashVFX.transform.position = contactPoint;
                        splashVFX.transform.parent = null;

                        if (myRenderer != null)
                        {
                            Vector3 size = myRenderer.bounds.size;
                            float maxSize = Mathf.Max(size.x, size.z); 
                            splashVFX.transform.localScale = Vector3.one * Mathf.Clamp(maxSize, 0.2f, 2.5f);
                        }
                        splashVFX.Show();
                    }
                    // Spring Oscillation
                    ApplySpringForce(velocityY);
                    hasSplashed = true; 
                }
            } 

        }
        public void ApplySpringForce(float velocityY)
        {
            if (springOscillation != null)
            {
                springOscillation.ApplyForce(velocityY);
            }
        }

        public void EnterHoleTrigger()
        {
            ChangeState(State.Collectable);
            
            if (_specialItemHighlightEffect != null)
            {
                _specialItemHighlightEffect.gameObject.SetActive(false);
            }
        }

        public Sprite GetIcon()
        {
            return m_ItemDefinition.GetIcon();
        }

        public string GetItemId()
        {
            return m_ItemDefinition.GetId();
        }
        public void ExitHoleTrigger()
        {
            ChangeState(State.Idle);
            
            if (_specialItemHighlightEffect != null)
            {
                _specialItemHighlightEffect.gameObject.SetActive(true);
            }
        }

        public bool IsInScreen(Camera particularCam)
        {
            if (myRenderer == null) return false;
            if (particularCam == null) return false;
            return myRenderer.IsInClipView(particularCam);
        }
        public virtual void OnCollected(Vector3 position)
        {
            this.RaiseEvent(EventList.GamePlay.COLLECT_ITEM,new CollectItemData()
            {
                itemID = m_ItemData.itemId,
                position = position,
                xp = m_ItemDefinition.GetXp()
            });
            rb.velocity = -10 * Vector3.up;
            DisableAllCollider();
            _isCollected = true;
            //gameObject.Recycle();
            StartCoroutine(DelayDisable(3));

            //DOVirtual.DelayedCall(3, () =>
            //{
            //    if (gameObject != null)
            //    {
            //        gameObject.Recycle();
            //    }
            //}).SetId(RecycleTweenId);
            if (!GamePlay.IsWin)
            {
                Locator.Game.ScreenInGame.HandleEatCombo();
                if (CanPlaySFX)
                {
                    EventController.RaiseEvent(EventList.Audio.ITEM_DOWN);
                    CanPlaySFX = false;
                    DOVirtual.DelayedCall(SoundController.GetLengthSound(EventList.Audio.ITEM_DOWN), () => CanPlaySFX = true);
                }
            }
        }

        IEnumerator DelayDisable(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameObject != null)
            {
                gameObject.SetActive(false);
            }
        }

        //TODO: cache colliders
        protected virtual void DisableAllCollider()
        {
            var colliders = GetComponentsInChildren<Collider>();
            foreach (var col in colliders)
            {
                col.enabled = false;
            }
        }
        
        public State GetCurrentState()
        {
            return m_CurrentState;
        }

        public virtual void LoadData(ItemData itemData,ItemDefinition itemDefinition)
        {
            if (rb == null)
            {
                rb = GetComponent<Rigidbody>();
            }

            if (itemColliders == null || itemColliders.Count == 0)
            {
                itemColliders = GetComponentsInChildren<Transform>()
                    .Where(x=>x.CompareTag("FallingItem_Physics") || x.GetComponentInChildren<Collider>() != null)
                    .Select(x =>
                    {
                        x.tag = "FallingItem_Physics";
                        return x.gameObject;
                    }).ToList();
            }
            else
            {
                //TODO: cache colliders
                foreach (var col in itemColliders)
                {
                    var collider = col.GetComponent<Collider>();
                    if (collider != null)
                    {
                        collider.enabled = true;
                    }
                }
            }

            _isCollected = false;
            
            if (myRenderer == null)
            {
                myRenderer = GetComponentInChildren<MeshRenderer>();
                hasRenderer = true;
            }

            if (springOscillation == null)
            {
                springOscillation = GetComponentInChildren<SpringOscillation>();
                hasSpringOscillation = true;
                if (springOscillation != null)
                {
                    springOscillation.SetSizeDependentLimits(myRenderer.bounds.size);
                }
            }
            var myTransform = transform;
            myTransform.position = itemData.position;
            myTransform.eulerAngles = itemData.eulerAngle;
            myTransform.transform.localScale = itemData.localScale;
            if (itemData.isStartKinematic)
            {
                SaveAndDestroyRigidbody();
            }
            m_ItemData = itemData;
            m_ItemDefinition = itemDefinition;
        }

        private void SaveAndDestroyRigidbody()
        {
            if (rb != null)
            {
                savedData.velocity = rb.velocity;
                savedData.angularVelocity = rb.angularVelocity;
                savedData.mass = rb.mass;
                savedData.drag = rb.drag;
                savedData.angularDrag = rb.angularDrag;
                savedData.useGravity = rb.useGravity;
                savedData.isKinematic = true;
                savedData.interpolation = rb.interpolation;
                savedData.collisionDetectionMode = rb.collisionDetectionMode;
                Destroy(rb);
            }

            hasRigidbody = false;
        }

        private void RestoreRigidbody()
        {
            if (rb == null)
            {
                rb = gameObject.AddComponent<Rigidbody>();
                rb.velocity = savedData.velocity;
                rb.angularVelocity = savedData.angularVelocity;
                rb.mass = savedData.mass;
                rb.drag = savedData.drag;
                rb.angularDrag = savedData.angularDrag;
                rb.useGravity = savedData.useGravity;
                rb.isKinematic = savedData.isKinematic;
                rb.interpolation = savedData.interpolation;
                rb.collisionDetectionMode = savedData.collisionDetectionMode;
            }

            hasRigidbody = true;
        }

        protected void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.Idle:
                    break;
                case State.Collectable:
                    SetLayer(LayerMainWorld);
                    break;
                case State.Falling:
                    break;
            }

            m_CurrentState = newState;
            switch (m_CurrentState)
            {
                case State.None:
                    break;
                case State.Idle:
                    break;
                case State.Collectable:
                    SetLayer(LayerHole);
                    OnBecomeCollectable();
                    break;
                case State.Falling:
                    break;
            }
        }

        protected virtual void OnBecomeCollectable()
        {
            if (Game.usingNewLogicPull)
            {
                onDraging = true;
                targetObject = GamePlay.levelController.GetHolePlayer().transform;
            }
            else
            {
                RestoreRigidbody();
                rb.isKinematic = false;
            }
            if (posOnWater != null)
            {
                posOnWater.enabled = false;
            }
            // if (_specialItemHighlightEffect != null)
            // {
            //     _specialItemHighlightEffect.gameObject.SetActive(false);
            // }

        }

        #region New Whirlpool Logic

        public bool onDraging = false;
        public Transform targetObject; // Object B
        private static Plane[] sharedPlanes;
        private Plane[] curentPlanes;
    
        public void ManagedLateUpdate()
        {
            //if (hasRigidbody)
            //{
            //    //temporary keep, ugly if remove
            //    lastVelocity = rb.velocity;
            //}
            if (hasSpringOscillation)
            {
                springOscillation.CheckSpringObject();
            }
            if (GameG.objectBobbingWater && hasPosOnWater)
            {
                if (rb && posOnWater.enabled)
                {
                    posOnWater.enabled = false;
                }
                else
                {
                    posOnWater.CheckPosWater();
                }
            }
            if (!hasRigidbody)
            {
               if (curentPlanes == null || ((sharedPlanes == null || Time.frameCount % 5 == 0) && curentPlanes != sharedPlanes))
               {
                   sharedPlanes = GeometryUtility.CalculateFrustumPlanes(Camera.main);
                   curentPlanes = sharedPlanes;
               }
               if (hasRenderer && GeometryUtility.TestPlanesAABB(sharedPlanes, myRenderer.bounds))
               {
                   RestoreRigidbody();
                   AppearInScreen();
               }
            }
            //MeoMew disable, rb.velocity is heavy when call 1k object
            else if (m_CurrentState != State.Collectable && rb != null && rb.velocity != Vector3.zero)
            {
                rb.velocity = Vector3.Lerp(rb.velocity, Vector3.zero, Time.deltaTime * 6f);
            }
        }

        public void SetPosOnwater(TCP2_GetPosOnWater pow)
        {
            posOnWater = pow;
            hasPosOnWater = true;
        }
        protected virtual void AppearInScreen() { }
        
        #endregion

        protected void SetLayer(int newLayer)
        {
            gameObject.layer = newLayer;
            foreach (var item in itemColliders)
            {
                item.layer = newLayer;
            }
            //itemCollider.gameObject.layer = newLayer;
        }


        public enum State
        {
            None,
            Idle,
            Collectable,
            Falling
        }

        [System.Serializable]
        public struct RigidbodyData
        {
            public Vector3 velocity;
            public Vector3 angularVelocity;
            public float mass;
            public float drag;
            public float angularDrag;
            public bool useGravity;
            public bool isKinematic;
            public RigidbodyInterpolation interpolation;
            public CollisionDetectionMode collisionDetectionMode;
        }
    }
}