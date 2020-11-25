using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class AnimalComponent : MonoBehaviour
    {
        [SerializeField]
        private Animator m_Anim;

        private Vector3 m_SpawnPos, m_TargetPos;
        public float m_Speed, m_Delta, m_TimeCountDelay, m_NextTimeStart;
        private bool m_IsMoving;

        private static int SPEED = Animator.StringToHash("Speed");

        public void OnInit()
        {
            OnReset();
        }

        public void OnUpdate(float deltaTime)
        {
            if (m_IsMoving)
            {
                transform.localPosition = Vector3.MoveTowards(transform.localPosition, m_TargetPos, deltaTime * m_Speed);
                m_Delta = Vector3.Distance(transform.localPosition, m_TargetPos);
                if(m_Delta < .1f)
                {
                    OnReset();
                }
            }
            else
            {
                m_TimeCountDelay += deltaTime;
                if(m_TimeCountDelay >= m_NextTimeStart)
                {
                    StartFly();
                }
            }
        }

        private void OnReset()
        {
            m_NextTimeStart = Random.Range(2f, 8f);
            m_TimeCountDelay = 0f;
            m_IsMoving = false;
        }

        private void StartFly()
        {
            var isSpawnLeft = Random.Range(0, 2) == 0;

            m_SpawnPos.x = isSpawnLeft ? -2f : 2f;
            m_SpawnPos.y = Random.Range(-1.5f, 1.5f);
            m_SpawnPos.z = 4f;

            transform.localPosition = m_SpawnPos;

            m_TargetPos.x = m_SpawnPos.x * -1;
            m_TargetPos.y = Random.Range(-1.5f, 1.5f);
            m_TargetPos.z = 4f;

            m_Speed = Random.Range(.5f, 1f);
            m_Anim.SetFloat(SPEED, m_Speed);

            var size = Vector3.one * Random.Range(.4f, .8f);
            transform.localScale = size;
            var targetDir = (m_TargetPos - transform.localPosition).normalized;

            var angleX = isSpawnLeft ? 0f : 180f;
            var angleZ = Vector3.SignedAngle(targetDir, transform.parent.right, isSpawnLeft ? Vector3.back : Vector3.forward);


            transform.localRotation = Quaternion.Euler(angleX, 0f, angleZ);
            m_IsMoving = true;
        }

    }
}