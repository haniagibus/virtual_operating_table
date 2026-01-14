using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VirtualOperatingTable;

public class PivotEntryUI : MonoBehaviour
{
    public Text nameText;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    public RotationPivot pivot;
    public TableElementControlPanel tableElementControl;

    public float lastX = 0f;
    public float lastY = 0f;
    public float lastZ = 0f;

    public float stepSize = 5f;

    private bool suppressEvent = false;

    private bool isPressingX = false;
    private bool isPressingY = false;
    private bool isPressingZ = false;

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

        trigger.triggers.Add(CreateEntry(EventTriggerType.PointerDown, _ => OnSliderPressed(axisName)));
        trigger.triggers.Add(CreateEntry(EventTriggerType.PointerUp, _ => OnSliderReleased(axisName)));
        trigger.triggers.Add(CreateEntry(EventTriggerType.PointerExit, _ => OnSliderExit(axisName)));
    }

    EventTrigger.Entry CreateEntry(EventTriggerType type, UnityEngine.Events.UnityAction<BaseEventData> callback)
    {
        EventTrigger.Entry e = new EventTrigger.Entry { eventID = type };
        e.callback.AddListener(callback);
        return e;
    }

    // SLIDER PRESS LOGIC
    void OnSliderPressed(string axisName)
    {
        SetPressing(axisName, true);

        TableElement element = GetCurrentElement();
        if (element == null) return;

        ElementAnimation anim = element.GetAnimationForPivot(pivot);
        if (anim == null) return;

        element.Play(anim, true); // odtwarza wszystkie clipy w sekwencji
    }

    void OnSliderReleased(string axisName)
    {
        if (!IsPressing(axisName)) return;
        SetPressing(axisName, false);

        TableElement element = GetCurrentElement();
        if (element == null) return;

        ElementAnimation anim = element.GetAnimationForPivot(pivot);
        if (anim == null) return;

        element.Play(anim, false); // odtwarza wszystkie clipy w odwrotnym kierunku
    }

    void OnSliderExit(string axisName)
    {
        if (IsPressing(axisName))
            OnSliderReleased(axisName);
    }

    // SLIDER VALUE CHANGE
    void OnSliderXChanged(float value)
    {
        if (suppressEvent) return;
        ApplyRotation(sliderX, ref lastX, value, Vector3.right);
    }

    void OnSliderYChanged(float value)
    {
        if (suppressEvent) return;
        ApplyRotation(sliderY, ref lastY, value, Vector3.up);
    }

    void OnSliderZChanged(float value)
    {
        if (suppressEvent) return;
        ApplyRotation(sliderZ, ref lastZ, value, Vector3.forward);
    }

    void ApplyRotation(Slider slider, ref float lastValue, float value, Vector3 axis)
    {
        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        slider.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastValue;
        lastValue = stepped;

        pivot.RotateWithVector3(axis, delta);
    }

    TableElement GetCurrentElement()
    {
        return tableElementControl != null
            ? tableElementControl.currentSelectedElement
            : null;
    }

    void SetPressing(string axisName, bool value)
    {
        if (axisName == "X") isPressingX = value;
        else if (axisName == "Y") isPressingY = value;
        else if (axisName == "Z") isPressingZ = value;
    }

    bool IsPressing(string axisName)
    {
        if (axisName == "X") return isPressingX;
        if (axisName == "Y") return isPressingY;
        if (axisName == "Z") return isPressingZ;
        return false;
    }
}
