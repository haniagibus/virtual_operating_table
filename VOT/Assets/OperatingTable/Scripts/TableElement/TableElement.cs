using System.Collections.Generic;
using UnityEngine;

namespace OperatingTable
{
    public class TableElement : MonoBehaviour
    {
        [Header("Element Info")]
        public string elementName;

        [Header("Element Type")]
        public ElementType type = ElementType.Component;

        [Header("Default Mount Side")]
        [Tooltip("Domyślna strona montażu (lewa/prawa)")]
        public MountSide defaultMountSide = MountSide.Right;

        [Header("Flip State")]
        [HideInInspector]
        public bool isFlipped = false;

        [Header("Attachment State")]
        public bool isAttached = false;

        [Header("Rotation System")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Movement System")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        [HideInInspector]
        public MountPoint currentMountPoint;

        void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
            {
                elementName = gameObject.name.Replace("_", " ");
            }

            // Ustaw początkowy stan obrotu na false (domyślnie element jest po prawej)
            isFlipped = false;

            UpdateVisibility();
        }

        public void UpdateVisibility()
        {
            gameObject.SetActive(isAttached);
        }

        public void SetAttached(bool attached)
        {
            isAttached = attached;
            UpdateVisibility();
        }

        /// <summary>
        /// Sprawdza czy element jest zamontowany po przeciwnej stronie niż domyślna
        /// </summary>
        public bool IsMountedOnOppositeSide()
        {
            if (currentMountPoint == null)
                return false;

            return currentMountPoint.side != defaultMountSide;
        }

        /// <summary>
        /// Zwraca pivot o podanej nazwie
        /// </summary>
        public RotationPivot GetPivotByName(string pivotName)
        {
            return rotationPivots.Find(p => p.pivotName == pivotName);
        }

        /// <summary>
        /// Zwraca oś ruchu o podanej nazwie
        /// </summary>
        public MovementAxis GetAxisByName(string axisName)
        {
            return movementAxes.Find(a => a.axisName == axisName);
        }

        /// <summary>
        /// Sprawdza czy element ma jakiekolwiek pivoty
        /// </summary>
        public bool HasRotationPivots()
        {
            return rotationPivots != null && rotationPivots.Count > 0;
        }

        /// <summary>
        /// Sprawdza czy element ma jakiekolwiek osie ruchu
        /// </summary>
        public bool HasMovementAxes()
        {
            return movementAxes != null && movementAxes.Count > 0;
        }
    }
}