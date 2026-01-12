using UnityEngine;

namespace OperatingTable
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

        private Vector3 initialLocalRotation; // NOWE

        void Awake()
        {
            if (string.IsNullOrEmpty(pivotName))
            {
                pivotName = gameObject.name.Replace("_", " ");
            }

            // Zapisz początkową rotację lokalną
            initialLocalRotation = transform.localEulerAngles;

            // Inicjalizuj kąty z aktualnej rotacji obiektu
            SyncCurrentAnglesWithTransform(); // ZMIENIONE

            Debug.Log(pivotName + " - Kąty początkowe: X=" + currentAngleX.ToString("F1") +
                      ", Y=" + currentAngleY.ToString("F1") +
                      ", Z=" + currentAngleZ.ToString("F1"));
        }

        // NOWA METODA - synchronizuj currentAngle z rzeczywistą rotacją
        private void SyncCurrentAnglesWithTransform()
        {
            Vector3 euler = transform.localEulerAngles;
            currentAngleX = NormalizeAngle(euler.x);
            currentAngleY = NormalizeAngle(euler.y);
            currentAngleZ = NormalizeAngle(euler.z);
        }

        public bool RotateWithVector3(Vector3 axis, float delta)
        {
            // NAJPIERW zsynchronizuj kąty z rzeczywistością
            SyncCurrentAnglesWithTransform(); // NOWE
            
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
                // ZAMIAST transform.Rotate(), ustaw bezpośrednio localEulerAngles
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
                
                transform.localEulerAngles = newRotation; // ZMIENIONE z transform.Rotate()

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

        public bool IsAtLimitX()
        {
            SyncCurrentAnglesWithTransform(); // NOWE
            return currentAngleX <= minAngleX + 0.1f || currentAngleX >= maxAngleX - 0.1f;
        }

        public bool IsAtLimitY()
        {
            SyncCurrentAnglesWithTransform(); // NOWE
            return currentAngleY <= minAngleY + 0.1f || currentAngleY >= maxAngleY - 0.1f;
        }

        public bool IsAtLimitZ()
        {
            SyncCurrentAnglesWithTransform(); // NOWE
            return currentAngleZ <= minAngleZ + 0.1f || currentAngleZ >= maxAngleZ - 0.1f;
        }

        public float GetCurrentAngle(char axis)
        {
            SyncCurrentAnglesWithTransform(); // NOWE - zawsze aktualna wartość
            
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

        // NOWE METODY
        public void SetCurrentAngle(char axis, float angle)
        {
            switch (axis)
            {
                case 'X':
                case 'x':
                    currentAngleX = angle;
                    break;
                case 'Y':
                case 'y':
                    currentAngleY = angle;
                    break;
                case 'Z':
                case 'z':
                    currentAngleZ = angle;
                    break;
            }
        }

        public void SetRotationDirect(float angleX, float angleY, float angleZ)
        {
            Vector3 newRotation = transform.localEulerAngles;
            
            if (allowX)
            {
                newRotation.x = angleX;
                currentAngleX = angleX;
            }
            
            if (allowY)
            {
                newRotation.y = angleY;
                currentAngleY = angleY;
            }
            
            if (allowZ)
            {
                newRotation.z = angleZ;
                currentAngleZ = angleZ;
            }
            
            transform.localEulerAngles = newRotation;
            
            Debug.Log(pivotName + " - Bezpośrednio ustawiono rotację: X=" + angleX.ToString("F1") + 
                      ", Y=" + angleY.ToString("F1") + ", Z=" + angleZ.ToString("F1"));
        }
    }
}