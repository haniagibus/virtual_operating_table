using UnityEngine;
using UnityEngine.UI;

public class PivotEntryUI : MonoBehaviour
{
    public Text pivotLabel;

    public Slider sliderX;
    public Slider sliderY;
    public Slider sliderZ;

    public RotationPivot pivot;
    public SelectorWindow selector;

    void Start()
    {
        if (sliderX != null)
            sliderX.onValueChanged.AddListener(v => selector.RotatePivot(pivot, v, Vector3.right));

        if (sliderY != null)
            sliderY.onValueChanged.AddListener(v => selector.RotatePivot(pivot, v, Vector3.up));

        if (sliderZ != null)
            sliderZ.onValueChanged.AddListener(v => selector.RotatePivot(pivot, v, Vector3.forward));
    }
}
