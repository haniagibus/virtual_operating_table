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

    private TableElement currentSelectedElement = null;
    private RotationPivot currentPivot = null;
    private List<RotationPivot> currentPivots = new List<RotationPivot>();

    private bool suppressToggleCallback = false;

    void Start()
    {
        panel.SetActive(false);

        // automatyczne znalezienie elementów
        AutoFindTableElements();

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

    public void AutoFindTableElements()
    {
        tableElements.Clear();

        TableElement[] foundElements = GetComponentsInChildren<TableElement>(true);
        tableElements.AddRange(foundElements);

        Debug.Log("Znaleziono " + tableElements.Count + " elementów stołu.");

        UpdateDropdown();
    }

    void UpdateDropdown()
    {
        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var element in tableElements)
            names.Add(element != null ? element.displayName : "<null>");

        dropdown.AddOptions(names);
    }

    void OnDropdownChange(int index)
    {
        if (index < 0 || index >= tableElements.Count)
        {
            currentSelectedElement = null;
            visibilityToggle.interactable = false;
            return;
        }

        currentSelectedElement = tableElements[index];

        if (currentSelectedElement == null)
        {
            visibilityToggle.interactable = false;
            return;
        }

        visibilityToggle.interactable = true;
        Debug.Log("Wybrano element: " + currentSelectedElement.displayName);

        LoadPivotsForSelected();
        UpdateToggleState();
        BuildPivotUI();
    }

    void LoadPivotsForSelected()
    {
        currentPivots.Clear();

        if (currentSelectedElement.rotationPivots.Count > 0)
        {
            currentPivots.AddRange(currentSelectedElement.rotationPivots);

        }
        else
        {
            Debug.LogWarning(currentSelectedElement.name + " nie ma zdefiniowanych pivotów!");
        }
    }

    void UpdateToggleState()
    {
        bool isActive = currentSelectedElement.isAttached;

        suppressToggleCallback = true;
        visibilityToggle.isOn = isActive;
        suppressToggleCallback = false;
    }

    // ----------------------------------------------------
    // VISIBILITY SYSTEM
    // ----------------------------------------------------

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;
        if (currentSelectedElement == null) return;

        currentSelectedElement.AttachDetach(isOn);
    }

    // ----------------------------------------------------
    // PIVOT UI SYSTEM
    // ----------------------------------------------------

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
            else
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
            else
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
            else
            {
                entry.sliderZ.gameObject.SetActive(false);
            }
        }
    }


}