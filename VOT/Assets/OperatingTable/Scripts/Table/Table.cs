using UnityEngine;
using System.Collections.Generic;
using OperatingTable;

namespace OperatingTable
{
    public class Table : MonoBehaviour
    {
        [Header("Auto-Discovery")]
        [Tooltip("Automatycznie znajdź wszystkie elementy w dzieciach")]
        public bool autoDiscoverElements = true;

        private Dictionary<string, TableElement> elements = new Dictionary<string, TableElement>();

        void Awake()
        {
            if (autoDiscoverElements)
            {
                DiscoverElements();
            }
        }

        /// <summary>
        /// Automatycznie znajduje wszystkie TableElement w hierarchii
        /// </summary>
        public void DiscoverElements()
        {
            elements.Clear();
            TableElement[] foundElements = GetComponentsInChildren<TableElement>();

            foreach (TableElement element in foundElements)
            {
                if (!string.IsNullOrEmpty(element.elementId))
                {
                    elements[element.elementId] = element;
                    Debug.Log("[OperatingTable] Znaleziono element: " + element.elementId + " (" + element.displayName + ")");
                }
            }

            Debug.Log("[OperatingTable] Odkryto " + elements.Count + " elementów");
        }

        // ============================================================
        // GŁÓWNE API - Rotacja
        // ============================================================

        /// <summary>
        /// Obraca element wokół określonej osi używając pierwszego dostępnego pivotu
        /// </summary>
        public bool RotateElement(string elementId, Vector3 axis, float delta)
        {
            TableElement element = GetElement(elementId);
            if (element == null) return false;

            RotationPivot[] pivots = element.GetRotationPivots();
            if (pivots.Length == 0)
            {
                Debug.LogWarning("[OperatingTable] Element " + elementId + " nie ma żadnych RotationPivot");
                return false;
            }

            // Użyj pierwszego pivotu (lub można wybrać który)
            return pivots[0].RotateWithVector3(axis, delta);
        }

        /// <summary>
        /// Obraca element używając konkretnego pivotu
        /// </summary>
        public bool RotateElementWithPivot(string elementId, string pivotName, Vector3 axis, float delta)
        {
            TableElement element = GetElement(elementId);
            if (element == null) return false;

            RotationPivot[] pivots = element.GetRotationPivots();
            foreach (RotationPivot pivot in pivots)
            {
                if (pivot.pivotName == pivotName)
                {
                    return pivot.RotateWithVector3(axis, delta);
                }
            }

            Debug.LogWarning("[OperatingTable] Nie znaleziono pivotu '" + pivotName + "' w elemencie " + elementId);
            return false;
        }

        // ============================================================
        // GŁÓWNE API - Przesuw
        // ============================================================

        /// <summary>
        /// Przesuwa element wzdłuż osi używając pierwszego dostępnego MovementAxis
        /// </summary>
        public bool MoveElement(string elementId, Vector3 axis, float delta)
        {
            TableElement element = GetElement(elementId);
            if (element == null) return false;

            MovementAxis[] axes = element.GetMovementAxes();
            if (axes.Length == 0)
            {
                Debug.LogWarning("[OperatingTable] Element " + elementId + " nie ma żadnych MovementAxis");
                return false;
            }

            return axes[0].MoveWithVector3(axis, delta);
        }

        // ============================================================
        // HELPERS
        // ============================================================

        public TableElement GetElement(string elementId)
        {
            if (!elements.ContainsKey(elementId))
            {
                Debug.LogWarning("[OperatingTable] Element '" + elementId + "' nie został znaleziony");
                return null;
            }
            return elements[elementId];
        }

        public TableElement[] GetAllElements()
        {
            TableElement[] result = new TableElement[elements.Count];
            elements.Values.CopyTo(result, 0);
            return result;
        }

        public TableElement[] GetElementsByGroup(string groupId)
        {
            List<TableElement> result = new List<TableElement>();
            foreach (TableElement element in elements.Values)
            {
                if (element.groupId == groupId)
                {
                    result.Add(element);
                }
            }
            return result.ToArray();
        }

        /// <summary>
        /// Reset wszystkich elementów do pozycji początkowej
        /// </summary>
        public void ResetAllElements()
        {
            foreach (TableElement element in elements.Values)
            {
                // Reset rotacji
                RotationPivot[] pivots = element.GetRotationPivots();
                foreach (RotationPivot pivot in pivots)
                {
                    // RotationPivot musiałby mieć metodę Reset - można dodać
                }

                // Reset pozycji
                MovementAxis[] axes = element.GetMovementAxes();
                foreach (MovementAxis movementAxis in axes)
                {
                    movementAxis.ResetPosition();
                }
            }

            Debug.Log("[OperatingTable] Zresetowano wszystkie elementy");
        }
    }
}