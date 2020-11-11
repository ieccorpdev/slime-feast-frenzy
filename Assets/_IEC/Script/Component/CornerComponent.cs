using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class CornerComponent : MonoBehaviour
    {
        private Tween m_Tween;

        private void OnMouseDown()
        {
            m_Tween?.Kill(true);
            Vector3 shakeVector = new Vector3(0f, 0.05f, 0f);
            m_Tween = transform.DOShakePosition(.5f, shakeVector, 20, 1f, false, true);
            EventController.RaiseEvent(EventList.GamePlay.GAME_WALL_CLICK, new EventParam(this, transform.position));
        }
    }
}