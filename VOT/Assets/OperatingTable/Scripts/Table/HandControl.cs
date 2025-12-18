using UnityEngine;
using System.Collections;


public class HandControl : MonoBehaviour
{
    [Header("Table State")]
    public TableStateManager tableStateManager;


    [Header("Table Components - Rotation")]
    public RotationPivot tableBackPartUpperRotate;
    public RotationPivot tableBackPartLowerRotate;
    public RotationPivot tableRotation;

    [Header("Table Telescopic Movement")]
    [Tooltip("Teleskopowy mechanizm podnoszenia stołu")]
    public TelescopicMovement tableHeightControl;

    [Header("Table Longitudinal Movement")]
    [Tooltip("Mechanizm przesuwania stołu wzdłużnie")]
    public MovementAxis tableLongitudinalControl;

    [Header("Table Elements to Move")]
    [Tooltip("Element który będzie odłączany/przyłączany podczas ruchu")]
    public Transform tableRotateElement;

    [Header("Blend Shapes")]
    [Tooltip("SkinnedMeshRenderer z blend shapes dla plecków")]
    public SkinnedMeshRenderer tableBackPad;
    [Tooltip("Szybkość zmiany blend shape - współczynnik względem rotacji")]
    public float blendShapeMultiplier = 5f;


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

    // Indeksy blend shapes - znajdowane automatycznie
    private int blendShapeBackPadUpperUp = -1;
    private int blendShapeBackPadUpperDown = -1;
    private int blendShapeBackPadLowerUp = -1;
    private int blendShapeBackPadLowerDown = -1;

    private void Start()
    {
        // Znajdź indeksy blend shapes
        if (tableBackPad != null)
        {
            Mesh mesh = tableBackPad.sharedMesh;
            for (int i = 0; i < mesh.blendShapeCount; i++)
            {
                string shapeName = mesh.GetBlendShapeName(i);
                
                if (shapeName == "back_pad_upper_up")
                    blendShapeBackPadUpperUp = i;
                else if (shapeName == "back_pad_upper_down")
                    blendShapeBackPadUpperDown = i;
                else if (shapeName == "back_pad_lower_up")
                    blendShapeBackPadLowerUp = i;
                else if (shapeName == "back_pad_lower_down")
                    blendShapeBackPadLowerDown = i;
            }

            Debug.Log("[HandControl] Blend shapes: upper_up=" + blendShapeBackPadUpperUp + ", upper_down=" + blendShapeBackPadUpperDown + ", lower_up=" + blendShapeBackPadLowerUp + ", lower_down=" + blendShapeBackPadLowerDown);
        }
    }

    // ============================================================
    // ROTATION
    // ============================================================

    // BACK TILT
    public void TiltBackUp()
    {
        Debug.Log("[HandControl] Podnoszę górną część stołu");
        StartTiltElement(tableBackPartUpperRotate, Vector3.right, 1, blendShapeBackPadUpperUp, blendShapeBackPadUpperDown);
    }

    public void TiltBackDown()
    {
        Debug.Log("[HandControl] Opuszczam górną część stołu");
        StartTiltElement(tableBackPartUpperRotate, Vector3.right, -1, blendShapeBackPadUpperDown, blendShapeBackPadUpperUp);
    }

    // LEGS TILT
    public void TiltLegsUp()
    {
        Debug.Log("[HandControl] Podnoszę dolną część stołu");
        StartTiltElement(tableBackPartLowerRotate, Vector3.right, -1, blendShapeBackPadLowerUp, blendShapeBackPadLowerDown);
    }

    public void TiltLegsDown()
    {
        Debug.Log("[HandControl] Opuszczam dolną część stołu");
        StartTiltElement(tableBackPartLowerRotate, Vector3.right, 1, blendShapeBackPadLowerDown, blendShapeBackPadLowerUp);
    }

    // TRENDELENBURG POSITION 
    public void TiltTrendelenburg()
    {
        Debug.Log("[HandControl] Pozycja Trendelenburga");
        StartTiltElement(tableRotation, Vector3.right, -1, -1, -1);
    }

    public void TiltReverseTrendelenburg()
    {
        Debug.Log("[HandControl] Odwrócona pozycja Trendelenburga");
        StartTiltElement(tableRotation, Vector3.right, 1, -1, -1);
    }

    // LATERAL TILT
    public void TiltTableRight()
    {
        Debug.Log("[HandControl] Przechylam stół w prawo");
        StartTiltElement(tableRotation, Vector3.up, -1, -1, -1);
    }

    public void TiltTableLeft()
    {
        Debug.Log("[HandControl] Przechylam stół w lewo");
        StartTiltElement(tableRotation, Vector3.up, 1, -1, -1);
    }


    // ============================================================
    // MOVEMENT
    // ============================================================

    // HEIGHT
    public void RaiseTable()
    {
        Debug.Log("[HandControl] Podnoszę stół");
        StartHeightMovement(1);
    }

    public void LowerTable()
    {
        Debug.Log("[HandControl] Opuszczam stół");
        StartHeightMovement(-1);
    }

    // LONGITUDINAL
    public void MoveTableForward()
    {
        Debug.Log("[HandControl] Przesuwam stół do przodu");
        StartLongitudinalMovement(Vector3.right, 1);
    }

    public void MoveTableBackward()
    {
        Debug.Log("[HandControl] Przesuwam stół do tyłu");
        StartLongitudinalMovement(Vector3.right, -1);
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
    // INTERNAL LOGIC
    // ============================================================

    // ROTATION
    private void StartTiltElement(RotationPivot pivot, Vector3 axis, int direction, int blendShapeIndexIncrease, int blendShapeIndexDecrease)
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
        currentTiltCoroutine = StartCoroutine(TiltElementCoroutine(pivot, axis, direction, blendShapeIndexIncrease, blendShapeIndexDecrease));
    }

    private IEnumerator TiltElementCoroutine(RotationPivot pivot, Vector3 axis, int direction, int blendShapeIndexIncrease, int blendShapeIndexDecrease)
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

        DetachFromParent(tableRotateElement);
        AttachToParent(tableRotateElement, "table_leg_column_segment_4_move");

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

        DetachFromParent(tableRotateElement);
        AttachToParent(tableRotateElement, "table");

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
}