using UnityEngine;
using UnityEngine.UI;
using OperatingTable;
using System.Collections;

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
    private bool isAnimating = false; // flaga animacji

    public float stepSize = 5f; // krok slidera

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
        if (suppressEvent || isAnimating) return;

        float stepped = Mathf.Round(value / stepSize) * stepSize;

        suppressEvent = true;
        sliderX.value = stepped;
        suppressEvent = false;

        float delta = stepped - lastX;
        lastX = stepped;
        axis.MoveWithVector3(Vector3.right, delta);

        if (selector.currentSelectedElement != null)
        {
            StartCoroutine(PlayAxisAnimation(selector.currentSelectedElement, axis, "X", delta >= 0));
        }
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

        axis.MoveWithVector3(Vector3.up, delta);

        if (selector.currentSelectedElement != null)
        {
            StartCoroutine(PlayAxisAnimation(selector.currentSelectedElement, axis, "Y", delta >= 0));
        }
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

        axis.MoveWithVector3(Vector3.forward, delta);

        if (selector.currentSelectedElement != null)
        {
            StartCoroutine(PlayAxisAnimation(selector.currentSelectedElement, axis, "Z", delta >= 0));
        }
    }

    IEnumerator PlayAxisAnimation(TableElement element, MovementAxis axisObj, string axis, bool forward)
    {
        if (element == null || element.GetComponent<Animator>() == null) yield break;

        isAnimating = true;
        sliderX.interactable = false;
        sliderY.interactable = false;
        sliderZ.interactable = false;

        element.PlayAnimationForAxis(axisObj, axis, forward);

        var anim = element.elementAnimations.Find(a => a.axis == axisObj && a.axisName == axis);
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
}
