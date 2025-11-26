using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
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

    [Header("Specific Pivots")]
    private Coroutine currentTiltCoroutine = null;
    private bool isTilting =false;

    public RotationPivot tableBackPartUpperRotate; 
    public RotationPivot tableBackPartLowerRotate;
    public RotationPivot tableRotation;

    

    //ograniczenia stołu
    private float maxBackTilt = 90f;
    
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
    }

    // ----------------------------------------------------
    // Level zero button
    // ----------------------------------------------------
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

    // ----------------------------------------------------
    // Tilt functiions
    // ----------------------------------------------------
    public void TiltStart(RotationPivot pivot){
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

    public void TiltStop(RotationPivot pivot){
        isTilting= false;
        Debug.Log("Zatrzymuję obracanie");

        if (currentTiltCoroutine != null)
        {
            StopCoroutine(currentTiltCoroutine);
            currentTiltCoroutine = null;
        }
    }

    // ----------------------------------------------------
    // Tilt back buttons
    // ----------------------------------------------------
    public void TiltBackUpStart(){
        Debug.Log("Podnoszę górną część stołu");
        TiltStart(tableBackPartUpperRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
        tableBackPartUpperRotate, 
        Vector3.right,  // oś X
        1,              // kierunek: 1 = dodatni, -1 = ujemny
        2f              // krok co x stopni
    ));

    }

    public void TiltBackDownStart(){
        isTilting = true;
        Debug.Log("Opuszczam górną część stołu");
        TiltStart(tableBackPartUpperRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartUpperRotate,
            Vector3.right,
            -1,
            2f
        ));
    }

    // ----------------------------------------------------
    // Tilt leg functiions
    // ----------------------------------------------------
    public void TiltLegsUpStart(){
        Debug.Log("Podnoszę dolną część stołu");
        TiltStart(tableBackPartLowerRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
        tableBackPartLowerRotate, 
        Vector3.right,  // oś X
        -1,              // kierunek: 1 = dodatni, -1 = ujemny
        2f              // krok co x stopni
    ));

    }

    public void TiltLegsDownStart(){
        isTilting = true;
        Debug.Log("Opuszczam dolną część stołu");
        TiltStart(tableBackPartLowerRotate);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableBackPartLowerRotate,
            Vector3.right,
            1,
            2f
        ));
    }

    // ----------------------------------------------------
    // Trendelenburg position
    // ----------------------------------------------------
    public void TrendelenburgPositionStart(){
        Debug.Log("Pozycja trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
        tableRotation, 
        Vector3.right,  // oś X
        -1,              // kierunek: 1 = dodatni, -1 = ujemny
        2f              // krok co x stopni
        ));
    }

    public void ReverseTrendelenburgPositionStart(){
        isTilting = true;
        Debug.Log("Odwrócona pozycja trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.right,
            1,
            2f
        ));
    }

    // ----------------------------------------------------
    // Tilt table
    // ----------------------------------------------------

    public void TiltTableRightStart(){
        Debug.Log("Pozycja trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
        tableRotation, 
        Vector3.up ,  // oś Y
        -1,              // kierunek: 1 = dodatni, -1 = ujemny
        2f              // krok co x stopni
        ));
    }

    public void TiltTableLeftStart(){
        isTilting = true;
        Debug.Log("Odwrócona pozycja trendelenburga");
        TiltStart(tableRotation);
        currentTiltCoroutine = StartCoroutine(TiltElement(
            tableRotation,
            Vector3.up,
            1,
            2f
        ));
    }

    private IEnumerator TiltElement(RotationPivot pivot, Vector3 axis, int direction, float step = 5f)
    {
        // Pobierz aktualny kąt z pivotu
        float currentAngle = pivot.currentAngle;
        
        while (isTilting)
        {
            // Oblicz nowy kąt
            float newAngle = currentAngle + (step * direction);
            
            // Sprawdź limity
            if (newAngle > pivot.maxAngle)
            {
               Debug.Log("Osiągnięto maksymalny kąt");
                break;
            }
            if (newAngle < pivot.minAngle)
            {
                Debug.Log("Osiągnięto minimalny kąt");
                break;
            }
            
            // Wykonaj obrót
            float rotationStep = step * direction;
            pivot.transform.Rotate(axis, rotationStep, Space.Self);
            
            // Zaktualizuj zapisany kąt
            currentAngle = newAngle;
            pivot.currentAngle = newAngle;
            
            yield return new WaitForSeconds(0.05f);
        }
        
        currentTiltCoroutine = null;
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
