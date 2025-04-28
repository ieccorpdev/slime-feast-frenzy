using System;
using System.Collections.Generic;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    [RequireComponent(typeof(ConditionalSelectionBase))]
    public class BaseItem : _BaseBehaviour
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
        private int LayerMainWorld => LayerMask.NameToLayer("MainWorld");
        private int LayerHole => LayerMask.NameToLayer("Hole");
        protected string RecycleTweenId => gameObject.GetInstanceID() + "recycle";
        public ItemData ItemData => m_ItemData;
        public Renderer MyRenderer => myRenderer;
        public ItemDefinition ItemDefinition => m_ItemDefinition;

        public bool IsCollected => _isCollected;
        public Rigidbody Rb => rb;
        private static bool CanPlaySFX = true;

        private void Start()
        {
            if (m_ItemDefinition == null && this.itemDefinition != null)
            {
                m_ItemDefinition = this.itemDefinition;
            }
        }

        public void EnterHoleTrigger()
        {
            ChangeState(State.Collectable);
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
            DOVirtual.DelayedCall(3, () =>
            {
                if (gameObject != null)
                {
                    gameObject.Recycle();
                }
            }).SetId(RecycleTweenId);
            if (!GamePlay.IsWin)
            {
                if (CanPlaySFX)
                {
                    EventController.RaiseEvent(EventList.Audio.ITEM_DOWN);
                    CanPlaySFX = false;
                    DOVirtual.DelayedCall(0.6f, () => CanPlaySFX = true);
                }
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
            }
            var myTransform = transform;
            myTransform.position = itemData.position;
            myTransform.eulerAngles = itemData.eulerAngle;
            myTransform.transform.localScale = itemData.localScale;
            rb.isKinematic = itemData.isStartKinematic;
            m_ItemData = itemData;
            m_ItemDefinition = itemDefinition;
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
                rb.isKinematic = false;
            }
            if (posOnWater != null)
            {
                posOnWater.enabled = false;
            }
        }

        #region New Whirlpool Logic
        
        public bool onDraging = false;
        public Transform targetObject; // Object B
        public float pullSpeed = 2.5f;   // Speed towards object B
        public float stopPullDistance = 0.1f;
        public float gravityStrength = -50f; // Simulated gravity
        private Vector3 velocity;
        private bool shouldPull = true;
        public float rotationSpeedMin = 0f; // Min rotation speed (degrees per second)
        public float rotationSpeedMax = 360f; // Max rotation speed (degrees per second)

        void Update()
        {
            if (targetObject == null || !onDraging) return;
            // Distance check
            float distance = Vector3.Distance(transform.position, targetObject.position);
            if (distance <= stopPullDistance)
                shouldPull = false;

            Vector3 move = Vector3.zero;

            // Horizontal pull
            if (shouldPull)
            {
                // Calculate direction to Object B in world space
                Vector3 directionToTarget = targetObject.position - transform.position;
                directionToTarget.y = 0; // Ignore vertical for horizontal pull
                directionToTarget.Normalize(); // Normalize the direction vector

                // Apply pull speed towards Object B
                move += directionToTarget * pullSpeed * Time.deltaTime;
            }

            // Gravity
            velocity.y += gravityStrength * Time.deltaTime;
            move += velocity * Time.deltaTime;

            // Apply movement to object
            transform.position += move;

            // Random rotation in all directions
            if (shouldPull)
            {
                // Generate random rotation vector
                Vector3 randomRotation = new Vector3(
                    UnityEngine.Random.Range(rotationSpeedMin, rotationSpeedMax),
                    UnityEngine.Random.Range(rotationSpeedMin, rotationSpeedMax),
                    UnityEngine.Random.Range(rotationSpeedMin, rotationSpeedMax)
                );

                // Apply random rotation to the object
                transform.Rotate(randomRotation * Time.deltaTime);
            }
        }

        // Optional: Reset pull if needed
        public void ResetPull()
        {
            shouldPull = true;
        }

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

        protected virtual void OnDisable()
        {
            DOTween.Kill(RecycleTweenId);
        }

        public enum State
        {
            None,
            Idle,
            Collectable,
            Falling
        }
    }
}