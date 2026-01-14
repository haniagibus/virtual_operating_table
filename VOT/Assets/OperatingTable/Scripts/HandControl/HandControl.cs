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

        public TableState tableState = TableState.Normal;

        [Header("Table Telescopic Movement")]
        public TelescopicMovement tableHeightControl;

        [Header("Table Longitudinal Movement")]
        public MovementAxis tableLongitudinalControl;

        [Header("Blend Shapes")]
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
        public TableState NormalPosition()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return tableState;
            }
            Debug.Log("[HandControl] Ustawiam stół w pozycji normalnej");
            tableState = TableState.Normal;
            return tableState;

        }

        public TableState ReversePosition()
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return tableState;
            }
            Debug.Log("[HandControl] Ustawiam stół w pozycji reverse");
            tableState = TableState.Reverse;
            return tableState;

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

            if (detectedAxis == 'X') 
            {
                blendShapeController.UpdateLateral(angle);
            }
            else if (detectedAxis == 'Z') 
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
       
    }
}