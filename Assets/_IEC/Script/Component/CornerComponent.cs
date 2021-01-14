using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class CornerComponent : _BaseBehaviour
    {
        private Tween m_Tween;
        private Vector3 CurrentLocalPos => transform.localPosition;

        [SerializeField]
        private MeshRenderer m_Mesh;

        public void OnSetupMat(Material mat)
        {
            m_Mesh.material = mat;
        }

        private void OnMouseDown()
        {
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_LEAVES_EFFECT, new EventParam(this, transform.position));
        }

        public void OnTweenMove()
        {
            m_Tween?.Kill(true);
            gameObject.SetActive(true);
            if (!Game.isStartLevelEffect) return;
            var moveTime = Config.startEffectMoveTime;
            var delayTime = 0f;
             
            var tempPos = CurrentLocalPos;
            tempPos.y = 20f;
            transform.localPosition = tempPos;

            m_Tween = transform.DOLocalMoveY(0f, moveTime).SetDelay(delayTime);
        }

        public void OnDestroyMe()
        {
            if (!Game.isStartLevelEffect) return;
            gameObject.SetActive(false);
        }
    }
}