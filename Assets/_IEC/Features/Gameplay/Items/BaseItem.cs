using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay.ObserverEventData;
using DG.Tweening;
using IEC;
using UnityEngine;
using _IEC.Script.GamePlay;
using PathCreation.Examples;

namespace _IEC.Features.Gameplay
{
    [RequireComponent(typeof(ConditionalSelectionBase))]
    public class BaseItem : _BaseBehaviour, IManagedFixedUpdate
    {
        [SerializeField] protected Rigidbody rb;
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
        private Camera mainCam;
        public bool IsCollected => _isCollected;

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

        private RigidbodyData savedData;
        private PathFollower _pathFollower;

        public virtual void Start()
        {
            _pathFollower = GetComponentInChildren<PathFollower>();
            if (m_ItemDefinition == null && this.itemDefinition != null)
            {
                m_ItemDefinition = this.itemDefinition;
            }
            mainCam = Camera.main;
            UpdateManager.Instance.RegisterFixedUpdate(this);
        }

        void OnDestroy()
        {
            DOTween.Kill(RecycleTweenId);
            UpdateManager.Instance.UnregisterFixedUpdate(this);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (m_CurrentState == State.Idle && collision.gameObject.layer == LayerMask.NameToLayer("Ground") && !_isCollected)
            {
                rb.isKinematic = true;
            }
        }

        public void EnterHoleTrigger()
        {
            StopAllCoroutines();
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
            if (PlayableController.Instance.isEndGame)
            {
                return;
            }
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
            if(ItemData.itemId == "jellyfish")
            {
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(DelayDisable(1f));
            }
            //DOVirtual.DelayedCall(3, () =>
            //{
            //    if (gameObject != null)
            //    {
            //        gameObject.Recycle();
            //    }
            //}).SetId(RecycleTweenId);
            if (CanPlaySFX)
            {
                EventController.RaiseEvent(EventList.Audio.ITEM_DOWN);
                CanPlaySFX = false;
                DOVirtual.DelayedCall(SoundController.GetLengthSound(EventList.Audio.ITEM_DOWN), () => CanPlaySFX = true);
            }
        }

        IEnumerator DelayDisable(float delay)
        {
            yield return new WaitForSeconds(delay);
            if (gameObject != null)
            {
                Destroy(this.gameObject);
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
                myRenderer = GetComponentInChildren<Renderer>();
            }

            
            var myTransform = transform;
            myTransform.position = itemData.position;
            myTransform.eulerAngles = itemData.eulerAngle;
            myTransform.transform.localScale = itemData.localScale;
            rb.interpolation = RigidbodyInterpolation.Interpolate;
            m_ItemData = itemData;
            m_ItemDefinition = itemDefinition;
            SaveAndDestroyRigidbody();
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
                savedData.collisionDetectionMode = rb.collisionDetectionMode;
#if UNITY_EDITOR
                DestroyImmediate(rb);
                #else
                Destroy(rb);
                #endif
            }
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
                rb.collisionDetectionMode = savedData.collisionDetectionMode;
                rb.interpolation = RigidbodyInterpolation.Interpolate;
            }
        }

        protected void ChangeState(State newState)
        {
            if (newState == m_CurrentState) return;

            if (newState == State.Collectable && _pathFollower)
            {
                _pathFollower.StopMove();
            }

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

            if (m_CurrentState == State.Idle && _pathFollower)
            {
                _pathFollower.StartMoveDelay();
            }

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
            StopAllCoroutines();

            // if (_specialItemHighlightEffect != null)
            // {
            //     _specialItemHighlightEffect.gameObject.SetActive(false);
            // }

        }

        #region New Whirlpool Logic

        public bool onDraging = false;
        public Transform targetObject; // Object B
    
        public void ManagedFixedUpdate()
        {
            bool hasRb = rb;
            
            if (!hasRb)
            {
                if (Time.frameCount % 2 == 0)
                {
                    if (IsInScreen(mainCam))
                    {
                        RestoreRigidbody();
                    }
                }
            }
            else
            {
                if (m_CurrentState != State.Collectable && !rb.isKinematic && !_isCollected)
                {
                    if (this.transform.position.y < PlayableController.Instance.mapHeight)
                    {
                        rb.isKinematic = false;
                        OnCollected(this.transform.position);
                    }
                }
            }
        }
        
        #endregion

        public void DisableMove()
        {
            if (_pathFollower)
            {
                _pathFollower.StopMove();
            }
        }

        protected void SetLayer(int newLayer)
        {
            gameObject.layer = newLayer;
            foreach (var item in itemColliders)
            {
                item.layer = newLayer;
            }
            //itemCollider.gameObject.layer = newLayer;
        }

        public void MoveToTarget(Transform target, float speed)
        {
            StopAllCoroutines();
            StartCoroutine(MoveToXZPosition(target, speed));
        }

        public IEnumerator MoveToXZPosition(Transform targetTransform, float speed)
        {
            rb.isKinematic = true;
            // Only move in X and Z, keep current Y
            Vector3 start = transform.position;
            Vector3 destination = new Vector3(targetTransform.position.x, start.y, targetTransform.position.z);
            while (Vector3.Distance(transform.position, destination) > 0.2f)
            {
                DisableMove();
                transform.position = Vector3.MoveTowards(transform.position, destination, speed * 0.068f);
                yield return null;
                destination = new Vector3(targetTransform.position.x, start.y, targetTransform.position.z);
            }
            rb.isKinematic = false;
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