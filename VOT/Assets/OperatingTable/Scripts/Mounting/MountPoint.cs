using UnityEngine;

namespace OperatingTable
{
    public class MountPoint : MonoBehaviour
    {
        [Header("Info")]
        public string displayName;

        [Header("Status")]
        [Tooltip("Aktualnie podłączone akcesorium")]
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

        /// <summary>
        /// Sprawdza czy obiekt może być podłączony jako akcesorium
        /// </summary>
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

            // jeśli było gdzieś indziej – odłącz
            if (element.currentMountPoint != null)
            {
                element.currentMountPoint.Detach();
            }

            attachedAccessory = accessory;
            element.currentMountPoint = this;

            // 🔥 KLUCZOWA LINIA
            accessory.transform.SetParent(transform);

            accessory.transform.localPosition = Vector3.zero;
            // accessory.transform.localRotation = Quaternion.identity;
            // accessory.transform.localScale = Vector3.one;

            element.SetAttached(true);

            Debug.Log("Mounted " + accessory.name + " to " + displayName);
            return true;
        }


        public void Detach()
        {
            if (!IsOccupied)
                return;

            TableElement element = attachedAccessory.GetComponent<TableElement>();
            element.currentMountPoint = null;

            attachedAccessory.transform.SetParent(null);
            attachedAccessory = null;
        }

        public bool HasAccessory(GameObject accessory)
        {
            return attachedAccessory == accessory;
        }
    }
}
