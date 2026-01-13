using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using OperatingTable;

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
    // // MENU 
    // // ============================================================

    // public void OnMenuMiddleButtonDown()
    // {
    //     switch (menuController.currentState)
    //     {
    //         case MenuState.Closed:
    //             menuController.EnterMenu();
    //             break;

    //         case MenuState.Menu:
    //             menuController.EnterPositions();
    //             break;

    //         case MenuState.Positions:
    //             ConfirmPosition();
    //             break;
    //     }
    // }

    // private void ConfirmPosition()
    // {
    //     int index = menuController.currentIndex;

    //     if (index < 0) return;

    //     // TU mapujesz index → akcja
    //     Debug.Log("Zatwierdzono pozycję: " + index);
    // }

    // public void OnMenuLeftButtonDown()
    // {
    //     if (menuController.currentState == MenuState.Menu ||
    //         menuController.currentState == MenuState.Positions)
    //     {
    //         menuController.Iterate();
    //     }
    // }

    // public void OnMenuRightButtonDown()
    // {
    //     switch (menuController.currentState)
    //     {
    //         case MenuState.Positions:
    //             menuController.ExitToMenu();
    //             break;

    //         case MenuState.Menu:
    //             menuController.ExitAll();
    //             break;
    //     }
    // }

    // ============================================================
    // ROTATION 
    // ============================================================

    // BACK TILT
    public void OnTiltBackUpButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltBackPlate(-1);
        }
    }

    public void OnTiltBackDownButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltBackPlate(1);
        }
    }

    // LEGS TILT
    public void OnTiltLegsUpButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltLegsPlate(1);
        }
    }

    public void OnTiltLegsDownButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltLegsPlate(-1);
        }
    }

    public void OnRightLegSelectedButtonDown(Button button)
    {
        if (handControl != null)
        {
            handControl.RightLegSelected();

            if (button.image.color == Color.white)
                button.image.color = Color.red;
            else
                button.image.color = Color.white;
        }
    }
    public void OnLeftLegSelectedButtonDown(Button button)
    {
        if (handControl != null)
        {
            handControl.LeftLegSelected();

            if (button.image.color == Color.white)
                button.image.color = Color.red;
            else
                button.image.color = Color.white;
        }
    }

    // TRENDELENBURG POSITION
    public void OnTrendelenburgButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTrendelenburg(1);
        }
    }

    public void OnReverseTrendelenburgButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTrendelenburg(-1);
        }
    }

    // LATERAL TILT
    public void OnTiltTableRightButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTable(-1);
        }
    }

    public void OnTiltTableLeftButtonDown()
    {
        if (handControl != null)
        {
            handControl.TiltTable(1);
        }
    }

    // ============================================================
    // MOVEMENT
    // ============================================================

    // HEIGHT
    public void OnRaiseTableButtonDown()
    {
        if (handControl != null)
        {
            handControl.ChangeTableHeight(1);
        }
    }

    public void OnLowerTableButtonDown()
    {
        if (handControl != null)
        {
            handControl.ChangeTableHeight(-1);
        }
    }

    // LONGITUDINAL
    public void OnSlideForwardButtonDown()
    {
        if (handControl != null)
        {
            handControl.MoveTable(-1);
        }
    }

    public void OnSlideBackwardButtonDown()
    {
        if (handControl != null)
        {
            handControl.MoveTable(1);
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
    // ON/OFF
    // ============================================================
    public void OnPowerButtonDown(Button button)
    {
        if (handControl != null)
        {
            handControl.PowerOnOff();

            if (button.image.color == Color.white)
                button.image.color = Color.red;
            else
                button.image.color = Color.white;
        }
    }

    // ============================================================
    // ZERO POSITION
    // ============================================================
    public void OnLevelZeroButtonDown()
    {
        if (handControl != null)
        {
            handControl.LevelZero();
        }
    }

    // ============================================================
    // NORMAL / REVERSE
    // ============================================================
    public void OnNormalPositionButtonDown()
    {
        if (handControl != null)
        {
            handControl.NormalPosition();
        }
    }

    public void OnReversePositionButtonDown()
    {
        if (handControl != null)
        {
            handControl.ReversePosition();
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