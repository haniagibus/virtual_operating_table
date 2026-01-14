using UnityEngine;

namespace VirtualOperatingTable
{
    public class RotationPivot : MonoBehaviour
    {
        public string pivotName;

        [Header("Allowed Axes")]
        public bool allowX = false;
        public bool allowY = true;
        public bool allowZ = false;

        [Header("X Axis Rotation Limits")]
        public float minAngleX = -180f;
        public float maxAngleX = 180f;

        [Header("Y Axis Rotation Limits")]
        public float minAngleY = -180f;
        public float maxAngleY = 180f;

        [Header("Z Axis Rotation Limits")]
        public float minAngleZ = -180f;
        public float maxAngleZ = 180f;

        [Header("Current Angles (Read Only)")]
        [HideInInspector]
        public float currentAngleX = 0f;
        [HideInInspector]
        public float currentAngleY = 0f;
        [HideInInspector]
        public float currentAngleZ = 0f;

        private Vector3 initialLocalRotation;

        void Awake()
        {
            if (string.IsNullOrEmpty(pivotName))
            {
                pivotName = gameObject.name.Replace("_", " ");
            }

            initialLocalRotation = transform.localEulerAngles;

            SyncCurrentAnglesWithTransform();

            Debug.Log(pivotName + " - Kąty początkowe: X=" + currentAngleX.ToString("F1") +
                      ", Y=" + currentAngleY.ToString("F1") +
                      ", Z=" + currentAngleZ.ToString("F1"));
        }

        private void SyncCurrentAnglesWithTransform()
        {
            Vector3 euler = transform.localEulerAngles;
            currentAngleX = NormalizeAngle(euler.x);
            currentAngleY = NormalizeAngle(euler.y);
            currentAngleZ = NormalizeAngle(euler.z);
        }

        public bool RotateWithVector3(Vector3 axis, float delta)
        {
            SyncCurrentAnglesWithTransform(); 
            
            char detectedAxis = DetectAxis(axis);

            if (detectedAxis == '?')
            {
                Debug.LogWarning("Nieznana oś: " + axis);
                return false;
            }

            float currentAngle = 0f;
            float minAngle = -180f;
            float maxAngle = 180f;
            bool allowed = false;

            switch (detectedAxis)
            {
                case 'X':
                    currentAngle = currentAngleX;
                    minAngle = minAngleX;
                    maxAngle = maxAngleX;
                    allowed = allowX;
                    break;
                case 'Y':
                    currentAngle = currentAngleY;
                    minAngle = minAngleY;
                    maxAngle = maxAngleY;
                    allowed = allowY;
                    break;
                case 'Z':
                    currentAngle = currentAngleZ;
                    minAngle = minAngleZ;
                    maxAngle = maxAngleZ;
                    allowed = allowZ;
                    break;
            }

            if (!allowed)
            {
                Debug.LogWarning("Oś " + detectedAxis + " jest wyłączona dla " + pivotName);
                return false;
            }

            float newAngle = currentAngle + delta;
            bool hitLimit = false;

            if (newAngle > maxAngle)
            {
                Debug.Log(pivotName + " - Osiągnięto maksymalny kąt na osi " + detectedAxis + ": " + maxAngle.ToString("F1") + "°");
                newAngle = maxAngle;
                delta = maxAngle - currentAngle;
                hitLimit = true;
            }
            else if (newAngle < minAngle)
            {
                Debug.Log(pivotName + " - Osiągnięto minimalny kąt na osi " + detectedAxis + ": " + minAngle.ToString("F1") + "°");
                newAngle = minAngle;
                delta = minAngle - currentAngle;
                hitLimit = true;
            }

            if (Mathf.Abs(delta) > 0.001f)
            {
                Vector3 newRotation = transform.localEulerAngles;
                
                switch (detectedAxis)
                {
                    case 'X':
                        newRotation.x = newAngle;
                        currentAngleX = newAngle;
                        break;
                    case 'Y':
                        newRotation.y = newAngle;
                        currentAngleY = newAngle;
                        break;
                    case 'Z':
                        newRotation.z = newAngle;
                        currentAngleZ = newAngle;
                        break;
                }
                
                transform.localEulerAngles = newRotation;

                Debug.Log(pivotName + " - Oś " + detectedAxis + ": " + currentAngle.ToString("F1") +
                         "° → " + newAngle.ToString("F1") + "° (delta: " + delta.ToString("F1") + "°)");
            }

            return !hitLimit;
        }

        private char DetectAxis(Vector3 axis)
        {
            axis = axis.normalized;
            float absX = Mathf.Abs(axis.x);
            float absY = Mathf.Abs(axis.y);
            float absZ = Mathf.Abs(axis.z);

            if (absX > absY && absX > absZ)
                return 'X';
            else if (absY > absX && absY > absZ)
                return 'Y';
            else if (absZ > absX && absZ > absY)
                return 'Z';

            return '?';
        }

        private float NormalizeAngle(float angle)
        {
            while (angle > 180f)
                angle -= 360f;
            while (angle < -180f)
                angle += 360f;
            return angle;
        }

        public float GetCurrentAngle(char axis)
        {
            SyncCurrentAnglesWithTransform();
            
            switch (axis)
            {
                case 'X':
                case 'x':
                    return currentAngleX;
                case 'Y':
                case 'y':
                    return currentAngleY;
                case 'Z':
                case 'z':
                    return currentAngleZ;
                default:
                    return 0f;
            }
        }

    }
}