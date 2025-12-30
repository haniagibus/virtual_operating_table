using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperatingTable;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Dropdown dropdown;
    public Toggle visibilityToggle;

    [Header("Elementy stołu")]
    public List<TableElement> tableElements = new List<TableElement>();

    [Header("Pivot System")]
    public Transform pivotListContainer;
    public GameObject pivotEntryPrefab;

    // [Header("Movement System")]
    // public Transform movementListContainer;
    // public GameObject movementEntryPrefab;

    private TableElement currentSelectedElement = null;
    private bool suppressToggleCallback = false;

    void Start()
    {
        panel.SetActive(false);
        UpdateDropdown();

        dropdown.onValueChanged.AddListener(OnDropdownChange);
        visibilityToggle.onValueChanged.AddListener(OnVisibilityToggleChanged);

        if (tableElements.Count > 0)
        {
            dropdown.value = Mathf.Clamp(dropdown.value, 0, tableElements.Count - 1);
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

        foreach (var element in tableElements)
        {
            if (element != null)
                names.Add(element.elementName);
            else
                names.Add("<null>");
        }

        dropdown.AddOptions(names);
    }

    void OnDropdownChange(int index)
    {
        if (index < 0 || index >= tableElements.Count)
        {
            currentSelectedElement = null;
            visibilityToggle.interactable = false;
            ClearPivotUI();
            // ClearMovementUI();
            return;
        }

        currentSelectedElement = tableElements[index];

        if (currentSelectedElement == null)
        {
            visibilityToggle.interactable = false;
            ClearPivotUI();
            // ClearMovementUI();
            return;
        }

        visibilityToggle.interactable = true;
        Debug.Log("Wybrano element: " + currentSelectedElement.elementName);

        UpdateToggleState();
        BuildPivotUI();
        // BuildMovementUI();
    }

    void UpdateToggleState()
    {
        bool isAttached = currentSelectedElement.isAttached;

        suppressToggleCallback = true;
        visibilityToggle.isOn = isAttached;
        suppressToggleCallback = false;
    }

    // ----------------------------------------------------
    // VISIBILITY SYSTEM
    // ----------------------------------------------------

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;
        if (currentSelectedElement == null) return;

        currentSelectedElement.SetAttached(isOn);
    }

    // ----------------------------------------------------
    // PIVOT UI SYSTEM
    // ----------------------------------------------------

    void BuildPivotUI()
    {
        ClearPivotUI();

        if (currentSelectedElement == null || !currentSelectedElement.HasRotationPivots())
        {
            string elementName = currentSelectedElement != null ? currentSelectedElement.elementName : "null";
            Debug.LogWarning(elementName + " nie ma zdefiniowanych pivotów!");
            return;
        }

        foreach (var pivot in currentSelectedElement.rotationPivots)
        {
            if (pivot == null) continue;

            GameObject entryObj = Instantiate(pivotEntryPrefab, pivotListContainer);
            PivotEntryUI entry = entryObj.GetComponent<PivotEntryUI>();

            entry.pivot = pivot;
            entry.selector = this;
            entry.nameText.text = pivot.pivotName;

            // Konfiguruj slider X
            if (pivot.allowX && entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(true);
                entry.sliderX.minValue = pivot.minAngleX;
                entry.sliderX.maxValue = pivot.maxAngleX;

                float angleX = pivot.currentAngleX;
                entry.sliderX.value = angleX;
                entry.lastX = angleX;

                Debug.Log(pivot.pivotName + " - Slider X: min=" + pivot.minAngleX +
                         ", max=" + pivot.maxAngleX + ", value=" + angleX);
            }
            else if (entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(false);
            }

            // Konfiguruj slider Y
            if (pivot.allowY && entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(true);
                entry.sliderY.minValue = pivot.minAngleY;
                entry.sliderY.maxValue = pivot.maxAngleY;

                float angleY = pivot.currentAngleY;
                entry.sliderY.value = angleY;
                entry.lastY = angleY;

                Debug.Log(pivot.pivotName + " - Slider Y: min=" + pivot.minAngleY +
                         ", max=" + pivot.maxAngleY + ", value=" + angleY);
            }
            else if (entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(false);
            }

            // Konfiguruj slider Z
            if (pivot.allowZ && entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(true);
                entry.sliderZ.minValue = pivot.minAngleZ;
                entry.sliderZ.maxValue = pivot.maxAngleZ;

                float angleZ = pivot.currentAngleZ;
                entry.sliderZ.value = angleZ;
                entry.lastZ = angleZ;

                Debug.Log(pivot.pivotName + " - Slider Z: min=" + pivot.minAngleZ +
                         ", max=" + pivot.maxAngleZ + ", value=" + angleZ);
            }
            else if (entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(false);
            }
        }
    }

    void ClearPivotUI()
    {
        foreach (Transform child in pivotListContainer)
            Destroy(child.gameObject);
    }

    // // ----------------------------------------------------
    // // MOVEMENT UI SYSTEM
    // // ----------------------------------------------------

    // void BuildMovementUI()
    // {
    //     ClearMovementUI();

    //     if (currentSelectedElement == null || !currentSelectedElement.HasMovementAxes())
    //     {
    //         string elementName = currentSelectedElement != null ? currentSelectedElement.elementName : "null";
    //         Debug.LogWarning(elementName + " nie ma zdefiniowanych osi ruchu!");
    //         return;
    //     }

    //     foreach (var axis in currentSelectedElement.movementAxes)
    //     {
    //         if (axis == null) continue;

    //         var entryObj = Instantiate(movementEntryPrefab, movementListContainer);
    //         var entry = entryObj.GetComponent<MovementEntryUI>();

    //         entry.axis = axis;
    //         entry.selector = this;
    //         entry.nameText.text = axis.axisName;

    //         // Konfiguruj slider
    //         if (entry.slider != null)
    //         {
    //             entry.slider.minValue = axis.minPosition;
    //             entry.slider.maxValue = axis.maxPosition;
    //             entry.slider.value = axis.currentPosition;
    //             entry.lastPosition = axis.currentPosition;

    //             Debug.Log(axis.axisName + " - Slider: min=" + axis.minPosition +
    //                      ", max=" + axis.maxPosition + ", value=" + axis.currentPosition);
    //         }
    //     }
    // }

    // void ClearMovementUI()
    // {
    //     if (movementListContainer == null) return;

    //     foreach (Transform child in movementListContainer)
    //         Destroy(child.gameObject);
    // }
}