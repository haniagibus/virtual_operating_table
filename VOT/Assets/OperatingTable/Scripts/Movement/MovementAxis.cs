using UnityEngine;

namespace VirtualOperatingTable
{
    public class MovementAxis : MonoBehaviour
    {
        public string axisName;

        [Header("Allowed Axes")]
        public bool allowX = false;
        public bool allowY = true;
        public bool allowZ = false;

        [Header("X Axis Movement Limits (in Unity units)")]
        public float minDistanceX = -1f;
        public float maxDistanceX = 1f;

        [Header("Y Axis Movement Limits")]
        public float minDistanceY = -1f;
        public float maxDistanceY = 1f;

        [Header("Z Axis Movement Limits")]
        public float minDistanceZ = -1f;
        public float maxDistanceZ = 1f;

        [Header("Current Position (Read Only)")]
        [HideInInspector]
        public float currentPositionX = 0f;
        [HideInInspector]
        public float currentPositionY = 0f;
        [HideInInspector]
        public float currentPositionZ = 0f;

        [Header("Movement Settings")]
        public float stepSize = 0.000001f;

        private Vector3 initialPosition;

        void Awake()
        {
            if (string.IsNullOrEmpty(axisName))
            {
                axisName = gameObject.name.Replace("_", " ");
            }

            initialPosition = transform.localPosition;
            currentPositionX = 0f;
            currentPositionY = 0f;
            currentPositionZ = 0f;
        }

        public bool MoveWithVector3(Vector3 axis, float delta)
        {
            char detectedAxis = DetectAxis(axis);

            if (detectedAxis == '?')
            {
                Debug.LogWarning("[MovementAxis] Unknown axis: " + axis);
                return false;
            }

            float currentPos = 0f;
            float minDist = -1f;
            float maxDist = 1f;
            bool allowed = false;

            switch (detectedAxis)
            {
                case 'X':
                    currentPos = currentPositionX;
                    minDist = minDistanceX;
                    maxDist = maxDistanceX;
                    allowed = allowX;
                    break;
                case 'Y':
                    currentPos = currentPositionY;
                    minDist = minDistanceY;
                    maxDist = maxDistanceY;
                    allowed = allowY;
                    break;
                case 'Z':
                    currentPos = currentPositionZ;
                    minDist = minDistanceZ;
                    maxDist = maxDistanceZ;
                    allowed = allowZ;
                    break;
            }

            if (!allowed)
            {
                Debug.LogWarning("[MovementAxis] Axis " + detectedAxis + " is disabled for " + axisName);
                return false;
            }

            float newPos = currentPos + delta;
            bool hitLimit = false;

            if (newPos > maxDist)
            {
                Debug.Log("[MovementAxis]" + axisName + " - Maximum position reached on axis " + detectedAxis);
                newPos = maxDist;
                delta = maxDist - currentPos;
                hitLimit = true;
            }
            else if (newPos < minDist)
            {
                Debug.Log("[MovementAxis]" + axisName + " - Minimum position reached on axis " + detectedAxis);
                newPos = minDist;
                delta = minDist - currentPos;
                hitLimit = true;
            }

            if (Mathf.Abs(delta) > stepSize)
            {
                Vector3 movement = axis.normalized * delta;
                transform.localPosition += movement;

                switch (detectedAxis)
                {
                    case 'X':
                        currentPositionX = newPos;
                        break;
                    case 'Y':
                        currentPositionY = newPos;
                        break;
                    case 'Z':
                        currentPositionZ = newPos;
                        break;
                }
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
    }
}
