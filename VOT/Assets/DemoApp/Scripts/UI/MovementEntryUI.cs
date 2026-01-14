using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using VirtualOperatingTable;

public class MovementEntryUI : MonoBehaviour
{
    public Text nameText;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    public MovementAxis axis;
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
        EventTrigger trigger = slider.GetComponent<EventTrigger>();
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

    void OnSliderPressed(string axisName)
    {
        SetPressing(axisName, true);

        TableElement element = GetCurrentElement();
        if (element == null) return;

        ElementAnimation anim = element.GetAnimationForAxis(axis);
        if (anim == null) return;

        element.Play(anim, true); 
    }

    void OnSliderReleased(string axisName)
    {
        if (!IsPressing(axisName)) return;
        SetPressing(axisName, false);

        TableElement element = GetCurrentElement();
        if (element == null) return;

        ElementAnimation anim = element.GetAnimationForAxis(axis);
        if (anim == null) return;

        element.Play(anim, false); 
    }

    void OnSliderExit(string axisName)
    {
        if (IsPressing(axisName))
            OnSliderReleased(axisName);
    }

    void OnSliderXChanged(float value)
    {
        if (suppressEvent) return;
        ApplyMovement(sliderX, ref lastX, value, Vector3.right);
    }

    void OnSliderYChanged(float value)
    {
        if (suppressEvent) return;
        ApplyMovement(sliderY, ref lastY, value, Vector3.up);
    }

    void OnSliderZChanged(float value)
    {
        if (suppressEvent) return;
        ApplyMovement(sliderZ, ref lastZ, value, Vector3.forward);
    }

    void ApplyMovement(Slider slider, ref float lastValue, float newValue, Vector3 dir)
    {
        float stepped = Mathf.Round(newValue / stepSize) * stepSize;

        suppressEvent = true;
        slider.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastValue;
        lastValue = stepped;

        axis.MoveWithVector3(dir, delta);
    }

    TableElement GetCurrentElement()
    {
        return tableElementControl != null ? tableElementControl.currentSelectedElement : null;
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
