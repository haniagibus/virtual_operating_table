using UnityEngine;
using System.Collections;


public class HandControl : MonoBehaviour
{
    [Header("Table Components - Rotation")]
    public RotationPivot tableBackPartUpperRotate;
    public RotationPivot tableBackPartLowerRotate;
    public RotationPivot tableRotation;

    [Header("Table Components - Telescopic Height")]
    [Tooltip("Teleskopowy mechanizm podnoszenia stołu")]
    public TelescopicMovement tableHeightControl;

    [Header("Table State")]
    public TableStateManager tableStateManager;

    [Header("Rotation Settings")]
    public float rotationStep = 2f;
    public float rotationTickInterval = 0.05f;

    [Header("Height Movement Settings")]
    public float heightStep = 0.0001f; // 2cm na tick
    public float heightTickInterval = 0.05f;

    private Coroutine currentTiltCoroutine = null;
    private Coroutine currentHeightCoroutine = null;
    private bool isTilting = false;
    private bool isMovingHeight = false;

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
    // BACK TILT
    // ============================================================
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

    // ============================================================
    // LEGS TILT
    // ============================================================
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

    // ============================================================
    // TRENDELENBURG POSITION
    // ============================================================
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

    // ============================================================
    // TABLE LATERAL TILT
    // ============================================================
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
    // TABLE HEIGHT
    // ============================================================
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

    // ============================================================
    // STOP ALL MOVEMENT
    // ============================================================
    public void StopAllMovement()
    {
        // Zatrzymaj rotację
        isTilting = false;
        if (currentTiltCoroutine != null)
        {
            StopCoroutine(currentTiltCoroutine);
            currentTiltCoroutine = null;
        }

        // Zatrzymaj ruch wysokości
        isMovingHeight = false;
        if (currentHeightCoroutine != null)
        {
            StopCoroutine(currentHeightCoroutine);
            currentHeightCoroutine = null;
        }

        Debug.Log("[HandControl] Zatrzymuję ruch stołu");
    }

    // ============================================================
    // INTERNAL LOGIC - Rotation
    // ============================================================
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

    // ============================================================
    // INTERNAL LOGIC - Telescopic Height
    // ============================================================
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

    // ============================================================
    // PUBLIC GETTERS
    // ============================================================
    public bool IsTilting
    {
        get { return isTilting; }
    }

    public bool IsMovingHeight
    {
        get { return isMovingHeight; }
    }

    public bool IsResetting
    {
        get { return tableStateManager != null && tableStateManager.isResetting; }
    }
}