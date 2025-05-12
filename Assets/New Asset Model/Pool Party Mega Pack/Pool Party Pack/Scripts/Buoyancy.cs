// Pool Floaties from Luceed Studio - https://luceed.studio

using UnityEngine;

namespace LuceedStudio_PoolFloaties
{
    public class Buoyancy : MonoBehaviour
    {
        [Header("Trigger")]
        [SerializeField] private bool triggerOnStart = true;

        [Header("Options")]
        [SerializeField, Range(0.1f, 2f)] private float globalAmplitude = 1f;
        [SerializeField, Range(0.1f, 2f)] private float globalSpeed = 1f;

        [Header("X Axis")]
        [SerializeField] private bool useX = true;
        [SerializeField] private Vector2 xAmplitudeRange = new Vector2(8f, 12f);
        [SerializeField] private Vector2 xDurationRange = new Vector2(0.2f, 0.4f);

        [Header("Z Axis")]
        [SerializeField] private bool useZ = true;
        [SerializeField] private Vector2 zAmplitudeRange = new Vector2(4f, 6f);
        [SerializeField] private Vector2 zDurationRange = new Vector2(0.2f, 0.4f);

        private bool isUpdating = false;
        public bool IsUpdating { get { return isUpdating; } }

        private Vector3 initialLocalEulerAngles = Vector3.zero;

        private float xCurrentAmplitude = 0f;
        private float xCurrentDuration = 0f;
        private bool xForth = true;
        private float xStartValue = 0f;
        private float xEndValue = 0f;
        private float xTime = 0f;

        private float zCurrentAmplitude = 0f;
        private float zCurrentDuration = 0f;
        private bool zForth = true;
        private float zStartValue = 0f;
        private float zEndValue = 0f;
        private float zTime = 0f;

        private void Awake()
        {
            initialLocalEulerAngles = transform.localEulerAngles;
        }

        private void Start()
        {
            if (triggerOnStart)
            {
                UpdateState(true);
            }
        }

        public void UpdateState(bool state)
        {
            xCurrentAmplitude = GetAmplitudeX();
            xCurrentDuration = GetDurationX();
            xForth = true;
            xTime = 0f;

            zCurrentAmplitude = GetAmplitudeZ();
            zCurrentDuration = GetDurationZ();
            zForth = true;
            zTime = 0f;

            isUpdating = state;

            if (!state)
            {
                transform.localEulerAngles = initialLocalEulerAngles;
            }
        }

        private void UpdateForthX()
        {
            xCurrentAmplitude = GetAmplitudeX();
            xCurrentDuration = GetDurationX();
            xForth = !xForth;
            xStartValue = GetClampedRotationAxis(transform.localEulerAngles.x);
            xEndValue = xForth ? initialLocalEulerAngles.x + xCurrentAmplitude : initialLocalEulerAngles.x - xCurrentAmplitude;
            xTime = 0f;
        }

        private void UpdateForthZ()
        {
            zCurrentAmplitude = GetAmplitudeZ();
            zCurrentDuration = GetDurationZ();
            zForth = !zForth;
            zStartValue = GetClampedRotationAxis(transform.localEulerAngles.z);
            zEndValue = zForth ? initialLocalEulerAngles.z + zCurrentAmplitude : initialLocalEulerAngles.z - zCurrentAmplitude;
            zTime = 0f;
        }

        private float GetClampedRotationAxis(float axis)
        {
            if (axis > 180)
            {
                return axis - 360;
            }
            else
            {
                return axis;
            }
        }

        private float GetAmplitudeX()
        {
            return Random.Range(xAmplitudeRange.x, xAmplitudeRange.y) * globalAmplitude;
        }

        private float GetAmplitudeZ()
        {
            return Random.Range(zAmplitudeRange.x, zAmplitudeRange.y) * globalAmplitude;
        }

        private float GetDurationX()
        {
            return Random.Range(xDurationRange.x, xDurationRange.y) * globalAmplitude;
        }

        private float GetDurationZ()
        {
            return Random.Range(zDurationRange.x, zDurationRange.y) * globalAmplitude;
        }

        private void FixedUpdate()
        {
            if (isUpdating)
            {
                xTime += Time.deltaTime * globalSpeed;
                zTime += Time.deltaTime * globalSpeed;

                if (xTime >= xCurrentDuration)
                {
                    UpdateForthX();
                }

                if (zTime >= zCurrentDuration)
                {
                    UpdateForthZ();
                }

                Vector3 tempLocalEulerAngles = transform.localEulerAngles;
                
                if (useX)
                {
                    float xT = xTime / xCurrentDuration;
                    xT = xT * xT * (3f - 2f * xT);

                    float xValue = Mathf.Lerp(xStartValue, xEndValue, xT);
                    tempLocalEulerAngles.x = xValue;
                }

                if (useZ)
                {
                    float zT = zTime / zCurrentDuration;
                    zT = zT * zT * (3f - 2f * zT);

                    float zValue = Mathf.Lerp(zStartValue, zEndValue, zT);
                    tempLocalEulerAngles.z = zValue;
                }

                transform.localEulerAngles = tempLocalEulerAngles;
            }
        }
    }
}

