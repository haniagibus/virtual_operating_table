using UnityEngine;
using System.Collections.Generic;

namespace OperatingTable
{
    public class TableElement : MonoBehaviour
    {
        [Header("Identification")]
        public string elementId;
        public string displayName;

        [Header("Element Type")]
        public ElementType type = ElementType.Component;

        [Header("Attachment")]
        public bool isAttached = true;

        [Tooltip("Mount Point tego elementu (gdzie element się mocuje)")]
        public MountPoint elementMountPoint;

        [Tooltip("Lista Mount Points stołu, do których można przyłączyć ten element")]
        public List<MountPoint> compatibleMountPoints = new List<MountPoint>();

        [Header("Movement & Rotation")]
        [Tooltip("Lista osi ruchu dla tego elementu")]
        public List<MovementAxis> movementAxes = new List<MovementAxis>();

        [Tooltip("Lista punktów obrotu dla tego elementu")]
        public List<RotationPivot> rotationPivots = new List<RotationPivot>();

        [Header("Optional")]
        [Tooltip("Grupowanie elementów (np. 'backSection', 'legSection')")]
        public string groupId = "";

        private void Awake()
        {
            elementId = gameObject.name;
            displayName = gameObject.name.Replace("_", " ");
        }

        // ============================================================
        // ATTACHMENT CONTROL
        // ============================================================

        public void Attach()
        {
            isAttached = true;
            gameObject.SetActive(true);
            Debug.Log("[TableElement] " + displayName + " został przyłączony");
        }

        public void Detach()
        {
            isAttached = false;
            gameObject.SetActive(false);
            Debug.Log("[TableElement] " + displayName + " został odłączony");
        }

        public void AttachDetach(bool isAttached)
        {
            if (isAttached)
                Detach();
            else
                Attach();
        }

        // ============================================================
        // HELPERS - AUTO FIND
        // ============================================================

        [ContextMenu("Auto Find Components")]
        public void AutoFindComponents()
        {
            // Znajdź mount point na tym elemencie
            elementMountPoint = GetComponent<MountPoint>();

            // Znajdź wszystkie osie ruchu
            movementAxes.Clear();
            MovementAxis[] foundAxes = GetComponents<MovementAxis>();
            movementAxes.AddRange(foundAxes);

            // Znajdź wszystkie pivoty
            rotationPivots.Clear();
            RotationPivot[] foundPivots = GetComponents<RotationPivot>();
            rotationPivots.AddRange(foundPivots);

            Debug.Log("[TableElement] Auto Find: znaleziono " + movementAxes.Count + " osi ruchu i " + rotationPivots.Count + " pivotów");
        }

        // ============================================================
        // GŁÓWNE API - Rotacja przez Delta (dla sliderów)
        // ============================================================

        /// <summary>
        /// Obraca konkretny pivot elementu o podaną deltę
        /// </summary>
        public bool RotatePivotByDelta(RotationPivot pivot, Vector3 axis, float delta)
        {
            if (this == null) return false;

            if (!this.rotationPivots.Contains(pivot))
            {
                Debug.LogWarning("[Table] Pivot nie należy do elementu " + elementId);
                return false;
            }

            return pivot.RotateWithVector3(axis, delta);
        }

        /// <summary>
        /// Przesuwa konkretną oś elementu o podaną deltę
        /// </summary>
        public bool MoveAxisByDelta(MovementAxis axis, Vector3 direction, float delta)
        {
            if (!this.movementAxes.Contains(axis))
            {
                Debug.LogWarning("[Table] MovementAxis nie należy do elementu " + elementId);
                return false;
            }

            return axis.MoveWithVector3(direction, delta);
        }


        // ============================================================
        // GETTERS
        // ============================================================

        public MountPoint GetElementMountPoint()
        {
            return elementMountPoint;
        }

        public List<MountPoint> GetCompatibleMountPoints()
        {
            return compatibleMountPoints;
        }

        public MovementAxis[] GetMovementAxes()
        {
            return movementAxes.ToArray();
        }

        public RotationPivot[] GetRotationPivots()
        {
            return rotationPivots.ToArray();
        }

        public bool IsAttached()
        {
            return isAttached;
        }

        // ============================================================
        // VALIDATION
        // ============================================================

        public bool CanAttachToMountPoint(MountPoint mountPoint)
        {
            return compatibleMountPoints.Contains(mountPoint);
        }
    }
}