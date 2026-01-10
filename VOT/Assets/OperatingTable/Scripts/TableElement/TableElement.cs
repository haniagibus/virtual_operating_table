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

        [Header("Attachment State")]
        public bool isAttached = true;

        [Header("Rotation System")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Movement System")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        // [Header("Visibility")]
        // [SerializeField] private bool isVisible = true;

        [HideInInspector]
        public MountPoint currentMountPoint;

        void Awake()
        {
            if (string.IsNullOrEmpty(elementName))
            {
                elementName = gameObject.name.Replace("_", " ");
            }

            // UpdateVisibility();
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

        // public void SetAttached(bool attached)
        // {
        //     isAttached = attached;
        // }

        // public void SetVisible(bool visible)
        // {
        //     isVisible = visible;
        //     gameObject.SetActive(visible);
        // }

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