using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace IEC
{
    public class AutoRotate : MonoBehaviour
    {
        [Header("Rotate axises by degrees per second")]
        public Vector3 rotateVector = Vector3.zero;

        public enum spaceEnum { Local, World };
        public spaceEnum rotateSpace;

        // Update is called once per frame
        void Update()
        {
            float delta = (Time.timeScale == 0f) ? Time.unscaledDeltaTime : Time.deltaTime;
            if (rotateSpace == spaceEnum.Local)
                transform.Rotate(rotateVector * delta);
            if (rotateSpace == spaceEnum.World)
                transform.Rotate(rotateVector * delta, Space.World);
        }
    }
}