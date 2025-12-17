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

    // // ============================================================
    // // TABLE RESET
    // // ============================================================

    // public void StartReset()
    // {
    //     if (tableStateManager != null)
    //     {
    //         Debug.Log("[HandControl] Rozpoczynam reset stołu");
    //         tableStateManager.isResetting = true;
    //     }
    // }

    // public void StopReset()
    // {
    //     if (tableStateManager != null)
    //     {
    //         Debug.Log("[HandControl] Zatrzymuję reset stołu");
    //         tableStateManager.isResetting = false;
    //     }
    // }

    // ============================================================
    // ROTATION
    // ============================================================

    // BACK TILT
    public void TiltBackUp()
    {
        Debug.Log("[HandControl] Podnoszę górną część stołu");
        StartTiltElement(tableBackPartUpperRotate, Vector3.right, 1);
    }

    public void TiltBackDown()
    {
        Debug.Log("[HandControl] Opuszczam górną część stołu");
        StartTiltElement(tableBackPartUpperRotate, Vector3.right, -1);
    }

    // LEGS TILT
    public void TiltLegsUp()
    {
        Debug.Log("[HandControl] Podnoszę dolną część stołu");
        StartTiltElement(tableBackPartLowerRotate, Vector3.right, -1);
    }

    public void TiltLegsDown()
    {
        Debug.Log("[HandControl] Opuszczam dolną część stołu");
        StartTiltElement(tableBackPartLowerRotate, Vector3.right, 1);
    }

    // TRENDELENBURG POSITION 
    public void TiltTrendelenburg()
    {
        Debug.Log("[HandControl] Pozycja Trendelenburga");
        StartTiltElement(tableRotation, Vector3.right, -1);
    }

    public void TiltReverseTrendelenburg()
    {
        Debug.Log("[HandControl] Odwrócona pozycja Trendelenburga");
        StartTiltElement(tableRotation, Vector3.right, 1);
    }

    // LATERAL TILT
    public void TiltTableRight()
    {
        Debug.Log("[HandControl] Przechylam stół w prawo");
        StartTiltElement(tableRotation, Vector3.up, -1);
    }

    public void TiltTableLeft()
    {
        Debug.Log("[HandControl] Przechylam stół w lewo");
        StartTiltElement(tableRotation, Vector3.up, 1);
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
        StartLongitudinalMovement(Vector3.up, 1);
    }

    public void MoveTableBackward()
    {
        Debug.Log("[HandControl] Przesuwam stół do tyłu");
        StartLongitudinalMovement(Vector3.up, -1);
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

    // public bool IsResetting
    // {
    //     get { return tableStateManager != null && tableStateManager.isResetting; }
    // }
}