using UnityEngine;

namespace VirtualOperatingTable
{
    public class MountPoint : MonoBehaviour
    {
        [Header("Info")]
        public string displayName;

        [Header("Side Configuration")]
        public MountSide side = MountSide.Left;

        [Header("Rail Movement Limits")]
        public float minRailPosition = -0.5f;
    
        public float maxRailPosition = 0.5f;

        [Header("Status")]
        public GameObject attachedAccessory;

        void Start()
        {
            if (string.IsNullOrEmpty(displayName))
            {
                displayName = gameObject.name.Replace("_", " ");
            }
        }

        public bool IsOccupied
        {
            get { return attachedAccessory != null; }
        }

        public bool CanAttach(GameObject accessory)
        {
            if (IsOccupied)
                return false;

            if (accessory == null)
                return false;

            TableElement element = accessory.GetComponent<TableElement>();
            if (element == null)
                return false;

            if (element.type != ElementType.Accessory)
                return false;

            return true;
        }

        public bool Attach(GameObject accessory)
        {
            if (!CanAttach(accessory))
                return false;

            TableElement element = accessory.GetComponent<TableElement>();

            if (element.currentMountPoint != null)
            {
                element.currentMountPoint.Detach();
            }

            bool wasFlipped = element.isFlipped;
            bool shouldBeFlipped = (side != element.defaultMountSide);

            if (wasFlipped != shouldBeFlipped)
            {
                accessory.transform.Rotate(0f, 180f, 0f, Space.World);
                Debug.Log("[MountPoint]" + (shouldBeFlipped ? "Rotated" : "Reverted rotation") + " " + accessory.name + " by 180°");
            }

            element.isFlipped = shouldBeFlipped;

            attachedAccessory = accessory;
            element.currentMountPoint = this;

            accessory.transform.SetParent(transform, true);
            accessory.transform.localPosition = Vector3.zero;
            accessory.transform.localRotation = Quaternion.identity;

            if (element.isFlipped)
            {
                accessory.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            element.SetAttached(true);
            element.PlayAttachAnimation();

            UpdateAccessoryMovementLimits(element);

            Debug.Log("[MountPoint] Mounted " + accessory.name + " to " + displayName + " (side: " + side + ", flipped: " + shouldBeFlipped + ")");
            return true;
        }

        private void UpdateAccessoryMovementLimits(TableElement element)
        {
            if (element.movementAxes == null || element.movementAxes.Count == 0)
                return;

            foreach (var axis in element.movementAxes)
            {
                if (axis == null || !axis.allowX)
                    continue;

                if (axis.gameObject != element.gameObject)
                    continue;

                axis.minDistanceX = minRailPosition;
                axis.maxDistanceX = maxRailPosition;
                Debug.Log("[MountPoint] Updated X-axis limits for " + axis.axisName + ": [" + minRailPosition + ", " + maxRailPosition + "]");
            }
        }

        public void Detach()
        {
            if (!IsOccupied)
                return;

            TableElement element = attachedAccessory.GetComponent<TableElement>();
            
            if (element.isFlipped)
            {
                attachedAccessory.transform.Rotate(0f, 180f, 0f, Space.World);
                element.isFlipped = false;
                Debug.Log("[MountPoint] Reverted rotation of " + attachedAccessory.name + " to default position");
            }

            element.currentMountPoint = null;
            attachedAccessory.transform.SetParent(null);
            element.SetAttached(false);

            Debug.Log("[MountPoint] Detached " + attachedAccessory.name + " from " + displayName);
            
            attachedAccessory = null;
        }

        public bool HasAccessory(GameObject accessory)
        {
            return attachedAccessory == accessory;
        }
    }
}
