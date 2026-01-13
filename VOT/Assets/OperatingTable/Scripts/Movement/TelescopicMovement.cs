using UnityEngine;

namespace VirtualOperatingTable
{
    public class TelescopicMovement : MonoBehaviour
    {
        [Header("Telescopic Axes")]
        [Tooltip("Lista osi w kolejności wysuwania (od pierwszej do ostatniej)")]
        public MovementAxis[] movementAxes;

        [Header("Movement Settings")]
        [Tooltip("Oś ruchu (domyślnie Y - do góry)")]
        public Vector3 movementAxis = Vector3.up;

        [Header("Table Top Attachment")]
        [Tooltip("Blat stołu do przepinania podczas ruchu")]
        public Transform tableTopElement;

        [Tooltip("Nazwa górnej sekcji nogi")]
        public string topLegSectionName = "table_leg_column_segment_4_move";

        [Tooltip("Nazwa głównego obiektu stołu")]
        public string mainTableName = "table";

        public float stepSize = 0.0001f;
        private bool isMoving = false;

        void Start()
        {
            ValidateAxes();
        }

        private void ValidateAxes()
        {
            int validCount = 0;

            foreach (MovementAxis axis in movementAxes)
            {
                if (axis != null)
                {
                    validCount++;
                }
                else
                {
                    Debug.LogWarning("[TelescopicMovement] Brak przypisanego MovementAxis!");
                }
            }

            Debug.Log("[TelescopicMovement] Zainicjalizowano " + validCount + " osi teleskopowych");
        }

        public bool Move(float delta)
        {
            if (!isMoving)
            {
                BeginMovement();
                isMoving = true;
            }

            bool moved = MoveInternal(delta);

            if (!moved)
            {
                EndMovement();
                isMoving = false;
            }

            return moved;
        }


        private bool MoveInternal(float delta)
        {
            if (movementAxes == null || movementAxes.Length == 0)
                return false;

            foreach (MovementAxis axis in movementAxes)
            {
                if (axis == null)
                    continue;

                float currentPos = GetCurrentPosition(axis);
                float maxPos = GetMaxPosition(axis);
                float minPos = GetMinPosition(axis);

                bool canMove = delta > 0
                    ? currentPos < maxPos - stepSize
                    : currentPos > minPos + stepSize;

                if (canMove)
                {
                    return axis.MoveWithVector3(movementAxis, delta);
                }
            }

            return false;
        }


        private void BeginMovement()
        {
            if (tableTopElement == null)
                return;

            DetachFromParent(tableTopElement);
            AttachToParent(tableTopElement, topLegSectionName);

            Debug.Log("[TelescopicMovement] Blat przypięty do nogi");
        }

        private void EndMovement()
        {
            if (tableTopElement == null)
                return;

            DetachFromParent(tableTopElement);
            AttachToParent(tableTopElement, mainTableName);

            Debug.Log("[TelescopicMovement] Blat przypięty do stołu");
        }


        private void DetachFromParent(Transform element)
        {
            if (element == null || element.parent == null)
                return;

            Vector3 pos = element.position;
            Quaternion rot = element.rotation;

            element.SetParent(null);
            element.position = pos;
            element.rotation = rot;
        }

        private void AttachToParent(Transform element, string parentName)
        {
            GameObject parent = GameObject.Find(parentName);
            if (parent == null)
                return;

            Vector3 pos = element.position;
            Quaternion rot = element.rotation;

            element.SetParent(parent.transform);
            element.position = pos;
            element.rotation = rot;
        }


        private float GetCurrentPosition(MovementAxis axis)
        {
            switch (DetectAxisChar(movementAxis))
            {
                case 'X': return axis.currentPositionX;
                case 'Y': return axis.currentPositionY;
                case 'Z': return axis.currentPositionZ;
                default: return 0f;
            }
        }

        private float GetMaxPosition(MovementAxis axis)
        {
            switch (DetectAxisChar(movementAxis))
            {
                case 'X': return axis.maxDistanceX;
                case 'Y': return axis.maxDistanceY;
                case 'Z': return axis.maxDistanceZ;
                default: return 0f;
            }
        }

        private float GetMinPosition(MovementAxis axis)
        {
            switch (DetectAxisChar(movementAxis))
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

            if (absX > absY && absX > absZ) return 'X';
            if (absY > absX && absY > absZ) return 'Y';
            if (absZ > absX && absZ > absY) return 'Z';

            return '?';
        }
    }
}
