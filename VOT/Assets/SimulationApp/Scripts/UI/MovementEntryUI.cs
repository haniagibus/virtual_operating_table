using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OperatingTable;

public class MovementEntryUI : MonoBehaviour
{
    public Text nameText;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    public MovementAxis axis;
    public SelectorWindow selector;

    public float lastX = 0f;
    public float lastY = 0f;
    public float lastZ = 0f;

    private bool suppressEvent = false;

    // NOWE: flagi śledzące czy slider jest wciśnięty
    private bool isPressingX = false;
    private bool isPressingY = false;
    private bool isPressingZ = false;

    public float stepSize = 5f; // krok slidera

    void Start()
    {
        if (sliderX != null)
        {
            lastX = sliderX.value;
            sliderX.onValueChanged.AddListener(OnSliderXChanged);
            AddSliderListeners(sliderX, "X");
        }

        if (sliderY != null)
        {
            lastY = sliderY.value;
            sliderY.onValueChanged.AddListener(OnSliderYChanged);
            AddSliderListeners(sliderY, "Y");
        }

        if (sliderZ != null)
        {
            lastZ = sliderZ.value;
            sliderZ.onValueChanged.AddListener(OnSliderZChanged);
            AddSliderListeners(sliderZ, "Z");
        }
    }

    void AddSliderListeners(Slider slider, string axisName)
    {
        EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = slider.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // Kliknięcie - animacja do przodu
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerDown na slider " + axisName);
            OnSliderPressed(axisName); 
        });
        trigger.triggers.Add(entryDown);

        // Puszczenie - animacja do tyłu
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerUp na slider " + axisName);
            OnSliderReleased(axisName); 
        });
        trigger.triggers.Add(entryUp);

        // Exit - tylko jeśli slider był wciśnięty
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerExit na slider " + axisName);
            OnSliderExit(axisName); 
        });
        trigger.triggers.Add(entryExit);

        Debug.Log("[MovementEntryUI] Dodano EventTrigger do slidera " + axisName);
    }

    void OnSliderPressed(string axisName)
    {
        Debug.Log("[MovementEntryUI] OnSliderPressed wywołane dla " + axisName);
        
        // Ustaw flagę wciśnięcia
        SetPressing(axisName, true);
        
        if (selector == null || selector.currentSelectedElement == null)
        {
            Debug.LogWarning("[MovementEntryUI] Brak selector lub elementu!");
            return;
        }
        
        Debug.Log("[MovementEntryUI] Naciśnięto slider " + axisName + " - animacja do przodu");
        selector.currentSelectedElement.PlayAnimationForAxis(axis, axisName, true);
    }

    void OnSliderReleased(string axisName)
    {
        Debug.Log("[MovementEntryUI] OnSliderReleased wywołane dla " + axisName);
        
        // Sprawdź czy slider był faktycznie wciśnięty
        if (!IsPressing(axisName))
        {
            Debug.Log("[MovementEntryUI] Slider nie był wciśnięty, ignoruję release");
            return;
        }
        
        // Wyzeruj flagę
        SetPressing(axisName, false);
        
        if (selector == null || selector.currentSelectedElement == null)
        {
            Debug.LogWarning("[MovementEntryUI] Brak selector lub elementu!");
            return;
        }
        
        Debug.Log("[MovementEntryUI] Puszczono slider " + axisName + " - animacja do tyłu");
        selector.currentSelectedElement.PlayAnimationForAxis(axis, axisName, false);
    }

    void OnSliderExit(string axisName)
    {
        // Wywołaj release tylko jeśli slider był wciśnięty
        if (IsPressing(axisName))
        {
            Debug.Log("[MovementEntryUI] Exit podczas wciskania - wywołuję release");
            OnSliderReleased(axisName);
        }
    }

    // Pomocnicze metody do zarządzania flagami
    void SetPressing(string axisName, bool value)
    {
        if (axisName == "X") isPressingX = value;
        else if (axisName == "Y") isPressingY = value;
        else if (axisName == "Z") isPressingZ = value;
    }

    bool IsPressing(string axisName)
    {
        if (axisName == "X") return isPressingX;
        else if (axisName == "Y") return isPressingY;
        else if (axisName == "Z") return isPressingZ;
        return false;
    }

    void OnSliderXChanged(float value)
    {
        if (suppressEvent) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderX.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastX;
        lastX = stepped;
        axis.MoveWithVector3(Vector3.right, delta);
    }

    void OnSliderYChanged(float value)
    {
        if (suppressEvent) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderY.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastY;
        lastY = stepped;

        axis.MoveWithVector3(Vector3.up, delta);
    }

    void OnSliderZChanged(float value)
    {
        if (suppressEvent) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderZ.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastZ;
        lastZ = stepped;

        axis.MoveWithVector3(Vector3.forward, delta);
    }
}