using _IEC.Features.Common.Extension;
using DG.Tweening;
using IEC;
using UnityEngine;

namespace _IEC.Features.Gameplay
{
    public class BaseBoosterItem : BaseItem, IBoosterItem
    {
        private string TWEEN_ANIM_MOVE => "TWEEN_ANIM_MOVE" + gameObject.GetInstanceID();
        public virtual void ApplyEffect() { }
        
        public BoosterData BoosterData { get; private set; }

        public void Init()
        {
            SetLayer(LayerMask.NameToLayer("Default"));
            ChangeState(State.None);
            _isCollected = false;
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

        public void SetBoosterData(BoosterData boosterData)
        {
            BoosterData = boosterData;
        }

        public override void OnCollected(Vector3 position)
        {
            base.OnCollected(position);
            
            this.RaiseEvent(EventList.GamePlay.ON_BOOSTER_COLLECTED, BoosterData);
        }

        public virtual void MoveToPos(Vector3 pos)
        {
            DOTween.Kill(TWEEN_ANIM_MOVE);
            var randomJumpPower = Random.Range(4f, 7f);
            var randomRotation = new Vector3(
                Random.Range(-360f, 360f), 
                Random.Range(-360f, 360f), 
                Random.Range(-360f, 360f)
            );
            var duration = 0.75f;
            transform.eulerAngles = randomRotation;
            
            transform.DOJump(pos, randomJumpPower, 1, duration)
                .SetEase(Ease.OutQuad)
                .Join(transform.DORotate(Vector3.zero, duration)) 
                .SetId(TWEEN_ANIM_MOVE)
                .OnComplete(() =>
                {
                    Rb.isKinematic = false;
                });
        }

        void OnDisable()
        {
            DOTween.Kill(TWEEN_ANIM_MOVE);
        }
    }
}