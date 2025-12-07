using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;

public class HandControlPanel : MonoBehaviour
{
    [Header("Hand Control Panel")]
    public GameObject handControlPanel;
    public KeyCode toggleKey = KeyCode.P;
    public bool startClosed = true;

    [Header("Optional")]
    public EventSystem eventSystem;
    public bool autoSelectFirst = true;

    [Header("Table Reset")]
    public TableStateManager tableStateManager;
    public KeyCode resetKey = KeyCode.R;
    private bool isOpen = false;

    [Header("Specific Pivots")]
    private Coroutine currentTiltCoroutine = null;
    private bool isTilting = false;

    public RotationPivot tableBackPartUpperRotate;
    public RotationPivot tableBackPartLowerRotate;
    public RotationPivot tableRotation;

    void Awake()
    {
        if (handControlPanel == null)
        {
            Debug.LogError("[HandControlPanel] handControlPanel nie przypisany w Inspektorze!");
        }

        if (eventSystem == null)
            eventSystem = EventSystem.current;
    }

    void Start()
    {
        if (handControlPanel != null)
        {
            isOpen = !startClosed;
            SetPanelState(!startClosed);
            SetCursorForPanel(isOpen);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleHandControlPanel();
        }
    }

    // ============================================================
    // LEVEL ZERO BUTTON
    // ============================================================
    public void StartReset()
    {
        if (tableStateManager != null)
        {
            Debug.Log("Rozpoczynam reset - trzymam przycisk");
            tableStateManager.isResetting = true;
        }
    }

    public void StopReset()
    {
        if (tableStateManager != null)
        {
            Debug.Log("Zatrzymuję reset - puszczam przycisk");
            tableStateManager.isResetting = false;
        }
    }

    // ============================================================
    // TILT FUNCTIONS
    // ============================================================
    public void TiltStart(RotationPivot pivot)
    {
        if (pivot == null)
        {
            Debug.LogError("Nie przypisano pivotu!");
            return;
        }
        isTilting = true;
        if (currentTiltCoroutine != null)
        {
            StopCoroutine(currentTiltCoroutine);
        }
    }

    public void TiltStop()
    {
        isTilting = false;
        Debug.Log("Zatrzymuję obracanie - puszczono przycisk");

        if (currentTiltCoroutine != null)
        {
            StopCoroutine(currentTiltCoroutine);
            currentTiltCoroutine = null;
        }
    }

    // ============================================================
    // TILT BACK BUTTONS
    // ============================================================
    public void TiltBackUpStart()
    {
        Debug.Log("Podnoszę górną część stołu");
        TiltStart(tableBackPartUpperRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartUpperRotate,
            Vector3.right,  // oś X
            1,
            2f
        ));
    }

    public void TiltBackDownStart()
    {
        Debug.Log("Opuszczam górną część stołu");
        TiltStart(tableBackPartUpperRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartUpperRotate,
            Vector3.right,
            -1,
            2f
        ));
    }

    // ============================================================
    // TILT LEGS
    // ============================================================
    public void TiltLegsUpStart()
    {
        Debug.Log("Podnoszę dolną część stołu");
        TiltStart(tableBackPartLowerRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartLowerRotate,
            Vector3.right,
            -1,
            2f
        ));
    }

    public void TiltLegsDownStart()
    {
        Debug.Log("Opuszczam dolną część stołu");
        TiltStart(tableBackPartLowerRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartLowerRotate,
            Vector3.right,
            1,
            2f
        ));
    }

    // ============================================================
    // TRENDELENBURG POSITION
    // ============================================================
    public void TrendelenburgPositionStart()
    {
        Debug.Log("Pozycja Trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.right,  // oś Z dla Trendelenburga
            -1,
            2f
        ));
    }

    public void ReverseTrendelenburgPositionStart()
    {
        Debug.Log("Odwrócona pozycja Trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.right,  // oś Z
            1,
            2f
        ));
    }

    // ============================================================
    // TILT TABLE LEFT/RIGHT
    // ============================================================
    public void TiltTableRightStart()
    {
        Debug.Log("Przechylam stół w prawo");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.up,  // oś Y
            -1,
            2f
        ));
    }

    public void TiltTableLeftStart()
    {
        Debug.Log("Przechylam stół w lewo");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.up,  // oś Y
            1,
            2f
        ));
    }

    // ============================================================
    // GŁÓWNA FUNKCJA OBROTU - zatrzymuje się po puszczeniu przycisku
    // ============================================================
    private IEnumerator TiltElement(RotationPivot pivot, Vector3 axis, int direction, float step)
    {
        if (pivot == null)
        {
            Debug.LogError("Pivot jest null!");
            yield break;
        }

        while (isTilting)
        {
            // Oblicz deltę obrotu
            float delta = step * direction;

            // Użyj funkcji RotateWithVector3 - zwraca false gdy osiągnięto limit
            bool canContinue = pivot.RotateWithVector3(axis, delta);
            
            // Jeśli osiągnięto limit, zatrzymaj
            if (!canContinue)
            {
                Debug.Log("Osiągnięto limit rotacji - zatrzymuję");
                isTilting = false;
                break;
            }

            yield return new WaitForSeconds(0.05f);
        }

        currentTiltCoroutine = null;
    }

    // ============================================================
    // PANEL TOGGLE
    // ============================================================
    public void ToggleHandControlPanel()
    {
        if (handControlPanel == null) return;

        isOpen = !isOpen;
        SetPanelState(isOpen);
        SetCursorForPanel(isOpen);

        if (isOpen && autoSelectFirst && eventSystem != null)
        {
            var firstSelectable = handControlPanel.GetComponentInChildren<Selectable>();
            if (firstSelectable != null)
                eventSystem.SetSelectedGameObject(firstSelectable.gameObject);
        }
        else if (!isOpen && eventSystem != null)
        {
            eventSystem.SetSelectedGameObject(null);
        }
    }

    private void SetPanelState(bool open)
    {
        handControlPanel.SetActive(open);
    }

    private void SetCursorForPanel(bool panelOpen)
    {
        if (panelOpen)
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}