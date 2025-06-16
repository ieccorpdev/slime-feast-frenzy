using System;
using System.Collections;
using System.Collections.Generic;
using _IEC.Features.Common.Extension;
using DG.Tweening;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.UI;
using Sequence = DG.Tweening.Sequence;
using Vector2 = UnityEngine.Vector2;

namespace IEC
{
    public class SpawnBoosterEventData
    {
        public int countWinStreak;
        public Vector3 posStartSpawn;

        public SpawnBoosterEventData(int countWinStreak, Vector3 posStartSpawn)
        {
            this.countWinStreak = countWinStreak;
            this.posStartSpawn = posStartSpawn;
        }
    }
    
    public class MagicPouchAnim : _BaseBehaviour
    {
        [SerializeField] private RectTransform midView;
        [SerializeField] private GameObject pouchAnimHolder;
        [SerializeField] private RectTransform holderPosSpawnItems;

        [Header("Bag Animation")]
        [SerializeField] private RectTransform pivotCarAnim;

        // [SerializeField] private Image carEmpty;
        // [SerializeField] private Image carOccupied;
        // [SerializeField] private Sprite[] iconEmpty;
        // [SerializeField] private Sprite[] iconOccupied;
        
        [Header("Animation Settings")] 
        public float moveDuration = 0.5f;
        public float openDuration = 0.733f;
        public float moveOutDuration = 0.3f;
        public float idleDuration = 0.15f;
        public Ease moveEase = Ease.OutQuad;

        private Vector2 _startingPosition;
        private string TWEEN_ANIM_POUCH => "TWEEN_ANIM_POUCH" + gameObject.GetInstanceID();

        private void Awake()
        {
            _startingPosition = pivotCarAnim.anchoredPosition;
            pouchAnimHolder.SetActive(false);
            this.RegisterListener(EventList.GamePlay.SHOW_ANIM_SPAWN_BOOSTER, OnShowBoosterSpawningAnimation);
        }

        private void OnDestroy()
        {
            this.UnregisterListener(EventList.GamePlay.SHOW_ANIM_SPAWN_BOOSTER, OnShowBoosterSpawningAnimation);
        }
        
        private void OnShowBoosterSpawningAnimation(object param)
        {
            var countWinStreak = (int)param;
            UpdateSkinAnim();
            PlayAnimation(countWinStreak);
        }
        
        private void OnDisable()
        {
            ResetAnim();
        }

        void UpdateSkinAnim()
        {
            

            // var winStreakCount = Mathf.Clamp(MagicPouchController.WinStreakData.countLevelWin - 1, 0, 2);
            // carEmpty.sprite = iconEmpty[winStreakCount];
            // carOccupied.sprite = iconOccupied[winStreakCount];
        }

        private string GetSkinNameByWinsStreakCount()
        {
            var str = "";

            return str;
        }
        
        void ResetAnim()
        {
            DOTween.Kill(TWEEN_ANIM_POUCH);
            pivotCarAnim.anchoredPosition = _startingPosition;
            // carOccupied.gameObject.SetActive(true);
            // carEmpty.gameObject.SetActive(false);
        }

        [Button]
        private void PlayAnimation(int countWinStreak)
        {
            ResetAnim();
            pouchAnimHolder.SetActive(true);

            Sequence animationSequence = DOTween.Sequence().SetId(TWEEN_ANIM_POUCH);
            
            animationSequence.AppendInterval(1.35f); //delay for camera to zoom out
            
            animationSequence.AppendCallback(() =>
            {
            });
//            animationSequence.Join(pivotCarAnim.DOAnchorPosX(midView.rect.width / 2f, moveDuration).SetEase(moveEase));

            animationSequence.AppendCallback(() =>
            {
            });
            
            animationSequence.AppendInterval(idleDuration);
            animationSequence.AppendCallback(() =>
            {
                //openAnimationDuration = skeletonGraphic.AnimationState.GetCurrent(0).Animation.Duration;
            });
            
            animationSequence.AppendInterval(openDuration);
            animationSequence.AppendCallback(() =>
            {
                //var pos = UI.GetWorldPositionFromUI(holderPosSpawnItems);
                // carOccupied.gameObject.SetActive(false);
                // carEmpty.gameObject.SetActive(true);
                //SpawnBooster(countWinStreak, pos);
            }); 
            
            animationSequence.AppendInterval(idleDuration);
            animationSequence.AppendCallback(() =>
            {
            });
            
//            animationSequence.Append(pivotCarAnim.DOAnchorPosX(midView.rect.width * 1.5f, moveOutDuration).SetEase(moveEase));
            animationSequence.OnComplete(OnCompleteAnimation);
        }
        
        void SpawnBooster(int countWinstreak, Vector3 posSpawnBooster)
        {
            this.RaiseEvent(EventList.GamePlay.SPAWN_BOOSTER_ITEM, new SpawnBoosterEventData(countWinstreak, posSpawnBooster));
        }
        
        void OnCompleteAnimation()
        {
            pouchAnimHolder.SetActive(false);
        }
    }
}