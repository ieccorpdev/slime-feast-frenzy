using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class PlaneEffectComponent : _BaseBehaviour
    {
        private Vector2 m_MousePos;
        private Ray m_Ray;

        private RaycastHit m_Hit;

        private void OnMouseDown()
        {
            if (!Game.isLeavesEffect) return;
            m_MousePos = GetClickPos();

            m_Ray = GamePlay.mainCamera.ScreenPointToRay(m_MousePos);

            if (Physics.Raycast(m_Ray, out m_Hit))
            {
                EventController.RaiseEvent(EventList.GamePlay.GAME_LEAVES_EFFECT, new EventParam(this, m_Hit.point));
            }
        }


        private Vector2 GetClickPos()
        {
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            else
            {
                return Input.mousePosition;
            }
        }

    }
}

