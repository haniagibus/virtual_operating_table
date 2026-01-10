using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace OperatingTable
{
    [System.Serializable]
    public class TablePosition
    {
        public string name;
        public float backUpperAngleX;
        public float backUpperAngleY;
        public float backUpperAngleZ;
        public float backLowerAngleX;
        public float backLowerAngleY;
        public float backLowerAngleZ;
        public float backLowerLeftAngleX;
        public float backLowerLeftAngleY;
        public float backLowerLeftAngleZ;
        public float backLowerRightAngleX;
        public float backLowerRightAngleY;
        public float backLowerRightAngleZ;
        public float tableRotationAngleX;
        public float tableRotationAngleY;
        public float tableRotationAngleZ;
        public float tableHeight;
        public float tableLongitudinalX;
        public bool isReversed;

        public TablePosition(string positionName)
        {
            name = positionName;
        }

        public bool IsEmpty()
        {
            return string.IsNullOrEmpty(name);
        }
    }

    public class TablePositionManager : MonoBehaviour
    {
        [Header("Hand Control Reference")]
        [Tooltip("Referencja do HandControl")]
        public HandControl handControl;

        [Header("Position Settings")]
        [Tooltip("Maksymalna liczba zapisanych pozycji")]
        public int maxPositions = 10;

        [Header("Movement Settings")]
        [Tooltip("Szybkość przywracania pozycji")]
        public float restoreRotationStep = 2f;
        public float restoreRotationTickInterval = 0.05f;
        public float restoreHeightStep = 0.001f;
        public float restoreHeightTickInterval = 0.05f;
        public float restoreLongitudinalStep = 0.001f;
        public float restoreLongitudinalTickInterval = 0.05f;

        [Header("Predefined Positions")]
        [Tooltip("Czy pozycja 1 jest zablokowana (predefiniowana)")]
        public bool lockFirstPosition = true;

        [Header("Height Control")]
        [Tooltip("Czy ignorować wysokość przy zapisywaniu/przywracaniu")]
        public bool ignoreHeight = false;

        private List<TablePosition> savedPositions = new List<TablePosition>();
        private bool isRestoring = false;

        private void Start()
        {
            // Inicjalizacja listy pozycji
            for (int i = 0; i < maxPositions; i++)
            {
                savedPositions.Add(new TablePosition(""));
            }

            // Sprawdź czy HandControl jest przypisany
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl nie jest przypisany!");
            }

            // Ustaw predefiniowaną pozycję 1
            if (lockFirstPosition)
            {
                SetupPredefinedPosition1();
            }
        }

        private TablePosition SetupLevel0Position()
        {
            TablePosition pos = new TablePosition("Level Zero");
            
            // Wszystkie kąty na zero
            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 0f;
            pos.backUpperAngleZ = 0f;
            pos.backLowerAngleX = 0f;
            pos.backLowerAngleY = 0f;
            pos.backLowerAngleZ = 0f;
            pos.backLowerLeftAngleX = 0f;
            pos.backLowerLeftAngleY = 0f;
            pos.backLowerLeftAngleZ = 0f;
            pos.backLowerRightAngleX = 0f;
            pos.backLowerRightAngleY = 0f;
            pos.backLowerRightAngleZ = 0f;
            pos.tableRotationAngleX = 0f;
            pos.tableRotationAngleY = 0f;
            pos.tableRotationAngleZ = 0f;
            pos.tableHeight = 0f;
            pos.tableLongitudinalX = 0f;
            pos.isReversed = false;

            Debug.Log("[TablePositionManager] Ustawiono pozycję " + name);
            return pos;
        }

        private TablePosition SetupFlexPosition()
        {
            TablePosition pos = new TablePosition("Flex");
            
            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = -40f;
            pos.backUpperAngleZ = 0f;
            pos.backLowerAngleX = 0f;
            pos.backLowerAngleY = 0f;
            pos.backLowerAngleZ = 0f;
            pos.backLowerLeftAngleX = 0f;
            pos.backLowerLeftAngleY = 0f;
            pos.backLowerLeftAngleZ = 0f;
            pos.backLowerRightAngleX = 0f;
            pos.backLowerRightAngleY = 0f;
            pos.backLowerRightAngleZ = 0f;
            pos.tableRotationAngleX = 0f;
            pos.tableRotationAngleY = 0f;
            pos.tableRotationAngleZ = -20f;
            pos.tableHeight = 0f;
            pos.tableLongitudinalX = 0f;
            pos.isReversed = false;

            Debug.Log("[TablePositionManager] Ustawiono pozycję " + pos.name);
            return pos;
        }

        private TablePosition SetupReflexPosition()
        {
            TablePosition pos = new TablePosition("Reflex");
            
            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 70f;
            pos.backUpperAngleZ = 0f;
            pos.backLowerAngleX = 0f;
            pos.backLowerAngleY = 0f;
            pos.backLowerAngleZ = 0f;
            pos.backLowerLeftAngleX = 0f;
            pos.backLowerLeftAngleY = 0f;
            pos.backLowerLeftAngleZ = 0f;
            pos.backLowerRightAngleX = 0f;
            pos.backLowerRightAngleY = 0f;
            pos.backLowerRightAngleZ = 0f;
            pos.tableRotationAngleX = 0f;
            pos.tableRotationAngleY = 0f;
            pos.tableRotationAngleZ = 30f;
            pos.tableHeight = 0f;
            pos.tableLongitudinalX = 0f;
            pos.isReversed = false;

            Debug.Log("[TablePositionManager] Ustawiono pozycję " + pos.name);
            return pos;
        }

        private void SetupPredefinedPosition1()
        {
            TablePosition pos = new TablePosition("Beach Chair");
            
            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 80f;
            pos.backUpperAngleZ = 0f;
            pos.backLowerAngleX = 0f;
            pos.backLowerAngleY = 70f;
            pos.backLowerAngleZ = 0f;
            pos.backLowerLeftAngleX = 0f;
            pos.backLowerLeftAngleY = 0f;
            pos.backLowerLeftAngleZ = 0f;
            pos.backLowerRightAngleX = 0f;
            pos.backLowerRightAngleY = 0f;
            pos.backLowerRightAngleZ = 0f;
            pos.tableRotationAngleX = 0f;
            pos.tableRotationAngleY = 0f;
            pos.tableRotationAngleZ = 25f;
            pos.tableHeight = 0f;
            pos.tableLongitudinalX = 0f;
            pos.isReversed = false;

            savedPositions[0] = pos;

            Debug.Log("[TablePositionManager] Ustawiono predefiniowaną pozycję 1");
        }

        // ============================================================
        // PUBLIC API
        // ============================================================

        /// <summary>
        /// Zapisuje aktualną pozycję stołu do określonego slotu
        /// </summary>
        public void SavePosition(int slotIndex)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl nie jest przypisany!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Stół wyłączony - nie można zapisać pozycji");
                return;
            }

            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Nieprawidłowy indeks slotu: " + slotIndex);
                return;
            }

            // Blokada zapisywania pozycji 1 (indeks 0)
            if (lockFirstPosition && slotIndex == 0)
            {
                Debug.LogWarning("[TablePositionManager] Pozycja 1 jest zablokowana - nie można jej nadpisać");
                return;
            }

            TablePosition pos = new TablePosition("Pozycja " + (slotIndex + 1));

            // Zapisz kąty rotacji
            if (handControl.tableBackPartUpperRotate != null)
            {
                pos.backUpperAngleX = handControl.tableBackPartUpperRotate.currentAngleX;
                pos.backUpperAngleY = handControl.tableBackPartUpperRotate.currentAngleY;
                pos.backUpperAngleZ = handControl.tableBackPartUpperRotate.currentAngleZ;
            }

            if (handControl.tableBackPartLowerRotate != null)
            {
                pos.backLowerAngleX = handControl.tableBackPartLowerRotate.currentAngleX;
                pos.backLowerAngleY = handControl.tableBackPartLowerRotate.currentAngleY;
                pos.backLowerAngleZ = handControl.tableBackPartLowerRotate.currentAngleZ;
            }

            if (handControl.tableBackPartLowerLeftRotate != null)
            {
                pos.backLowerLeftAngleX = handControl.tableBackPartLowerLeftRotate.currentAngleX;
                pos.backLowerLeftAngleY = handControl.tableBackPartLowerLeftRotate.currentAngleY;
                pos.backLowerLeftAngleZ = handControl.tableBackPartLowerLeftRotate.currentAngleZ;
            }

            if (handControl.tableBackPartLowerRightRotate != null)
            {
                pos.backLowerRightAngleX = handControl.tableBackPartLowerRightRotate.currentAngleX;
                pos.backLowerRightAngleY = handControl.tableBackPartLowerRightRotate.currentAngleY;
                pos.backLowerRightAngleZ = handControl.tableBackPartLowerRightRotate.currentAngleZ;
            }

            if (handControl.tableRotation != null)
            {
                pos.tableRotationAngleX = handControl.tableRotation.currentAngleX;
                pos.tableRotationAngleY = handControl.tableRotation.currentAngleY;
                pos.tableRotationAngleZ = handControl.tableRotation.currentAngleZ;
            }

            // Wysokość - zapisz tylko jeśli nie jest ignorowana
            if (!ignoreHeight && handControl.tableHeightControl != null)
            {
                pos.tableHeight = GetTelescopicPosition(handControl.tableHeightControl);
                Debug.Log("[TablePositionManager] ZAPISYWANIE - Wysokość telescopic: " + pos.tableHeight);
            }
            else
            {
                pos.tableHeight = 0f; // Ignoruj wysokość
            }

            // Zapisz pozycję wzdłużną
            if (handControl.tableLongitudinalControl != null)
            {
                pos.tableLongitudinalX = handControl.tableLongitudinalControl.currentPositionX;
            }

            // Zapisz stan reverse
            pos.isReversed = handControl.IsReversed;

            savedPositions[slotIndex] = pos;

            Debug.Log("[TablePositionManager] Zapisano pozycję do slotu " + (slotIndex + 1) + ": " + pos.name);
        }

        /// <summary>
        /// Przywraca pozycję stołu z określonego slotu
        /// </summary>
        public void LoadPosition(int slotIndex)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl nie jest przypisany!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Stół wyłączony - nie można załadować pozycji");
                return;
            }

            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Nieprawidłowy indeks slotu: " + slotIndex);
                return;
            }

            TablePosition pos = savedPositions[slotIndex];

            if (pos.IsEmpty())
            {
                Debug.LogWarning("[TablePositionManager] Slot " + (slotIndex + 1) + " jest pusty");
                return;
            }

            if (isRestoring)
            {
                Debug.LogWarning("[TablePositionManager] Trwa już przywracanie pozycji");
                return;
            }

            Debug.Log("[TablePositionManager] Ładowanie pozycji ze slotu " + (slotIndex + 1) + ": " + pos.name);
            StartCoroutine(LoadPositionCoroutine(pos));
        }

        public void RestorePredefinedPosition(PredefinedPositionType positionType)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl nie jest przypisany!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Stół wyłączony - nie można ustawić pozycji predefiniowanej");
                return;
            }

            TablePosition pos = null;

            switch (positionType)
            {
                case PredefinedPositionType.Level0:
                    pos = SetupLevel0Position();
                    break;
                case PredefinedPositionType.Flex:
                    pos = SetupFlexPosition();
                    break;
                case PredefinedPositionType.Reflex:
                    pos = SetupReflexPosition();
                    break;
                default:
                    Debug.LogWarning("[TablePositionManager] Nieznany typ pozycji predefiniowanej");
                    return;
            }

            Debug.Log("[TablePositionManager] Przywracanie pozycji predefiniowanej: " + pos.name);
            StartCoroutine(LoadPositionCoroutine(pos));
        }
        /// <summary>
        /// Usuwa zapisaną pozycję z określonego slotu
        /// </summary>
        public void ClearPosition(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Nieprawidłowy indeks slotu: " + slotIndex);
                return;
            }

            savedPositions[slotIndex] = new TablePosition("");
            Debug.Log("[TablePositionManager] Wyczyszczono slot " + (slotIndex + 1));
        }

        /// <summary>
        /// Czyści wszystkie zapisane pozycje
        /// </summary>
        public void ClearAllPositions()
        {
            for (int i = 0; i < maxPositions; i++)
            {
                savedPositions[i] = new TablePosition("");
            }
            Debug.Log("[TablePositionManager] Wyczyszczono wszystkie pozycje");
        }

        /// <summary>
        /// Sprawdza czy slot zawiera zapisaną pozycję
        /// </summary>
        public bool IsSlotOccupied(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return false;

            return !savedPositions[slotIndex].IsEmpty();
        }

        /// <summary>
        /// Zwraca nazwę pozycji w danym slocie
        /// </summary>
        public string GetPositionName(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return "";

            return savedPositions[slotIndex].name;
        }

        /// <summary>
        /// Ustawia niestandardową nazwę dla pozycji
        /// </summary>
        public void SetPositionName(int slotIndex, string customName)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Nieprawidłowy indeks slotu: " + slotIndex);
                return;
            }

            if (!savedPositions[slotIndex].IsEmpty())
            {
                savedPositions[slotIndex].name = customName;
                Debug.Log("[TablePositionManager] Zmieniono nazwę slotu " + (slotIndex + 1) + " na: " + customName);
            }
        }

        /// <summary>
        /// Zwraca szczegóły pozycji jako string (do debugowania)
        /// </summary>
        public string GetPositionDetails(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return "Nieprawidłowy slot";

            TablePosition pos = savedPositions[slotIndex];
            if (pos.IsEmpty())
                return "Pusty slot";

            return pos.name + "\n" +
                   "Wysokość: " + pos.tableHeight.ToString("F4") + "\n" +
                   "Pozycja wzdłużna: " + pos.tableLongitudinalX.ToString("F4") + "\n" +
                   "Reverse: " + pos.isReversed;
        }

        // ============================================================
        // PRIVATE METHODS - RESTORE LOGIC
        // ============================================================

        private IEnumerator LoadPositionCoroutine(TablePosition pos)
        {
            isRestoring = true;

            Debug.Log("[TablePositionManager] Rozpoczynam przywracanie pozycji: " + pos.name);

            // Zatrzymaj wszystkie ruchy
            handControl.StopAllMovement();

            // Najpierw ustaw reverse jeśli potrzeba
            if (pos.isReversed != handControl.IsReversed)
            {
                Debug.Log("[TablePositionManager] Zmiana reverse na: " + pos.isReversed);
                if (pos.isReversed)
                    handControl.ReversePosition();
                else
                    handControl.NormalPosition();

                // Poczekaj na zakończenie obrotu
                yield return new WaitForSeconds(0.5f);
            }

            // Zresetuj licznik
            runningCoroutines = 0;

            // Przywróć wszystkie rotacje i pozycje
            if (handControl.tableBackPartUpperRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartUpperRotate, pos.backUpperAngleX, pos.backUpperAngleY, pos.backUpperAngleZ));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie: tableBackPartUpperRotate");
            }

            if (handControl.tableBackPartLowerRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerRotate, pos.backLowerAngleX, pos.backLowerAngleY, pos.backLowerAngleZ));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie: tableBackPartLowerRotate");
            }

            if (handControl.tableBackPartLowerLeftRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerLeftRotate, pos.backLowerLeftAngleX, pos.backLowerLeftAngleY, pos.backLowerLeftAngleZ));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie: tableBackPartLowerLeftRotate");
            }

            if (handControl.tableBackPartLowerRightRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerRightRotate, pos.backLowerRightAngleX, pos.backLowerRightAngleY, pos.backLowerRightAngleZ));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie: tableBackPartLowerRightRotate");
            }

            if (handControl.tableRotation != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableRotation, pos.tableRotationAngleX, pos.tableRotationAngleY, pos.tableRotationAngleZ));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie: tableRotation");
            }

            // Wysokość - przywróć tylko jeśli nie jest ignorowana
            if (!ignoreHeight && handControl.tableHeightControl != null)
            {
                runningCoroutines++;
                StartCoroutine(RestoreHeightCoroutineTracked(pos.tableHeight));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie wysokości: " + pos.tableHeight);
            }
            else
            {
                Debug.Log("[TablePositionManager] Pomijam przywracanie wysokości (ignoreHeight = " + ignoreHeight + ")");
            }

            if (handControl.tableLongitudinalControl != null)
            {
                runningCoroutines++;
                StartCoroutine(RestoreLongitudinalCoroutineTracked(pos.tableLongitudinalX));
                Debug.Log("[TablePositionManager] Uruchomiono przywracanie pozycji wzdłużnej: " + pos.tableLongitudinalX);
            }

            Debug.Log("[TablePositionManager] Uruchomiono " + runningCoroutines + " operacji przywracania");

            // Poczekaj aż wszystkie się skończą
            float maxWaitTime = 30f;
            float waitedTime = 0f;
            while (runningCoroutines > 0 && waitedTime < maxWaitTime)
            {
                yield return new WaitForSeconds(0.1f);
                waitedTime += 0.1f;
            }

            if (runningCoroutines > 0)
            {
                Debug.LogWarning("[TablePositionManager] Timeout - nie wszystkie operacje się zakończyły. Pozostało: " + runningCoroutines);
            }

            isRestoring = false;
            Debug.Log("[TablePositionManager] Załadowano pozycję: " + pos.name);
        }

        private int runningCoroutines = 0;

        private IEnumerator RestorePivotCoroutineTracked(RotationPivot pivot, float targetX, float targetY, float targetZ)
        {
            yield return StartCoroutine(RestorePivotCoroutine(pivot, targetX, targetY, targetZ));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Pivot zakończony. Pozostało: " + runningCoroutines);
        }

        private IEnumerator RestoreHeightCoroutineTracked(float targetHeight)
        {
            yield return StartCoroutine(RestoreHeightCoroutine(targetHeight));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Wysokość zakończona. Pozostało: " + runningCoroutines);
        }

        private IEnumerator RestoreLongitudinalCoroutineTracked(float targetX)
        {
            yield return StartCoroutine(RestoreLongitudinalCoroutine(targetX));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Pozycja wzdłużna zakończona. Pozostało: " + runningCoroutines);
        }

        private IEnumerator RestorePivotCoroutine(RotationPivot pivot, float targetX, float targetY, float targetZ)
        {
            if (pivot == null)
                yield break;

            // Przywróć każdą oś oddzielnie
            if (pivot.allowX && Mathf.Abs(pivot.currentAngleX - targetX) > 0.01f)
            {
                yield return RestoreAxisCoroutine(pivot, Vector3.right, targetX);
            }

            if (pivot.allowY && Mathf.Abs(pivot.currentAngleY - targetY) > 0.01f)
            {
                yield return RestoreAxisCoroutine(pivot, Vector3.up, targetY);
            }

            if (pivot.allowZ && Mathf.Abs(pivot.currentAngleZ - targetZ) > 0.01f)
            {
                yield return RestoreAxisCoroutine(pivot, Vector3.forward, targetZ);
            }
        }

        private IEnumerator RestoreAxisCoroutine(RotationPivot pivot, Vector3 axis, float targetAngle)
        {
            char detectedAxis = DetectAxisFromVector(axis);
            float currentAngle = pivot.GetCurrentAngle(detectedAxis);

            while (Mathf.Abs(currentAngle - targetAngle) > 0.01f)
            {
                float diff = targetAngle - currentAngle;
                int direction = diff > 0 ? 1 : -1;
                float delta = restoreRotationStep * direction;

                if (Mathf.Abs(delta) > Mathf.Abs(diff))
                {
                    delta = diff;
                }

                pivot.RotateWithVector3(axis, delta);
                currentAngle = pivot.GetCurrentAngle(detectedAxis);

                yield return new WaitForSeconds(restoreRotationTickInterval);
            }
        }

        private IEnumerator RestoreHeightCoroutine(float targetHeight)
        {
            if (handControl.tableHeightControl == null)
            {
                Debug.LogWarning("[TablePositionManager] tableHeightControl jest null!");
                yield break;
            }

            float currentHeight = GetTelescopicPosition(handControl.tableHeightControl);
            Debug.Log("[TablePositionManager] Przywracanie wysokości - Start: " + currentHeight + " -> Cel: " + targetHeight);

            int iterations = 0;
            int maxIterations = 10000; // zabezpieczenie przed nieskończoną pętlą

            while (Mathf.Abs(currentHeight - targetHeight) > 0.001f && iterations < maxIterations)
            {
                float diff = targetHeight - currentHeight;
                int direction = diff > 0 ? 1 : -1;
                float delta = restoreHeightStep * direction;

                if (Mathf.Abs(delta) > Mathf.Abs(diff))
                {
                    delta = diff;
                }

                bool canContinue = handControl.tableHeightControl.Move(delta);
                if (!canContinue)
                {
                    Debug.LogWarning("[TablePositionManager] Wysokość - nie może kontynuować ruchu. Aktualna: " + currentHeight);
                    break;
                }

                currentHeight = GetTelescopicPosition(handControl.tableHeightControl);
                iterations++;

                yield return new WaitForSeconds(restoreHeightTickInterval);
            }

            if (iterations >= maxIterations)
            {
                Debug.LogWarning("[TablePositionManager] Wysokość - przekroczono maksymalną liczbę iteracji!");
            }

            Debug.Log("[TablePositionManager] Przywracanie wysokości - Koniec: " + currentHeight + " (iteracji: " + iterations + ")");
        }

        private float GetTelescopicPosition(TelescopicMovement telescopic)
        {
            if (telescopic == null)
            {
                Debug.LogWarning("[TablePositionManager] GetTelescopicPosition - telescopic jest null!");
                return 0f;
            }

            if (telescopic.sections == null || telescopic.sections.Length == 0)
            {
                Debug.LogWarning("[TablePositionManager] GetTelescopicPosition - brak sekcji!");
                return 0f;
            }

            float totalPosition = 0f;
            char axis = DetectAxisFromVector(telescopic.movementAxis);

            Debug.Log("[TablePositionManager] GetTelescopicPosition - oś: " + axis + ", sekcji: " + telescopic.sections.Length);

            foreach (TelescopicMovement.TelescopicSection section in telescopic.sections)
            {
                if (section.movementAxis != null)
                {
                    float sectionPos = 0f;
                    switch (axis)
                    {
                        case 'X':
                            sectionPos = section.movementAxis.currentPositionX;
                            break;
                        case 'Y':
                            sectionPos = section.movementAxis.currentPositionY;
                            break;
                        case 'Z':
                            sectionPos = section.movementAxis.currentPositionZ;
                            break;
                    }
                    totalPosition += sectionPos;
                    Debug.Log("[TablePositionManager] Sekcja: " + section.sectionName + " = " + sectionPos);
                }
            }

            Debug.Log("[TablePositionManager] GetTelescopicPosition - suma: " + totalPosition);
            return totalPosition;
        }

        private IEnumerator RestoreLongitudinalCoroutine(float targetX)
        {
            if (handControl.tableLongitudinalControl == null)
                yield break;

            float currentX = handControl.tableLongitudinalControl.currentPositionX;

            while (Mathf.Abs(currentX - targetX) > 0.001f)
            {
                float diff = targetX - currentX;
                int direction = diff > 0 ? 1 : -1;
                float delta = Mathf.Min(Mathf.Abs(diff), restoreLongitudinalStep) * direction;

                bool canContinue = handControl.tableLongitudinalControl.MoveWithVector3(Vector3.right, delta);
                if (!canContinue)
                {
                    Debug.LogWarning("[TablePositionManager] Longitudinal - limit");
                    break;
                }

                currentX = handControl.tableLongitudinalControl.currentPositionX;
                yield return new WaitForSeconds(restoreLongitudinalTickInterval);
            }

            Debug.Log("[TablePositionManager] Longitudinal restored to: " + currentX);
        }

        // ============================================================
        // HELPERS
        // ============================================================

        private char DetectAxisFromVector(Vector3 axis)
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

        // ============================================================
        // GETTERS
        // ============================================================

        public bool IsRestoring
        {
            get { return isRestoring; }
        }

        public int MaxPositions
        {
            get { return maxPositions; }
        }

        public int OccupiedSlotsCount
        {
            get
            {
                int count = 0;
                foreach (TablePosition pos in savedPositions)
                {
                    if (!pos.IsEmpty())
                        count++;
                }
                return count;
            }
        }
    }
}