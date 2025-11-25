using UnityEngine;
using UnityEngine.UI;

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

    void Start()
    {
        if (sliderX != null)
        {
            lastX = sliderX.value;
            sliderX.onValueChanged.AddListener(OnSliderXChanged);
        }

        if (sliderY != null)
        {
            lastY = sliderY.value;
            sliderY.onValueChanged.AddListener(OnSliderYChanged);
        }

        if (sliderZ != null)
        {
            lastZ = sliderZ.value;
            sliderZ.onValueChanged.AddListener(OnSliderZChanged);
        }
    }

    void OnSliderXChanged(float value)
    {
        float delta = value - lastX;
        lastX = value;
        selector.RotatePivot(pivot, delta, Vector3.right);
    }

    void OnSliderYChanged(float value)
    {
        float delta = value - lastY;
        lastY = value;
        selector.RotatePivot(pivot, delta, Vector3.up);
    }

    void OnSliderZChanged(float value)
    {
        float delta = value - lastZ;
        lastZ = value;
        selector.RotatePivot(pivot, delta, Vector3.forward);
    }
}