using UnityEngine;

namespace OperatingTable
{
    public class MountPoint : MonoBehaviour
    {
        [Header("Info")]
        public string displayName;

        [Header("Side Configuration")]
        [Tooltip("Strona stołu (lewa/prawa)")]
        public MountSide side = MountSide.Left;

        [Header("Rail Movement Limits")]
        [Tooltip("Minimalna pozycja przesunięcia po szynie (oś X)")]
        public float minRailPosition = -0.5f;
        
        [Tooltip("Maksymalna pozycja przesunięcia po szynie (oś X)")]
        public float maxRailPosition = 0.5f;

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

            // Sprawdź czy element był obrócony
            bool wasFlipped = element.isFlipped;
            
            // Sprawdź czy element powinien być obrócony na nowym mount poincie
            // Obrót następuje gdy strona mount pointa jest ODWROTNA niż defaultowa strona elementu
            bool shouldBeFlipped = (side != element.defaultMountSide);

            // Jeśli zmienia się stan obrotu, obróć o 180°
            if (wasFlipped != shouldBeFlipped)
            {
                accessory.transform.Rotate(0f, 180f, 0f, Space.World);
                Debug.Log((shouldBeFlipped ? "Obrócono" : "Cofnięto obrót") + " " + accessory.name + " o 180°");
            }

            // Zaktualizuj stan
            element.isFlipped = shouldBeFlipped;

            attachedAccessory = accessory;
            element.currentMountPoint = this;

            // Ustaw parent z zachowaniem world position (ale pozwól na dziedziczenie rotacji)
            accessory.transform.SetParent(transform, true);

            // Ustaw lokalną pozycję na zero (przyklej do mount pointa)
            accessory.transform.localPosition = Vector3.zero;
            
            // Ustaw lokalną rotację na zero (przyjmij rotację mount pointa)
            accessory.transform.localRotation = Quaternion.identity;
            
            // Jeśli element jest obrócony, zastosuj obrót lokalny
            if (element.isFlipped)
            {
                accessory.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
            }

            element.SetAttached(true);

            // Zaktualizuj limity ruchu dla osi X w zależności od mount pointa
            UpdateAccessoryMovementLimits(element);

            Debug.Log("Mounted " + accessory.name + " to " + displayName + " (side: " + side + ", flipped: " + shouldBeFlipped + ")");
            return true;
        }

        /// <summary>
        /// Aktualizuje limity ruchu akcesorium na podstawie limitów mount pointa
        /// </summary>
        private void UpdateAccessoryMovementLimits(TableElement element)
        {
            if (element.movementAxes == null || element.movementAxes.Count == 0)
                return;

            foreach (var axis in element.movementAxes)
            {
                if (axis == null || !axis.allowX)
                    continue;

                // Sprawdź czy MovementAxis jest na tym samym GameObject co TableElement
                if (axis.gameObject != element.gameObject)
                    continue;

                axis.minDistanceX = minRailPosition;
                axis.maxDistanceX = maxRailPosition;
                Debug.Log("Zaktualizowano limity osi X dla " + axis.axisName + ": [" + minRailPosition + ", " + maxRailPosition + "]");
            }
        }

        public void Detach()
        {
            if (!IsOccupied)
                return;

            TableElement element = attachedAccessory.GetComponent<TableElement>();
            
            // Jeśli element był obrócony, cofnij obrót do pozycji defaultowej
            if (element.isFlipped)
            {
                attachedAccessory.transform.Rotate(0f, 180f, 0f, Space.World);
                element.isFlipped = false;
                Debug.Log("Cofnięto obrót " + attachedAccessory.name + " do pozycji defaultowej");
            }

            element.currentMountPoint = null;
            attachedAccessory.transform.SetParent(null);
            
            // Wyłącz widoczność elementu
            element.SetAttached(false);

            Debug.Log("Detached " + attachedAccessory.name + " from " + displayName);
            
            attachedAccessory = null;
        }

        public bool HasAccessory(GameObject accessory)
        {
            return attachedAccessory == accessory;
        }
    }
}