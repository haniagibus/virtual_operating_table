using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using OperatingTable;
using System.Collections;

public class PivotEntryUI : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
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

    public float stepSize = 50f;

    private bool isDraggingX = false;
    private bool isDraggingY = false;
    private bool isDraggingZ = false;

    void Start()
    {
        if (sliderX != null) { lastX = sliderX.value; sliderX.onValueChanged.AddListener(OnSliderXChanged); }
        if (sliderY != null) { lastY = sliderY.value; sliderY.onValueChanged.AddListener(OnSliderYChanged); }
        if (sliderZ != null) { lastZ = sliderZ.value; sliderZ.onValueChanged.AddListener(OnSliderZChanged); }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        // Sprawdzamy który slider został złapany
        if (eventData.pointerPress == sliderX.gameObject) { isDraggingX = true; TryPlayAnimation("X"); }
        if (eventData.pointerPress == sliderY.gameObject) { isDraggingY = true; TryPlayAnimation("Y"); }
        if (eventData.pointerPress == sliderZ.gameObject) { isDraggingZ = true; TryPlayAnimation("Z"); }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Resetujemy flagi po puszczeniu
        if (eventData.pointerPress == sliderX.gameObject) isDraggingX = false;
        if (eventData.pointerPress == sliderY.gameObject) isDraggingY = false;
        if (eventData.pointerPress == sliderZ.gameObject) isDraggingZ = false;
    }

    void TryPlayAnimation(string axis)
    {
        if (!isAnimating && selector.currentSelectedElement != null)
        {
            RotationPivot pivot = this.pivot;
            bool forward = true; // można ustalić wg potrzeby
            StartCoroutine(PlayPivotAnimation(selector.currentSelectedElement, pivot, axis, forward));
        }
    }

    IEnumerator PlayPivotAnimation(TableElement element, RotationPivot pivot, string axis, bool forward)
    {
        if (element == null || element.GetComponent<Animator>() == null) yield break;

        isAnimating = true;
        sliderX.interactable = false;
        sliderY.interactable = false;
        sliderZ.interactable = false;

        element.PlayAnimationForPivot(pivot, axis, forward);

        var anim = element.elementAnimations.Find(a => a.pivot == pivot && a.axisName == axis);
        if (anim != null && anim.clip != null)
        {
            float duration = anim.clip.length / Mathf.Abs(anim.speed);
            yield return new WaitForSeconds(duration);
        }

        isAnimating = false;
        sliderX.interactable = true;
        sliderY.interactable = true;
        sliderZ.interactable = true;
    }

    void OnSliderXChanged(float value)
    {
        if (suppressEvent) return;
        float stepped = Mathf.Round(value / stepSize) * stepSize;
        suppressEvent = true; sliderX.value = stepped; suppressEvent = false;
        float delta = stepped - lastX; lastX = stepped;
        pivot.RotateWithVector3(Vector3.right, delta);
    }

    void OnSliderYChanged(float value)
    {
        if (suppressEvent) return;
        float stepped = Mathf.Round(value / stepSize) * stepSize;
        suppressEvent = true; sliderY.value = stepped; suppressEvent = false;
        float delta = stepped - lastY; lastY = stepped;
        pivot.RotateWithVector3(Vector3.up, delta);
    }

    void OnSliderZChanged(float value)
    {
        if (suppressEvent) return;
        float stepped = Mathf.Round(value / stepSize) * stepSize;
        suppressEvent = true; sliderZ.value = stepped; suppressEvent = false;
        float delta = stepped - lastZ; lastZ = stepped;
        pivot.RotateWithVector3(Vector3.forward, delta);
    }
}
