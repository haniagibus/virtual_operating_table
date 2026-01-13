using UnityEngine;
using System.Collections;

namespace VirtualOperatingTable
{
    public class HandControl : MonoBehaviour
    {
        [Header("Table Position Manager")]
        public TablePositionManager tablePositionManager;

        [Header("Table Components - Rotation")]
        public RotationPivot tableBackPartUpperRotate;
        public RotationPivot tableBackPartLowerRotate;
        public RotationPivot tableBackPartLowerLeftRotate;
        public RotationPivot tableBackPartLowerRightRotate;

        public RotationPivot tableRotation;

        [Header("Reverse Position")]
        [Tooltip("Główny transform blatu stołu do obrotu")]
        public Transform tableTop;

        private bool isReversed = false;
        private Coroutine reverseCoroutine = null;

        [Header("Table Telescopic Movement")]
        [Tooltip("Teleskopowy mechanizm podnoszenia stołu")]
        public TelescopicMovement tableHeightControl;

        // [Tooltip("Element blatu do przenoszenia podczas ruchu wysokości")]
        // public Transform tableTopElement;

        // [Tooltip("Nazwa obiektu górnej sekcji nogi (do przyłączenia podczas ruchu)")]
        // public string topLegSectionName = "table_leg_column_segment_4_move";

        // [Tooltip("Nazwa obiektu głównego stołu (do przyłączenia po zakończeniu ruchu)")]
        // public string mainTableName = "table";

        [Header("Table Longitudinal Movement")]
        [Tooltip("Mechanizm przesuwania stołu wzdłużnie")]
        public MovementAxis tableLongitudinalControl;

        [Header("Blend Shapes")]
        [Tooltip("Kontroler blend shapes (opcjonalny)")]
        public TableBlendShapeController blendShapeController;

        [Header("Rotation Settings")]
        public float rotationStep = 2f;
        public float rotationTickInterval = 0.05f;

        [Header("Height Movement Settings")]
        public float heightStep = 0.00005f;
        public float heightTickInterval = 0.05f;

        [Header("Longitudinal Movement Settings")]
        public float longitudinalStep = 0.00005f;
        public float longitudinalTickInterval = 0.05f;

        private Coroutine currentTiltCoroutine = null;
        private bool isTilting = false;

        private Coroutine currentHeightCoroutine = null;
        private bool isMovingHeight = false;

        private Coroutine currentLongitudinalCoroutine = null;
        private bool isMovingLongitudinal = false;

        public bool isLocked = false;
        public LegSelection currentLeg = LegSelection.Both;

        // ============================================================
        // ROTATION
        // ============================================================

        public void TiltBackPlate(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Podnoszę górną część stołu");
            StartTiltElement(tableBackPartUpperRotate, Vector3.forward, direction, false);
        }

        public void TiltLegsPlate(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Podnoszę dolną część stołu");
            if (currentLeg == LegSelection.Both)
            {
                StartTiltElement(tableBackPartLowerRotate, Vector3.forward, direction, false);
            }
            else if (currentLeg == LegSelection.Left)
            {
                StartTiltElement(tableBackPartLowerLeftRotate, Vector3.forward, direction, false);
            }
            else if (currentLeg == LegSelection.Right)
            {
                StartTiltElement(tableBackPartLowerRightRotate, Vector3.forward, direction, false);
            }
        }

        public void LeftLegSelected()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony");
                return;
            }

            if (currentLeg != LegSelection.Left)
            {
                currentLeg = LegSelection.Left;
                Debug.Log("[HandControl] Sterowanie: lewa noga");
            }
            else
            {
                currentLeg = LegSelection.Both;
                Debug.Log("[HandControl] Sterowanie: obie nogi");
            }
        }

        public void RightLegSelected()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            if (currentLeg != LegSelection.Right)
            {
                currentLeg = LegSelection.Right;
                Debug.Log("[HandControl] Sterowanie: prawa noga");
            }
            else
            {
                currentLeg = LegSelection.Both;
                Debug.Log("[HandControl] Sterowanie: obie nogi");
            }
        }

        // TRENDELENBURG POSITION - Z BLEND SHAPES
        public void TiltTrendelenburg(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Pozycja Trendelenburga");
            StartTiltElement(tableRotation, Vector3.forward, direction, true);
        }

        // LATERAL TILT - Z BLEND SHAPES
        public void TiltTable(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Przechylam stół");
            StartTiltElement(tableRotation, Vector3.right, direction, true);
        }

        // ============================================================
        // MOVEMENT
        // ============================================================

        public void ChangeTableHeight(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Zmieniam wysokość stołu");
            StartHeightMovement(direction);
        }

        public void MoveTable(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Przesuwam stół");
            StartLongitudinalMovement(Vector3.right, direction);
        }

        // ============================================================
        // STOP ALL MOVEMENT
        // ============================================================
        public void StopAllMovement()
        {
            isTilting = false;
            if (currentTiltCoroutine != null)
            {
                StopCoroutine(currentTiltCoroutine);
                currentTiltCoroutine = null;
            }

            isMovingHeight = false;
            if (currentHeightCoroutine != null)
            {
                StopCoroutine(currentHeightCoroutine);
                currentHeightCoroutine = null;
            }

            isMovingLongitudinal = false;
            if (currentLongitudinalCoroutine != null)
            {
                StopCoroutine(currentLongitudinalCoroutine);
                currentLongitudinalCoroutine = null;
            }

            Debug.Log("[HandControl] Zatrzymuję ruch stołu");
        }

        // ============================================================
        // LOCK / UNLOCK
        // ============================================================
        public void PowerOnOff()
        {
            isLocked = !isLocked;
        }

        // ============================================================
        // LEVEL ZERO
        // ============================================================
        public void LevelZero()
        {
            if (isLocked)
            {
                Debug.Log("[HandControl] Stół wyłączony - nie można ustawić Level Zero");
                return;
            }

            if (tablePositionManager == null)
            {
                Debug.LogError("[HandControl] TablePositionManager nie jest przypisany!");
                return;
            }

            tablePositionManager.RestorePredefinedPosition(PredefinedPositionType.Level0);
        }

        // ============================================================
        // NORMAL / REVERSE
        // ============================================================
        public void NormalPosition()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }
            Debug.Log("[HandControl] Ustawiam stół w pozycji normalnej");
            isReversed = false;
            reverseCoroutine = StartCoroutine(RotateToReverseCoroutine(isReversed));
        }

        public void ReversePosition()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }
            Debug.Log("[HandControl] Ustawiam stół w pozycji reverse");

            isReversed = true;
            reverseCoroutine = StartCoroutine(RotateToReverseCoroutine(isReversed));
        }

        // ============================================================
        // INTERNAL LOGIC
        // ============================================================

        // ROTATION
        private void StartTiltElement(RotationPivot pivot, Vector3 axis, int direction, bool useBlendShapes)
        {
            if (pivot == null)
            {
                Debug.LogError("[HandControl] Pivot nie jest przypisany!");
                return;
            }

            if (currentTiltCoroutine != null)
            {
                StopCoroutine(currentTiltCoroutine);
            }

            isTilting = true;
            currentTiltCoroutine = StartCoroutine(TiltElementCoroutine(pivot, axis, direction, useBlendShapes));
        }

        private IEnumerator TiltElementCoroutine(RotationPivot pivot, Vector3 axis, int direction, bool useBlendShapes)
        {
            if (pivot == null)
            {
                Debug.LogError("[HandControl] Pivot jest null!");
                yield break;
            }

            while (isTilting)
            {
                float delta = rotationStep * direction;
                bool canContinue = pivot.RotateWithVector3(axis, delta);

                char detectedAxis = DetectAxisFromVector(axis);

                float angle = pivot.GetCurrentAngle(detectedAxis);

                Debug.Log("[BlendShape] Axis=" + detectedAxis + " angle=" + angle);

                // Zaktualizuj blend shapes jeśli są włączone
                if (useBlendShapes && blendShapeController != null)
                {
                    UpdateBlendShapes(pivot, axis);
                }

                if (!canContinue)
                {
                    Debug.Log("[HandControl] Osiągnięto limit rotacji");
                    isTilting = false;
                    break;
                }

                yield return new WaitForSeconds(rotationTickInterval);
            }

            currentTiltCoroutine = null;
        }

        private void UpdateBlendShapes(RotationPivot pivot, Vector3 axis)
        {
            if (blendShapeController == null)
                return;

            char detectedAxis = DetectAxisFromVector(axis);

            float angle = pivot.GetCurrentAngle(detectedAxis);

            Debug.Log("[BlendShape] Axis=" + detectedAxis + " angle=" + angle);

            if (detectedAxis == 'X') // LATERAL (Left / Right)
            {
                blendShapeController.UpdateLateral(angle);
            }
            else if (detectedAxis == 'Z') // TRENDELENBURG (Forward / Backward)
            {
                blendShapeController.UpdateTrendelenburg(angle);
            }
        }

        // TELESCOPIC MOVEMENT
        private void StartHeightMovement(int direction)
        {
            if (tableHeightControl == null)
            {
                Debug.LogError("[HandControl] TelescopicMovement nie jest przypisany!");
                return;
            }

            if (currentHeightCoroutine != null)
            {
                StopCoroutine(currentHeightCoroutine);
            }

            isMovingHeight = true;
            currentHeightCoroutine = StartCoroutine(HeightMovementCoroutine(direction));
        }

        private IEnumerator HeightMovementCoroutine(int direction)
        {
            while (isMovingHeight)
            {
                float delta = heightStep * direction;
                bool canContinue = tableHeightControl.Move(delta);

                if (!canContinue)
                {
                    Debug.Log("[HandControl] Teleskop osiągnął limit");
                    isMovingHeight = false;
                    break;
                }

                yield return new WaitForSeconds(heightTickInterval);
            }

            currentHeightCoroutine = null;
        }

        // LONGITUDINAL MOVEMENT
        private void StartLongitudinalMovement(Vector3 axis, int direction)
        {
            if (tableLongitudinalControl == null)
            {
                Debug.LogError("[HandControl] MovementAxis dla ruchu wzdłużnego nie jest przypisany!");
                return;
            }

            if (currentLongitudinalCoroutine != null)
            {
                StopCoroutine(currentLongitudinalCoroutine);
            }

            isMovingLongitudinal = true;
            currentLongitudinalCoroutine = StartCoroutine(LongitudinalMovementCoroutine(axis, direction));
        }

        private IEnumerator LongitudinalMovementCoroutine(Vector3 axis, int direction)
        {
            while (isMovingLongitudinal)
            {
                float delta = longitudinalStep * direction;
                bool canContinue = tableLongitudinalControl.MoveWithVector3(axis, delta);

                if (!canContinue)
                {
                    Debug.Log("[HandControl] Osiągnięto limit przesuwu wzdłużnego");
                    isMovingLongitudinal = false;
                    break;
                }

                yield return new WaitForSeconds(longitudinalTickInterval);
            }

            currentLongitudinalCoroutine = null;
        }

        private IEnumerator RotateToReverseCoroutine(bool reverse)
        {
            if (tableTop == null)
            {
                Debug.LogError("[HandControl] TableTop nie jest przypisany!");
                yield break;
            }

            float targetAngle = reverse ? 180f : 0f;

            Vector3 euler = tableTop.localEulerAngles;
            euler.y = targetAngle;
            tableTop.localEulerAngles = euler;

            reverseCoroutine = null;
        }

        // HELPERS
        // private void DetachFromParent(Transform element)
        // {
        //     if (element == null)
        //     {
        //         Debug.LogWarning("[HandControl] Element do odłączenia jest null!");
        //         return;
        //     }

        //     if (element.parent != null)
        //     {
        //         Vector3 worldPosition = element.position;
        //         Quaternion worldRotation = element.rotation;

        //         element.SetParent(null);

        //         element.position = worldPosition;
        //         element.rotation = worldRotation;

        //         Debug.Log("[HandControl] Element " + element.name + " odłączony od parenta");
        //     }
        // }

        // private void AttachToParent(Transform element, string parentName)
        // {
        //     if (element == null)
        //     {
        //         Debug.LogWarning("[HandControl] Element do przyłączenia jest null!");
        //         return;
        //     }

        //     GameObject parent = GameObject.Find(parentName);

        //     if (parent == null)
        //     {
        //         Debug.LogWarning("[HandControl] Nie znaleziono obiektu " + parentName);
        //         return;
        //     }

        //     Vector3 worldPosition = element.position;
        //     Quaternion worldRotation = element.rotation;

        //     element.SetParent(parent.transform);

        //     element.position = worldPosition;
        //     element.rotation = worldRotation;

        //     Debug.Log("[HandControl] Element " + element.name + " przypięty do " + parentName);
        // }

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

        private float GetLongitudinalCurrentPosition()
        {
            if (tableLongitudinalControl == null)
                return 0f;

            return tableLongitudinalControl.currentPositionX;
        }

        // GETTERS
        public bool IsTilting
        {
            get { return isTilting; }
        }

        public bool IsMovingHeight
        {
            get { return isMovingHeight; }
        }

        public bool IsMovingLongitudinal
        {
            get { return isMovingLongitudinal; }
        }

        public bool IsReversed
        {
            get { return isReversed; }
        }
    }
}