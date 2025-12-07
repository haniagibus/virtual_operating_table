using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class HandControlPanel : MonoBehaviour
{
    [Header("UI Panel")]
    public GameObject handControlPanel;
    public KeyCode toggleKey = KeyCode.P;
    public bool startClosed = true;

    [Header("Hand Control")]
    public HandControl handControl;

    [Header("Optional UI Settings")]
    public EventSystem eventSystem;
    public bool autoSelectFirst = true;

    private bool isOpen = false;

    // ============================================================
    // UNITY LIFECYCLE
    // ============================================================
    void Awake()
    {
        if (handControlPanel == null)
        {
            Debug.LogError("[HandControlPanel] HandControlPanel nie przypisany w Inspektorze!");
        }

        if (handControl == null)
        {
            Debug.LogError("[HandControlPanel] HandControl nie przypisany w Inspektorze!");
        }

        if (eventSystem == null)
        {
            eventSystem = EventSystem.current;
        }
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

    // // ============================================================
    // // TABLE RESET
    // // ============================================================

    // public void OnResetButtonDown()
    // {
    //     if (handControl != null)
    //     {
    //         handControl.StartReset();
    //     }
    // }

    // ============================================================
    // TABLE HEIGHT (teleskopowe podnoszenie)
    // ============================================================
    public void OnRaiseTableButtonDown()
    {
        if (handControl != null)
        {
            handControl.RaiseTable();
        }
    }



    public void OnLowerTableButtonDown()
    {
        if (handControl != null)
        {
            handControl.LowerTable();
        }
    }

    // ============================================================
    // BACK TILT
    // ============================================================
    public void OnTiltBackUpButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltBackUp();
        }
    }

    public void OnTiltBackDownButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltBackDown();
        }
    }

    // ============================================================
    // LEGS TILT
    // ============================================================
    public void OnTiltLegsUpButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltLegsUp();
        }
    }

    public void OnTiltLegsDownButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltLegsDown();
        }
    }

    // ============================================================
    // TRENDELENBURG
    // ============================================================
    public void OnTrendelenburgButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTrendelenburg();
        }
    }

    public void OnReverseTrendelenburgButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltReverseTrendelenburg();
        }
    }

    // ============================================================
    // LATERAL TILT
    // ============================================================
    public void OnTiltTableRightButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTableRight();
        }
    }

    public void OnTiltTableLeftButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTableLeft();
        }
    }

    // ============================================================
    // STOP ALL MOVEMENT
    // ============================================================
    public void OnButtonUpStopAllMovement()
    {
        if (handControl != null)
        {
            handControl.StopAllMovement();
        }
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
            {
                eventSystem.SetSelectedGameObject(firstSelectable.gameObject);
            }
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