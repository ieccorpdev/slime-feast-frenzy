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
        private GameObject[] m_CornerTypes;

        [SerializeField]
        private MeshRenderer m_Mesh;

        public void OnSetupMat()
        {
            var wallType = Game.themeConfig.wallType;
            var typeCount = m_CornerTypes.Length;

            for (int i = 0; i < typeCount; i++)
            {
                m_CornerTypes[i].SetActive(i == wallType);
            }

            if(wallType == 0)
            {
                m_Mesh.material = Game.themeConfig.cornerMat;
            }
        }

        private void OnMouseDown()
        {
            if (Game.isFlyingIsland) return;
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_LEAVES_EFFECT, new EventParam(this, transform.position));
        }

        public void OnTweenMove()
        {
            m_Tween?.Kill(true);
            gameObject.SetActive(!Game.isHaveIsLand);
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