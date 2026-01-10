using UnityEngine;
using System.Collections;
//using System.Diagnostics;


namespace OperatingTable
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

        [Header("Table Longitudinal Movement")]
        [Tooltip("Mechanizm przesuwania stołu wzdłużnie")]
        public MovementAxis tableLongitudinalControl;

        // [Header("Table Elements to Move")]
        // [Tooltip("Element który będzie odłączany/przyłączany podczas ruchu")]
        // public Transform tableRotateElement;

        // [Header("Blend Shapes")]
        // [Tooltip("SkinnedMeshRenderer z blend shapes dla plecków")]
        // public SkinnedMeshRenderer tableBackPad;
        // [Tooltip("Szybkość zmiany blend shape - współczynnik względem rotacji")]
        // public float blendShapeMultiplier = 5f;


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


        // // Indeksy blend shapes - znajdowane automatycznie
        // private int blendShapeBackPadUpperUp = -1;
        // private int blendShapeBackPadUpperDown = -1;
        // private int blendShapeBackPadLowerUp = -1;
        // private int blendShapeBackPadLowerDown = -1;

        // private void Start()
        // {
        //     // Znajdź indeksy blend shapes
        //     if (tableBackPad != null)
        //     {
        //         Mesh mesh = tableBackPad.sharedMesh;
        //         for (int i = 0; i < mesh.blendShapeCount; i++)
        //         {
        //             string shapeName = mesh.GetBlendShapeName(i);

        //             if (shapeName == "back_pad_upper_up")
        //                 blendShapeBackPadUpperUp = i;
        //             else if (shapeName == "back_pad_upper_down")
        //                 blendShapeBackPadUpperDown = i;
        //             else if (shapeName == "back_pad_lower_up")
        //                 blendShapeBackPadLowerUp = i;
        //             else if (shapeName == "back_pad_lower_down")
        //                 blendShapeBackPadLowerDown = i;
        //         }

        //         Debug.Log("[HandControl] Blend shapes: upper_up=" + blendShapeBackPadUpperUp + ", upper_down=" + blendShapeBackPadUpperDown + ", lower_up=" + blendShapeBackPadLowerUp + ", lower_down=" + blendShapeBackPadLowerDown);
        //     }
        // }

        // ============================================================
        // ROTATION
        // ============================================================

        // BACK TILT
        public void TiltBackPlate(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Podnoszę górną część stołu");
            StartTiltElement(tableBackPartUpperRotate, Vector3.up, direction);
        }

        // public void TiltBackDown()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Opuszczam górną część stołu");
        //     StartTiltElement(tableBackPartUpperRotate, Vector3.up, -1);

        // }

        // LEGS TILT
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
                StartTiltElement(tableBackPartLowerRotate, Vector3.up, direction);
            }
            else if (currentLeg == LegSelection.Left)
            {
                StartTiltElement(tableBackPartLowerLeftRotate, Vector3.up, direction);

            }
            else if (currentLeg == LegSelection.Right)
            {
                StartTiltElement(tableBackPartLowerRightRotate, Vector3.up, direction);

            }
        }

        // public void TiltLegsDown()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Opuszczam dolną część stołu");
        //     if (currentLeg == LegSelection.Both)
        //     {
        //         StartTiltElement(tableBackPartLowerRotate, Vector3.up, 1);
        //     }
        //     else if (currentLeg == LegSelection.Left)
        //     {
        //         StartTiltElement(tableBackPartLowerLeftRotate, Vector3.up, 1);

        //     }
        //     else if (currentLeg == LegSelection.Right)
        //     {
        //         StartTiltElement(tableBackPartLowerRightRotate, Vector3.up, 1);

        //     }

        // }

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

        // TRENDELENBURG POSITION 
        public void TiltTrendelenburg(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Pozycja Trendelenburga");
            StartTiltElement(tableRotation, Vector3.forward, direction);
        }

        // public void TiltReverseTrendelenburg()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Odwrócona pozycja Trendelenburga");
        //     StartTiltElement(tableRotation, Vector3.forward, -1);
        // }

        // LATERAL TILT
        public void TiltTable(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Przechylam stół w prawo");
            StartTiltElement(tableRotation, Vector3.right, direction);
        }

        // public void TiltTableLeft()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Przechylam stół w lewo");
        //     StartTiltElement(tableRotation, Vector3.right, -1);
        // }


        // ============================================================
        // MOVEMENT
        // ============================================================

        // HEIGHT
        public void ChangeTableHeight(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Podnoszę stół");
            StartHeightMovement(direction);
        }

        // public void LowerTable()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Opuszczam stół");
        //     StartHeightMovement(-1);
        // }

        // LONGITUDINAL
        public void MoveTable(int direction)
        {
            if (isLocked == true)
            {
                Debug.Log("[HandControl] Stół wyłączony ");
                return;
            }

            Debug.Log("[HandControl] Przesuwam stół do przodu");
            StartLongitudinalMovement(Vector3.right, direction);
        }

        // public void MoveTableBackward()
        // {
        //     if (isLocked == true)
        //     {
        //         Debug.Log("[HandControl] Stół wyłączony ");
        //         return;
        //     }

        //     Debug.Log("[HandControl] Przesuwam stół do tyłu");
        //     StartLongitudinalMovement(Vector3.right, 1);
        // }


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
        private void StartTiltElement(RotationPivot pivot, Vector3 axis, int direction)
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
            currentTiltCoroutine = StartCoroutine(TiltElementCoroutine(pivot, axis, direction));
        }

        private IEnumerator TiltElementCoroutine(RotationPivot pivot, Vector3 axis, int direction)
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

                // // Zaktualizuj blend shapes
                // if (tableBackPad != null)
                // {
                //     float blendShapeDelta = Mathf.Abs(delta) * blendShapeMultiplier;


                //     // Zmniejsz blend shape w przeciwnym kierunku
                //     if (blendShapeIndexDecrease >= 0 && blendShapeIndexIncrease <= 0)
                //     {
                //         float currentWeight = tableBackPad.GetBlendShapeWeight(blendShapeIndexDecrease);
                //         float newWeight = Mathf.Clamp(currentWeight - blendShapeDelta, 0f, 100f);
                //         tableBackPad.SetBlendShapeWeight(blendShapeIndexDecrease, newWeight);
                //     }


                //     // Zwiększ blend shape w kierunku ruchu
                //     if (blendShapeIndexIncrease >= 0)
                //     {
                //         float currentWeight = tableBackPad.GetBlendShapeWeight(blendShapeIndexIncrease);
                //         float newWeight = Mathf.Clamp(currentWeight + blendShapeDelta, 0f, 100f);
                //         tableBackPad.SetBlendShapeWeight(blendShapeIndexIncrease, newWeight);
                //     }
                // }

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

            // DetachFromParent(tableRotateElement);
            // AttachToParent(tableRotateElement, "table_leg_column_segment_4_move");

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

            // DetachFromParent(tableRotateElement);
            // AttachToParent(tableRotateElement, "table");

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

        // void StartTracked(IEnumerator coroutine, int running)
        // {
        //     running++;
        //     StartCoroutine(TrackCoroutine(coroutine, running));
        // }

        // IEnumerator TrackCoroutine(IEnumerator coroutine, int running)
        // {
        //     yield return StartCoroutine(coroutine);
        //     running--;
        // }

        // private IEnumerator LevelZeroCoroutine()
        // {
        //     int running = 0;

        //     if (tableBackPartUpperRotate != null)
        //         StartTracked(ResetPivotToZeroCoroutine(tableBackPartUpperRotate), running);

        //     if (tableBackPartLowerRotate != null)
        //         StartTracked(ResetPivotToZeroCoroutine(tableBackPartLowerRotate), running);

        //     if (tableBackPartLowerLeftRotate != null)
        //         StartTracked(ResetPivotToZeroCoroutine(tableBackPartLowerLeftRotate), running);

        //     if (tableBackPartLowerRightRotate != null)
        //         StartTracked(ResetPivotToZeroCoroutine(tableBackPartLowerRightRotate), running);

        //     if (tableRotation != null)
        //         StartTracked(ResetPivotToZeroCoroutine(tableRotation), running);

        //     if (tableLongitudinalControl != null)
        //         StartTracked(ResetLongitudinalToZeroCoroutine(), running);

        //     yield return new WaitUntil(() => running == 0);

        //     Debug.Log("[HandControl] Pozycja zerowa - stół wypoziomowany i wycentrowany");
        // }

        // private IEnumerator ResetPivotToZeroCoroutine(RotationPivot pivot)
        // {
        //     if (pivot == null)
        //     {
        //         yield break;
        //     }

        //     if (pivot.allowX && Mathf.Abs(pivot.currentAngleX) > 0.01f)
        //     {
        //         yield return ResetAxisToZeroCoroutine(pivot, Vector3.right, pivot.currentAngleX);
        //     }

        //     if (pivot.allowY && Mathf.Abs(pivot.currentAngleY) > 0.01f)
        //     {
        //         yield return ResetAxisToZeroCoroutine(pivot, Vector3.up, pivot.currentAngleY);
        //     }

        //     if (pivot.allowZ && Mathf.Abs(pivot.currentAngleZ) > 0.01f)
        //     {
        //         yield return ResetAxisToZeroCoroutine(pivot, Vector3.forward, pivot.currentAngleZ);
        //     }
        // }

        // private IEnumerator ResetAxisToZeroCoroutine(RotationPivot pivot, Vector3 axis, float currentAngle)
        // {
        //     while (Mathf.Abs(currentAngle) > 0.01f)
        //     {
        //         int direction = currentAngle > 0 ? -1 : 1;
        //         float delta = rotationStep * direction;

        //         if (Mathf.Abs(delta) > Mathf.Abs(currentAngle))
        //         {
        //             delta = -currentAngle;
        //         }

        //         pivot.RotateWithVector3(axis, delta);

        //         char detectedAxis = DetectAxisFromVector(axis);
        //         currentAngle = pivot.GetCurrentAngle(detectedAxis);

        //         yield return new WaitForSeconds(rotationTickInterval);
        //     }
        // }
        // private IEnumerator ResetLongitudinalToZeroCoroutine()
        // {
        //     float currentPos = GetLongitudinalCurrentPosition();

        //     if (Mathf.Abs(currentPos) < 0.0001f)
        //     {
        //         yield break;
        //     }

        //     while (Mathf.Abs(currentPos) > 0.0001f)
        //     {
        //         int direction = currentPos > 0 ? -1 : 1;
        //         float delta = longitudinalStep * direction;

        //         if (Mathf.Abs(delta) > Mathf.Abs(currentPos))
        //         {
        //             delta = -currentPos;
        //         }

        //         bool canContinue = tableLongitudinalControl.MoveWithVector3(Vector3.right, delta);

        //         if (!canContinue)
        //         {
        //             break;
        //         }

        //         currentPos = GetLongitudinalCurrentPosition();
        //         yield return new WaitForSeconds(longitudinalTickInterval);
        //     }

        //     Debug.Log("[HandControl] Pozycja wzdłużna zresetowana do zera");
        // }

        private IEnumerator RotateToReverseCoroutine(bool reverse)
        {
            if (tableTop == null)
            {
                Debug.LogError("[HandControl] TableTop nie jest przypisany!");
                yield break;
            }

            float targetAngle = reverse ? 180f : 0f;
            float currentAngle = tableTop.localEulerAngles.y;

            if (currentAngle > 180f) currentAngle -= 360f;
            tableTop.Rotate(Vector3.up, targetAngle, Space.Self);
            Vector3 euler = tableTop.localEulerAngles;
            euler.y = targetAngle;
            tableTop.localEulerAngles = euler;

            reverseCoroutine = null;
        }

        // HELPERS
        private void DetachFromParent(Transform element)
        {
            if (element == null)
            {
                Debug.LogWarning("[HandControl] Element do odłączenia jest null!");
                return;
            }

            if (element.parent != null)
            {
                Vector3 worldPosition = element.position;
                Quaternion worldRotation = element.rotation;

                element.SetParent(null);

                element.position = worldPosition;
                element.rotation = worldRotation;

                Debug.Log("[HandControl] Element " + element.name + " odłączony od parenta");
            }
        }

        private void AttachToParent(Transform element, string parentName)
        {
            if (element == null)
            {
                Debug.LogWarning("[HandControl] Element do przyłączenia jest null!");
                return;
            }

            GameObject parent = GameObject.Find(parentName);

            if (parent == null)
            {
                Debug.LogWarning("[HandControl] Nie znaleziono obiektu " + parentName);
                return;
            }

            Vector3 worldPosition = element.position;
            Quaternion worldRotation = element.rotation;

            element.SetParent(parent.transform);

            element.position = worldPosition;
            element.rotation = worldRotation;

            Debug.Log("[HandControl] Element " + element.name + " przypięty do " + parentName);
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