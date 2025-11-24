using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HandControlPanel : MonoBehaviour
{
    [Header("Hand Control Panel")]
    public GameObject handControlPanel;              // przypisz panel pilota (UI) w Inspectorze
    public KeyCode toggleKey = KeyCode.P;      // klawisz do otwierania/zamykania panelu
    public bool startClosed = true;            // czy panel ma być zamknięty na starcie

    [Header("Optional")]
    public EventSystem eventSystem;            // opcjonalnie: EventSystem (jeśli null, użyje EventSystem.current)
    public bool autoSelectFirst = true;        // czy ustawić pierwszy Selectable po otwarciu

    [Header("Table Reset")]
    public TableStateManager tableStateManager;       
    public KeyCode resetKey = KeyCode.R;           //resetuj ustawienia stołu (normal)
    private bool isOpen = false;

    
    void Awake()
    {
        if (handControlPanel == null)
        {
            Debug.LogError("[PilotPanelToggle] pilotPanel nie przypisany w Inspectorze!");
        }

        if (eventSystem == null)
            eventSystem = EventSystem.current;
    }

    void Start()
    {
        if (handControlPanel != null)
        {
            isOpen = !startClosed;
            SetPanelState(!startClosed); // jeśli startClosed = true -> zamknij
            SetCursorForPanel(isOpen);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            ToggleHandControlPanel();
        }

        if (Input.GetKeyDown(resetKey))
        {
            if (tableStateManager != null)
            {
                Debug.Log("Naciśnięto klawisz R - resetuję stół!");
                tableStateManager.ResetToNormalPosition();
            }
            else
            {
                Debug.LogWarning("TableStateManager nie przypisany!");
            }
        }
    }

    public void ToggleHandControlPanel()
    {
        if (handControlPanel == null) return;

        isOpen = !isOpen;
        SetPanelState(isOpen);
        SetCursorForPanel(isOpen);

        if (isOpen && autoSelectFirst && eventSystem != null)
        {
            // ustaw pierwszy Selectable wewnątrz panelu, aby umożliwić sterowanie klawiaturą
            var firstSelectable = handControlPanel.GetComponentInChildren<Selectable>();
            if (firstSelectable != null)
                eventSystem.SetSelectedGameObject(firstSelectable.gameObject);
        }
        else if (!isOpen && eventSystem != null)
        {
            // usuń zaznaczenie gdy zamykamy
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
