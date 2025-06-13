using System.Collections;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace PathCreation.Examples
{
    // Moves along a path at constant speed.
    // Depending on the end of path instruction, will either loop, reverse, or stop at the end of the path.
    public class PathFollower : _BaseBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public float distanceTravelled;

        public bool followRotation = true;

        private bool moving = false;
        
        private void Awake()
        {
            this.RegisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.RegisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.PAUSE_GAME,OnPauseGame);
            this.UnregisterListener(EventList.GamePlay.RESUME_GAME,OnResumeGame);
        }

        private void OnPauseGame(object param)
        {
            moving = false;
        }

        private void OnResumeGame(object param)
        {
            moving = true;
        }

        void Start() {
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
                speed = Random.Range(0.5f, 1.1f);
                moving = false;
                float startTime = Random.Range(1f, 4f);
                StartCoroutine(DelayStart(startTime));
            }
        }

        private IEnumerator DelayStart(float delayTime)
        {
            yield return new WaitForSeconds(delayTime);
            moving = true;
        }
        
        void Update()
        {
            if (pathCreator != null)
            {
                if (moving)
                {
                    transform.position = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                    distanceTravelled += speed * Time.deltaTime;
                }
                if (followRotation)
                {
                    transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);   
                }
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged() {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }

        public void StopMove()
        {
            StopAllCoroutines();
            moving = false;
        }
        
        public void StartMoveDelay()
        {
            StartCoroutine(DelayStart(2f));
            transform.DORotate(Vector3.zero, 0.5f);
        }
    }
}