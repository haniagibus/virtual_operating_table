using UnityEngine;
using System.Collections.Generic;

namespace OperatingTable
{
    public class TelescopicMovement : MonoBehaviour
    {
        [System.Serializable]
        public class TelescopicSection
        {
            [Tooltip("MovementAxis tej sekcji")]
            public MovementAxis movementAxis;

            [Tooltip("Nazwa sekcji (opcjonalne, do debugowania)")]
            public string sectionName = "";
        }

        [Header("Telescopic Sections")]
        [Tooltip("Lista sekcji w kolejności wysuwania (od pierwszej do ostatniej)")]
        public TelescopicSection[] sections;

        [Header("Movement Settings")]
        [Tooltip("Oś ruchu (domyślnie Y - do góry)")]
        public Vector3 movementAxis = Vector3.up;

        [Header("Movement Settings")]
        public float stepSize = 0.000001f;

        void Start()
        {
            ValidateSections();
        }

        private void ValidateSections()
        {
            int validCount = 0;
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis != null)
                {
                    validCount++;
                    if (string.IsNullOrEmpty(section.sectionName))
                    {
                        section.sectionName = section.movementAxis.gameObject.name;
                    }
                }
                else
                {
                    Debug.LogWarning("[TelescopicMovement] Sekcja bez przypisanego MovementAxis!");
                }
            }

            Debug.Log("[TelescopicMovement] Zainicjalizowano " + validCount + " sekcji teleskopowych");
        }

        public bool Move(float delta)
        {
            if (sections.Length == 0)
            {
                Debug.LogWarning("[TelescopicMovement] Brak zdefiniowanych sekcji!");
                return false;
            }

            bool anyMoved = false;

            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis == null)
                {
                    Debug.LogWarning("[TelescopicMovement] Sekcja bez MovementAxis!");
                    continue;
                }

                float currentPos = GetCurrentPosition(section.movementAxis);
                float maxPos = GetMaxPosition(section.movementAxis);
                float minPos = GetMinPosition(section.movementAxis);

                bool canMove = false;
                if (delta > 0)
                {
                    canMove = currentPos < maxPos - stepSize;
                }
                else
                {
                    canMove = currentPos > minPos + stepSize;
                }

                if (canMove)
                {
                    bool moved = section.movementAxis.MoveWithVector3(movementAxis, delta);

                    if (moved)
                    {
                        anyMoved = true;
                        Debug.Log("[TelescopicMovement] Poruszam sekcję: " + section.sectionName +
                                 " (pozycja: " + GetCurrentPosition(section.movementAxis).ToString("F3") + ")");
                    }

                    break;
                }
                else
                {
                    Debug.Log("[TelescopicMovement] Sekcja " + section.sectionName + " osiągnęła limit");
                }
            }

            return anyMoved;
        }

        private float GetCurrentPosition(MovementAxis axis)
        {
            char detectedAxis = DetectAxisChar(movementAxis);
            switch (detectedAxis)
            {
                case 'X': return axis.currentPositionX;
                case 'Y': return axis.currentPositionY;
                case 'Z': return axis.currentPositionZ;
                default: return 0f;
            }
        }

        private float GetMaxPosition(MovementAxis axis)
        {
            char detectedAxis = DetectAxisChar(movementAxis);
            switch (detectedAxis)
            {
                case 'X': return axis.maxDistanceX;
                case 'Y': return axis.maxDistanceY;
                case 'Z': return axis.maxDistanceZ;
                default: return 0f;
            }
        }

        private float GetMinPosition(MovementAxis axis)
        {
            char detectedAxis = DetectAxisChar(movementAxis);
            switch (detectedAxis)
            {
                case 'X': return axis.minDistanceX;
                case 'Y': return axis.minDistanceY;
                case 'Z': return axis.minDistanceZ;
                default: return 0f;
            }
        }

        private char DetectAxisChar(Vector3 axis)
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

        public bool IsFullyExtended()
        {
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis == null)
                    continue;

                float current = GetCurrentPosition(section.movementAxis);
                float max = GetMaxPosition(section.movementAxis);

                if (current < max - stepSize)
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsFullyRetracted()
        {
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis == null)
                    continue;

                float current = GetCurrentPosition(section.movementAxis);
                float min = GetMinPosition(section.movementAxis);

                if (current > min + stepSize)
                {
                    return false;
                }
            }
            return true;
        }

        public void Reset()
        {
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis != null)
                {
                    section.movementAxis.ResetPosition();
                }
            }

            Debug.Log("[TelescopicMovement] Zresetowano teleskop");
        }

        public float GetTotalHeight()
        {
            float total = 0f;
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis != null)
                {
                    total += GetCurrentPosition(section.movementAxis);
                }
            }
            return total;
        }

        public float GetMaxHeight()
        {
            float total = 0f;
            foreach (TelescopicSection section in sections)
            {
                if (section.movementAxis != null)
                {
                    total += GetMaxPosition(section.movementAxis);
                }
            }
            return total;
        }
    }
}