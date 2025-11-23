using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Dropdown dropdown;
    public Toggle visibilityToggle;

    // [Header("Rotation Sliders")]
    // public Slider rotationY;

    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    [Header("Pivot System")]
    public Transform pivotListContainer;     // panel w UI
    public GameObject pivotEntryPrefab;      // prefab PivotEntryUI

    private Transform currentSelectedElement = null;
    private RotationPivot currentPivot = null;
    private List<RotationPivot> currentPivots = new List<RotationPivot>();

    private bool suppressToggleCallback = false;
    private bool suppressSliderCallback = false;

    void Start()
    {
        panel.SetActive(false);
        UpdateDropdown();

        dropdown.onValueChanged.AddListener(OnDropdownChange);
        visibilityToggle.onValueChanged.AddListener(OnVisibilityToggleChanged);
        //rotationY.onValueChanged.AddListener(OnRotateY);

        if (tableParts.Count > 0)
        {
            dropdown.value = Mathf.Clamp(dropdown.value, 0, tableParts.Count - 1);
            OnDropdownChange(dropdown.value);
        }
        else
        {
            suppressToggleCallback = true;
            visibilityToggle.isOn = false;
            suppressToggleCallback = false;
            visibilityToggle.interactable = false;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            panel.SetActive(!panel.activeSelf);

            if (panel.activeSelf)
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

    void UpdateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var part in tableParts)
            names.Add(part != null ? part.name : "<null>");

        dropdown.AddOptions(names);
    }

    void OnDropdownChange(int index)
    {
        if (index < 0 || index >= tableParts.Count)
        {
            currentSelectedElement = null;
            visibilityToggle.interactable = false;
            return;
        }

        currentSelectedElement = tableParts[index];

        if (currentSelectedElement == null)
        {
            visibilityToggle.interactable = false;
            return;
        }

        visibilityToggle.interactable = true;
        Debug.Log("Wybrano element: " + currentSelectedElement.name);

        LoadPivotsForSelected();
        UpdateToggleState();
        //UpdateSlidersFromRotation();
        BuildPivotUI();

    }

    void LoadPivotsForSelected()
    {
        currentPivots.Clear();
        currentPivot = null;

        RotationConfig cfg = currentSelectedElement.GetComponent<RotationConfig>();
        if (cfg != null && cfg.pivots.Count > 0)
        {
            currentPivots.AddRange(cfg.pivots);
            currentPivot = currentPivots[0]; // wybierz pierwszy pivot
            Debug.Log("Aktywny pivot: " + currentPivot.pivotName);


        }
        else
        {
            Debug.LogWarning(currentSelectedElement.name + " nie ma zdefiniowanych pivotów!");
        }
    }

    void UpdateToggleState()
    {
        bool isActive = currentSelectedElement.gameObject.activeSelf;

        suppressToggleCallback = true;
        visibilityToggle.isOn = isActive;
        suppressToggleCallback = false;
    }

    // ----------------------------------------------------
    // ROTATION SYSTEM
    // ----------------------------------------------------

    // void UpdateSlidersFromRotation()
    // {
    //     if (currentSelectedElement == null) return;

    //     suppressSliderCallback = true;

    //     Vector3 rot = currentSelectedElement.localEulerAngles;
    //     rotationY.value = rot.y;

    //     suppressSliderCallback = false;
    // }

    void OnRotateY(float value)
    {
        if (suppressSliderCallback || currentSelectedElement == null) return;
        if (currentPivot == null) return;

        if (!currentPivot.allowY)
        {
            Debug.LogWarning("Pivot " + currentPivot.pivotName + " nie pozwala na rotację Y!");
            return;
        }

        float currentY = currentSelectedElement.localEulerAngles.y;
        float delta = value - currentY;

        RotateAroundPivot(currentPivot, delta, Vector3.up);

        Debug.Log(currentSelectedElement.name +
                  " obracany wokół pivotu '" + currentPivot.pivotName +
                  "' o ΔY = " + delta + "°");
    }

    void RotateAroundPivot(RotationPivot pivot, float angle, Vector3 axis)
    {
        if (currentSelectedElement == null) return;
        
        currentSelectedElement.RotateAround(
            pivot.transform.position,
            pivot.transform.TransformDirection(axis),
            angle
        );
    }

    // ----------------------------------------------------
    // VISIBILITY SYSTEM
    // ----------------------------------------------------

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;
        if (currentSelectedElement == null) return;

        currentSelectedElement.gameObject.SetActive(isOn);
    }

    void BuildPivotUI()
{
    foreach (Transform child in pivotListContainer)
        Destroy(child.gameObject);

    if (currentPivots.Count == 0)
        return;

    foreach (var pivot in currentPivots)
    {
        var entryObj = Instantiate(pivotEntryPrefab, pivotListContainer);
        var entry = entryObj.GetComponent<PivotEntryUI>();

        entry.pivot = pivot;
        entry.selector = this;
        entry.nameText.text = pivot.pivotName;

        entry.sliderX.gameObject.SetActive(pivot.allowX);
        entry.sliderY.gameObject.SetActive(pivot.allowY);
        entry.sliderZ.gameObject.SetActive(pivot.allowZ);

        if (pivot.allowX && entry.sliderX != null) entry.sliderX.value = 0;
        if (pivot.allowY && entry.sliderY != null) entry.sliderY.value = 0;
        if (pivot.allowZ && entry.sliderZ != null) entry.sliderZ.value = 0;
    }
}

public void RotatePivot(RotationPivot pivot, float value, Vector3 axis)
{
    if (currentSelectedElement == null) return;

    foreach (Transform child in pivot.transform)
    {
        child.RotateAround(
            pivot.transform.position,
            pivot.transform.TransformDirection(axis),
            value
        );
    }

    // currentSelectedElement.RotateAround(
    //     pivot.transform.position,
    //     pivot.transform.TransformDirection(axis),
    //     value
    // );
    Debug.Log(currentSelectedElement.name +
              " - dzieci pivotu " + pivot.pivotName +
              " obracane o " + value + "° (oś " + axis + ")");
    Debug.Log(currentSelectedElement.name +
              " obracany wokół " + pivot.pivotName +
              " o " + value + "° (oś " + axis + ")");
}


}
