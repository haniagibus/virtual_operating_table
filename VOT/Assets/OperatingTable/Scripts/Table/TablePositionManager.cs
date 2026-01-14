using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace VirtualOperatingTable
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
        public TableState tableState;

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
        public HandControl handControl;

        [Header("Position Settings")]
        public int maxPositions = 10;

        [Header("Movement Settings")]
        public float restoreRotationStep = 2f;
        public float restoreRotationTickInterval = 0.05f;
        public float restoreHeightStep = 0.001f;
        public float restoreHeightTickInterval = 0.05f;
        public float restoreLongitudinalStep = 0.001f;
        public float restoreLongitudinalTickInterval = 0.05f;

        [Header("Predefined Positions")]
        public bool lockFirstPosition = true;

        [Header("Height Control")]
        public bool ignoreHeight = false;

        [Header("BlendShape Controller")]
        public TableBlendShapeController blendShapeController;

        private List<TablePosition> savedPositions = new List<TablePosition>();
        private bool isRestoring = false;

        private void Start()
        {
            for (int i = 0; i < maxPositions; i++)
            {
                savedPositions.Add(new TablePosition(""));
            }

            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl is not assigned!");
            }

            if (lockFirstPosition)
            {
                SetupPredefinedPosition1();
            }
        }

        private TablePosition SetupLevel0Position()
        {
            TablePosition pos = new TablePosition("Level Zero");

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
            pos.tableState = TableState.Normal;

            Debug.Log("[TablePositionManager] Set position " + name);
            return pos;
        }

        private TablePosition SetupFlexPosition()
        {
            TablePosition pos = new TablePosition("Flex");

            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 0f;
            pos.backUpperAngleZ = 40f;
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
            pos.tableState = TableState.Normal;

            Debug.Log("[TablePositionManager] Set position " + pos.name);
            return pos;
        }

        private TablePosition SetupReflexPosition()
        {
            TablePosition pos = new TablePosition("Reflex");

            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 0f;
            pos.backUpperAngleZ = -70f;
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
            pos.tableState = TableState.Normal;

            Debug.Log("[TablePositionManager] Set position " + pos.name);
            return pos;
        }

        private void SetupPredefinedPosition1()
        {
            TablePosition pos = new TablePosition("Beach Chair");

            pos.backUpperAngleX = 0f;
            pos.backUpperAngleY = 0f;
            pos.backUpperAngleZ = -80f;
            pos.backLowerAngleX = 0f;
            pos.backLowerAngleY = 0f;
            pos.backLowerAngleZ = -70f;
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
            pos.tableState = TableState.Normal;

            savedPositions[0] = pos;

            Debug.Log("[TablePositionManager] Set predefined position 1");
        }

        public void SavePosition(int slotIndex)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl is not assigned!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Table is locked - cannot save position");
                return;
            }

            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Invalid slot index: " + slotIndex);
                return;
            }

            if (lockFirstPosition && slotIndex == 0)
            {
                Debug.LogWarning("[TablePositionManager] Position 1 is locked - cannot overwrite it");
                return;
            }

            TablePosition pos = new TablePosition("Position " + (slotIndex + 1));

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

            if (!ignoreHeight && handControl.tableHeightControl != null)
            {
                pos.tableHeight = GetTelescopicPosition(handControl.tableHeightControl);
                Debug.Log("[TablePositionManager] SAVING - Telescopic height: " + pos.tableHeight);
            }
            else
            {
                pos.tableHeight = 0f;
            }

            if (handControl.tableLongitudinalControl != null)
            {
                pos.tableLongitudinalX = handControl.tableLongitudinalControl.currentPositionX;
            }

            pos.tableState = handControl.tableState;

            savedPositions[slotIndex] = pos;

            Debug.Log("[TablePositionManager] Saved position to slot " + (slotIndex + 1) + ": " + pos.name);
        }

        public void LoadPosition(int slotIndex)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl is not assigned!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Table is locked - cannot load position");
                return;
            }

            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Invalid slot index: " + slotIndex);
                return;
            }

            TablePosition pos = savedPositions[slotIndex];

            if (pos.IsEmpty())
            {
                Debug.LogWarning("[TablePositionManager] Slot " + (slotIndex + 1) + " is empty");
                return;
            }

            if (isRestoring)
            {
                Debug.LogWarning("[TablePositionManager] A position restore is already in progress");
                return;
            }

            Debug.Log("[TablePositionManager] Loading position from slot " + (slotIndex + 1) + ": " + pos.name);
            StartCoroutine(LoadPositionCoroutine(pos));
        }

        public void RestorePredefinedPosition(PredefinedPositionType positionType)
        {
            if (handControl == null)
            {
                Debug.LogError("[TablePositionManager] HandControl is not assigned!");
                return;
            }

            if (handControl.isLocked)
            {
                Debug.Log("[TablePositionManager] Table is locked - cannot set predefined position");
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
                    Debug.LogWarning("[TablePositionManager] Unknown predefined position type");
                    return;
            }

            Debug.Log("[TablePositionManager] Restoring predefined position: " + pos.name);
            StartCoroutine(LoadPositionCoroutine(pos));
        }

        public void ClearPosition(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Invalid slot index: " + slotIndex);
                return;
            }

            savedPositions[slotIndex] = new TablePosition("");
            Debug.Log("[TablePositionManager] Cleared slot " + (slotIndex + 1));
        }

        public void ClearAllPositions()
        {
            for (int i = 0; i < maxPositions; i++)
            {
                savedPositions[i] = new TablePosition("");
            }
            Debug.Log("[TablePositionManager] Cleared all positions");
        }

        public bool IsSlotOccupied(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return false;

            return !savedPositions[slotIndex].IsEmpty();
        }

        public string GetPositionName(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return "";

            return savedPositions[slotIndex].name;
        }

        public void SetPositionName(int slotIndex, string customName)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
            {
                Debug.LogError("[TablePositionManager] Invalid slot index: " + slotIndex);
                return;
            }

            if (!savedPositions[slotIndex].IsEmpty())
            {
                savedPositions[slotIndex].name = customName;
                Debug.Log("[TablePositionManager] Changed slot " + (slotIndex + 1) + " name to: " + customName);
            }
        }

        public string GetPositionDetails(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= maxPositions)
                return "Invalid slot";

            TablePosition pos = savedPositions[slotIndex];
            if (pos.IsEmpty())
                return "Empty slot";

            return pos.name + "\n" +
                   "Height: " + pos.tableHeight.ToString("F4") + "\n" +
                   "Longitudinal position: " + pos.tableLongitudinalX.ToString("F4") + "\n" +
                   "Reverse: " + pos.tableState.ToString();
        }
        
        private IEnumerator LoadPositionCoroutine(TablePosition pos)
        {
            isRestoring = true;

            Debug.Log("[TablePositionManager] Starting position restore: " + pos.name);

            handControl.StopAllMovement();

            if (pos.tableState != handControl.tableState)
            {
                Debug.Log("[TablePositionManager] Changing reverse to: " + pos.tableState);
                if (pos.tableState == TableState.Reverse)
                    handControl.ReversePosition();
                else
                    handControl.NormalPosition();

                yield return new WaitForSeconds(0.5f);
            }

            StartCoroutine(UpdateBlendShapesCoroutine());

            runningCoroutines = 0;

            if (handControl.tableBackPartUpperRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartUpperRotate, pos.backUpperAngleX, pos.backUpperAngleY, pos.backUpperAngleZ));
                Debug.Log("[TablePositionManager] Started restoring: tableBackPartUpperRotate");
            }

            if (handControl.tableBackPartLowerRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerRotate, pos.backLowerAngleX, pos.backLowerAngleY, pos.backLowerAngleZ));
                Debug.Log("[TablePositionManager] Started restoring: tableBackPartLowerRotate");
            }

            if (handControl.tableBackPartLowerLeftRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerLeftRotate, pos.backLowerLeftAngleX, pos.backLowerLeftAngleY, pos.backLowerLeftAngleZ));
                Debug.Log("[TablePositionManager] Started restoring: tableBackPartLowerLeftRotate");
            }

            if (handControl.tableBackPartLowerRightRotate != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableBackPartLowerRightRotate, pos.backLowerRightAngleX, pos.backLowerRightAngleY, pos.backLowerRightAngleZ));
                Debug.Log("[TablePositionManager] Started restoring: tableBackPartLowerRightRotate");
            }

            if (handControl.tableRotation != null)
            {
                runningCoroutines++;
                StartCoroutine(RestorePivotCoroutineTracked(handControl.tableRotation, pos.tableRotationAngleX, pos.tableRotationAngleY, pos.tableRotationAngleZ));
                Debug.Log("[TablePositionManager] Started restoring: tableRotation");
            }

            if (!ignoreHeight && handControl.tableHeightControl != null)
            {
                runningCoroutines++;
                StartCoroutine(RestoreHeightCoroutineTracked(pos.tableHeight));
                Debug.Log("[TablePositionManager] Started restoring height: " + pos.tableHeight);
            }
            else
            {
                Debug.Log("[TablePositionManager] Skipping height restore (ignoreHeight = " + ignoreHeight + ")");
            }

            if (handControl.tableLongitudinalControl != null)
            {
                runningCoroutines++;
                StartCoroutine(RestoreLongitudinalCoroutineTracked(pos.tableLongitudinalX));
                Debug.Log("[TablePositionManager] Started restoring longitudinal position: " + pos.tableLongitudinalX);
            }

            Debug.Log("[TablePositionManager] Started " + runningCoroutines + " restore operations");

            float maxWaitTime = 30f;
            float waitedTime = 0f;
            while (runningCoroutines > 0 && waitedTime < maxWaitTime)
            {
                yield return new WaitForSeconds(0.1f);
                waitedTime += 0.1f;
            }

            if (runningCoroutines > 0)
            {
                Debug.LogWarning("[TablePositionManager] Timeout - not all operations completed. Remaining: " + runningCoroutines);
            }

            isRestoring = false;
            Debug.Log("[TablePositionManager] Position loaded: " + pos.name);
        }

        private int runningCoroutines = 0;

        private IEnumerator RestorePivotCoroutineTracked(RotationPivot pivot, float targetX, float targetY, float targetZ)
        {
            yield return StartCoroutine(RestorePivotCoroutine(pivot, targetX, targetY, targetZ));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Pivot finished. Remaining: " + runningCoroutines);
        }

        private IEnumerator RestoreHeightCoroutineTracked(float targetHeight)
        {
            yield return StartCoroutine(RestoreHeightCoroutine(targetHeight));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Height restore finished. Remaining: " + runningCoroutines);
        }

        private IEnumerator RestoreLongitudinalCoroutineTracked(float targetX)
        {
            yield return StartCoroutine(RestoreLongitudinalCoroutine(targetX));
            runningCoroutines--;
            Debug.Log("[TablePositionManager] Longitudinal restore finished. Remaining: " + runningCoroutines);
        }

        private IEnumerator RestorePivotCoroutine(RotationPivot pivot, float targetX, float targetY, float targetZ)
        {
            if (pivot == null)
                yield break;

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

            Debug.Log("[TablePositionManager] " + pivot.pivotName + " axis " + detectedAxis +
                      " - Aim: " + targetAngle.ToString("F1") + "°");

            int iterations = 0;
            int maxIterations = 1000;

            while (iterations < maxIterations)
            {
                float currentAngle = pivot.GetCurrentAngle(detectedAxis);
                float diff = targetAngle - currentAngle;

                if (Mathf.Abs(diff) < 0.1f)
                    break;

                int direction = diff > 0 ? 1 : -1;
                float delta = restoreRotationStep * direction;

                if (Mathf.Abs(delta) > Mathf.Abs(diff))
                {
                    delta = diff;
                }

                bool success = pivot.RotateWithVector3(axis, delta);

                if (!success)
                {
                    Debug.LogWarning("[TablePositionManager] " + pivot.pivotName +
                                   " cannot continue " + detectedAxis);
                    break;
                }

                iterations++;
                yield return new WaitForSeconds(restoreRotationTickInterval);
            }

            if (iterations >= maxIterations)
            {
                Debug.LogWarning("[TablePositionManager] " + pivot.pivotName +
                                " max of iterations reached " + detectedAxis);
            }

            float finalAngle = pivot.GetCurrentAngle(detectedAxis);
            Debug.Log("[TablePositionManager] " + pivot.pivotName + " axis " + detectedAxis +
                      " is over with value: " + finalAngle.ToString("F1") + "°");
        }

        private IEnumerator RestoreHeightCoroutine(float targetHeight)
        {
            if (handControl.tableHeightControl == null)
            {
                Debug.LogWarning("[TablePositionManager] tableHeightControl is null!");
                yield break;
            }

            float currentHeight = GetTelescopicPosition(handControl.tableHeightControl);
            Debug.Log("[TablePositionManager] Restoring height - Start: " + currentHeight + " -> Aim: " + targetHeight);

            int iterations = 0;
            int maxIterations = 10000;

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
                    Debug.LogWarning("[TablePositionManager] Height - cannot continue. Current: " + currentHeight);
                    break;
                }

                currentHeight = GetTelescopicPosition(handControl.tableHeightControl);
                iterations++;

                yield return new WaitForSeconds(restoreHeightTickInterval);
            }

            if (iterations >= maxIterations)
            {
                Debug.LogWarning("[TablePositionManager] Height - no iterations left!");
            }

            Debug.Log("[TablePositionManager] Restoring height - over: " + currentHeight + " (number of iterations: " + iterations + ")");
        }

        private float GetTelescopicPosition(TelescopicMovement telescopic)
        {
            if (telescopic == null)
            {
                Debug.LogWarning("[TablePositionManager] GetTelescopicPosition - telescopic is null!");
                return 0f;
            }

            if (telescopic.movementAxes == null || telescopic.movementAxes.Length == 0)
            {
                Debug.LogWarning("[TablePositionManager] GetTelescopicPosition - no telescopic axis!");
                return 0f;
            }

            float totalPosition = 0f;
            char axis = DetectAxisFromVector(telescopic.movementAxis);

            foreach (MovementAxis movementAxis in telescopic.movementAxes)
            {
                if (movementAxis == null)
                    continue;

                float axisPos = 0f;

                switch (axis)
                {
                    case 'X':
                        axisPos = movementAxis.currentPositionX;
                        break;
                    case 'Y':
                        axisPos = movementAxis.currentPositionY;
                        break;
                    case 'Z':
                        axisPos = movementAxis.currentPositionZ;
                        break;
                }

                totalPosition += axisPos;

                Debug.Log("[TablePositionManager] Axis: " + movementAxis.gameObject.name +
                          " = " + axisPos.ToString("F4"));
            }

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

        private IEnumerator UpdateBlendShapesCoroutine()
        {
            if (blendShapeController == null || handControl == null)
                yield break;

            while (isRestoring)
            {
                if (handControl.tableRotation != null)
                {
                    float trendAngle = handControl.tableRotation.currentAngleZ;
                    blendShapeController.UpdateTrendelenburg(trendAngle);
                }

                if (handControl.tableRotation != null)
                {
                    float lateralAngle = handControl.tableRotation.currentAngleX;
                    blendShapeController.UpdateLateral(lateralAngle);
                }

                yield return null;

                Debug.Log("[TablePositionManager] BlendShape coroutine is over");
            }
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