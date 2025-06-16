using System.Collections;
using UnityEngine;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using _IEC.Features.Gameplay;
using UnityEngine.SceneManagement;
using Sirenix.OdinInspector;

namespace IEC
{
    public class GroundChecker : _BaseBehaviour
    {
        [SerializeField] private List<GameObject> invalidObjects;
        [SerializeField] private Transform itemHolder;
        [SerializeField] private LayerMask groundValidatorLayer;
        
        private const float HORIZONTAL_OFFSET_DECREASE = 0.85f;
        private const float SPHERE_RADIUS = 0.1f;
        private int _cachedLevelBeforeDebug;
        private bool _isBulkChecking = false;
        EventYieldWait _currentYieldWait = new EventYieldWait();

        void Awake()
        {
            Pool.Set(this);
            this.RegisterListener(EventList.GamePlay.ON_LEVEL_DEBUG_LOADED, OnLevelDebugLoaded);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.ON_LEVEL_DEBUG_LOADED, OnLevelDebugLoaded);
        }

        void OnLevelDebugLoaded(object param)
        {
            if(!_isBulkChecking) return;
            if (_currentYieldWait != null)
            {
                _currentYieldWait.isKeepWaiting = false;
            }
        }
        
        [Button]
        public void BulkValidateGround(int start, int end)
        {
            StartCoroutine(IEBulkCheckGround(start,end));
        }

        IEnumerator IEBulkCheckGround(int start, int end)
        {
            _isBulkChecking = true;
            _cachedLevelBeforeDebug = PlayerData.level;
            
            var listErrorLevels = new List<int>();
            for (int i = start; i < end; i++)
            {
                _currentYieldWait = new EventYieldWait();
                var currentLevel = i + 1;
                PlayerData.level = currentLevel;
                EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
                yield return _currentYieldWait;
                Debug.LogError("Start checking level " + currentLevel);
                CheckGroundForObjects();
                if (invalidObjects != null && invalidObjects.Count > 0) //is invalid level
                {
                    listErrorLevels.Add(currentLevel);
                }
            }

            if (listErrorLevels.Count > 0)
            {
                Debug.LogError($"Level Error {start + 1} - {end}: {new JsonDotNetModule().ToJson(listErrorLevels)}");
            }
            else
            {
                Debug.LogError("ALL LEVELS HAVE VALID GROUND");
            }
            
            Debug.LogError("Completed Bulk Check");
            
            _isBulkChecking = false;
            PlayerData.level = _cachedLevelBeforeDebug;
            EventController.RaiseEvent(EventList.GamePlay.NEW_GAME);
        }
        
        [Button]
        public void CheckGroundForObjects()
        {
            invalidObjects.Clear();
            var baseItems = itemHolder.GetComponentsInChildren<BaseItem>();
            foreach (var item in baseItems)
            {
                var childWithCollider = FindFirstActiveChildWithCollider(item.gameObject);
                if (childWithCollider != null)
                {
                    PerformGroundCheck(childWithCollider);
                }
            }
        }

        private GameObject FindFirstActiveChildWithCollider(GameObject parent)
        {
            if (parent.activeSelf && parent.GetComponent<Collider>() != null)
            {
                return parent;
            }

            foreach (Transform child in parent.transform)
            {
                if (!child.gameObject.activeSelf) continue;
                if (child.GetComponent<Collider>() != null)
                {
                    return child.gameObject;
                }

                var foundInChild = FindFirstActiveChildWithCollider(child.gameObject);
                if (foundInChild != null)
                {
                    return foundInChild;
                }
            }

            return null;
        }

        private void PerformGroundCheck(GameObject obj)
        {
            var collider = obj.GetComponent<Collider>();
            var bounds = collider.bounds;
            var center = bounds.center;
            var size = bounds.size;

            var shrunkWidth = size.x * HORIZONTAL_OFFSET_DECREASE;
            var shrunkDepth = size.z * HORIZONTAL_OFFSET_DECREASE;

            var halfWidth = shrunkWidth / 2f;
            var halfDepth = shrunkDepth / 2f;

            Vector3[] sphereOrigins = new Vector3[]
            {
                center + new Vector3(-halfWidth, 2, -halfDepth), // Front Left
                center + new Vector3(halfWidth, 2, -halfDepth),  // Front Right
                center + new Vector3(-halfWidth, 2, halfDepth),  // Back Left
                center + new Vector3(halfWidth, 2, halfDepth),   // Back Right
                center + new Vector3(0,2,0),                     // Center
            };

            bool allSpheresHitGround = true;
            float closestHitDistance = Mathf.Infinity;
            RaycastHit closestHit = new RaycastHit();

            foreach (var origin in sphereOrigins)
            {
                RaycastHit hit;
                bool sphereHit = Physics.SphereCast(
                    origin,
                    SPHERE_RADIUS,
                    Vector3.down,
                    out hit,
                    Mathf.Infinity,
                    groundValidatorLayer
                );

                if (!sphereHit)
                {
                    allSpheresHitGround = false;
                }
                else if (hit.distance < closestHitDistance)
                {
                    closestHitDistance = hit.distance;
                    closestHit = hit;
                }
                
                Color debugColor = sphereHit ? Color.green : Color.red;
                Debug.DrawRay(origin, Vector3.down * (sphereHit ? hit.distance : 100f), debugColor, 2f);
                DrawDebugSphere(origin, debugColor);
                if (sphereHit)
                {
                    DrawDebugSphere(origin + Vector3.down * hit.distance, debugColor);
                }
            }

            DrawDebugBounds(center, new Vector3(shrunkWidth, size.y, shrunkDepth), allSpheresHitGround);

            if (!allSpheresHitGround)
            {
                invalidObjects.Add(obj);
            }
        }

        private void DrawDebugSphere(Vector3 center, Color color)
        {
            int segments = 8;
            float deltaAngle = 360f / segments;
            
            // Draw circles in all three planes
            for (int plane = 0; plane < 3; plane++)
            {
                for (int i = 0; i < segments; i++)
                {
                    float angle1 = i * deltaAngle * Mathf.Deg2Rad;
                    float angle2 = (i + 1) * deltaAngle * Mathf.Deg2Rad;
                    
                    Vector3 p1 = center;
                    Vector3 p2 = center;
                    
                    switch (plane)
                    {
                        case 0: // XY plane
                            p1 += new Vector3(Mathf.Cos(angle1), Mathf.Sin(angle1), 0) * SPHERE_RADIUS;
                            p2 += new Vector3(Mathf.Cos(angle2), Mathf.Sin(angle2), 0) * SPHERE_RADIUS;
                            break;
                        case 1: // XZ plane
                            p1 += new Vector3(Mathf.Cos(angle1), 0, Mathf.Sin(angle1)) * SPHERE_RADIUS;
                            p2 += new Vector3(Mathf.Cos(angle2), 0, Mathf.Sin(angle2)) * SPHERE_RADIUS;
                            break;
                        case 2: // YZ plane
                            p1 += new Vector3(0, Mathf.Cos(angle1), Mathf.Sin(angle1)) * SPHERE_RADIUS;
                            p2 += new Vector3(0, Mathf.Cos(angle2), Mathf.Sin(angle2)) * SPHERE_RADIUS;
                            break;
                    }
                    
                    Debug.DrawLine(p1, p2, color, 2f);
                }
            }
        }

        private void DrawDebugBounds(Vector3 center, Vector3 size, bool hasGround)
        {
            Color debugColor = hasGround ? Color.green : Color.red;
            Vector3 halfSize = size / 2;

            Vector3[] corners = new Vector3[]
            {
                center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z),
                center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z),
                center + new Vector3(halfSize.x, -halfSize.y, halfSize.z),
                center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z)
            };

            for (int i = 0; i < corners.Length; i++)
            {
                Debug.DrawLine(corners[i], corners[(i + 1) % corners.Length], debugColor, 2f);
            }
        }
    }
}