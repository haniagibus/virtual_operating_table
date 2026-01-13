using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OperatingTable;
using System.Collections;

public class PivotEntryUI : MonoBehaviour
{
    public Text nameText;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    public RotationPivot pivot;
    public SelectorWindow selector;

    public float lastX = 0f;
    public float lastY = 0f;
    public float lastZ = 0f;

    private bool suppressEvent = false;
    private bool isAnimating = false;

    // NOWE: flagi śledzące czy slider jest wciśnięty
    private bool isPressingX = false;
    private bool isPressingY = false;
    private bool isPressingZ = false;

    public float stepSize = 5f;

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

    void AddSliderListeners(Slider slider, string pivotName)
    {
        EventTrigger trigger = slider.gameObject.GetComponent<EventTrigger>();
        if (trigger == null)
            trigger = slider.gameObject.AddComponent<EventTrigger>();

        trigger.triggers.Clear();

        // Kliknięcie - animacja do przodu
        EventTrigger.Entry entryDown = new EventTrigger.Entry();
        entryDown.eventID = EventTriggerType.PointerDown;
        entryDown.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerDown na slider " + pivotName);
            OnSliderPressed(pivotName); 
        });
        trigger.triggers.Add(entryDown);

        // Puszczenie - animacja do tyłu
        EventTrigger.Entry entryUp = new EventTrigger.Entry();
        entryUp.eventID = EventTriggerType.PointerUp;
        entryUp.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerUp na slider " + pivotName);
            OnSliderReleased(pivotName); 
        });
        trigger.triggers.Add(entryUp);

        // Exit - tylko jeśli slider był wciśnięty
        EventTrigger.Entry entryExit = new EventTrigger.Entry();
        entryExit.eventID = EventTriggerType.PointerExit;
        entryExit.callback.AddListener((data) => { 
            Debug.Log("[EVENT] PointerExit na slider " + pivotName);
            OnSliderExit(pivotName); 
        });
        trigger.triggers.Add(entryExit);

        Debug.Log("[PivotEntryUI] Dodano EventTrigger do slidera " + pivotName);
    }

    void OnSliderPressed(string pivotName)
    {
        Debug.Log("[PivotEntryUI] OnSliderPressed wywołane dla " + pivotName);
        
        // Ustaw flagę wciśnięcia
        SetPressing(pivotName, true);
        
        if (selector == null || selector.currentSelectedElement == null)
        {
            Debug.LogWarning("[PivotEntryUI] Brak selector lub elementu!");
            return;
        }
        
        Debug.Log("[PivotEntryUI] Naciśnięto slider " + pivotName + " - animacja do przodu");
        selector.currentSelectedElement.PlayAnimationForPivot(pivot, pivotName, true);
    }

    void OnSliderReleased(string pivotName)
    {
        Debug.Log("[PivotEntryUI] OnSliderReleased wywołane dla " + pivotName);
        
        // Sprawdź czy slider był faktycznie wciśnięty
        if (!IsPressing(pivotName))
        {
            Debug.Log("[PivotEntryUI] Slider nie był wciśnięty, ignoruję release");
            return;
        }
        
        // Wyzeruj flagę
        SetPressing(pivotName, false);
        
        if (selector == null || selector.currentSelectedElement == null)
        {
            Debug.LogWarning("[PivotEntryUI] Brak selector lub elementu!");
            return;
        }
        
        Debug.Log("[PivotEntryUI] Puszczono slider " + pivotName + " - animacja do tyłu");
        selector.currentSelectedElement.PlayAnimationForPivot(pivot, pivotName, false);
    }

    void OnSliderExit(string pivotName)
    {
        // Wywołaj release tylko jeśli slider był wciśnięty
        if (IsPressing(pivotName))
        {
            Debug.Log("[PivotEntryUI] Exit podczas wciskania - wywołuję release");
            OnSliderReleased(pivotName);
        }
    }

    // Pomocnicze metody do zarządzania flagami
    void SetPressing(string pivotName, bool value)
    {
        if (pivotName == "X") isPressingX = value;
        else if (pivotName == "Y") isPressingY = value;
        else if (pivotName == "Z") isPressingZ = value;
    }

    bool IsPressing(string pivotName)
    {
        if (pivotName == "X") return isPressingX;
        else if (pivotName == "Y") return isPressingY;
        else if (pivotName == "Z") return isPressingZ;
        return false;
    }

    void OnSliderXChanged(float value)
    {
        if (suppressEvent || isAnimating) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderX.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastX;
        lastX = stepped;
        pivot.RotateWithVector3(Vector3.right, delta);
    }

    void OnSliderYChanged(float value)
    {
        if (suppressEvent || isAnimating) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderY.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastY;
        lastY = stepped;

        pivot.RotateWithVector3(Vector3.up, delta);
    }

    void OnSliderZChanged(float value)
    {
        if (suppressEvent || isAnimating) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderZ.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastZ;
        lastZ = stepped;

        pivot.RotateWithVector3(Vector3.forward, delta);
    }
}