using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using OperatingTable;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI - Main Panel")]
    public GameObject mainPanel;
    
    [Header("Mode Selection")]
    public Button componentModeButton;
    public Button accessoryModeButton;

    [Header("Component Panel")]
    public GameObject componentPanel;
    public Dropdown componentDropdown;
    public Toggle componentVisibilityToggle;
    [HideInInspector]
    public List<TableElement> componentElements = new List<TableElement>();

    [Header("Accessory Panel")]
    public GameObject accessoryPanel;
    public Dropdown accessoryDropdown;
    public Dropdown mountPointDropdown;
    [HideInInspector]
    public List<TableElement> accessoryElements = new List<TableElement>();

    [Header("Auto-Discovery")]
    [Tooltip("Automatycznie znajdź wszystkie TableElement w scenie")]
    public bool autoDiscoverElements = true;
    
    [Tooltip("Rodzic zawierający wszystkie elementy (opcjonalny)")]
    public Transform elementsContainer;

    [Header("Pivot System")]
    public Transform pivotListContainer;
    public GameObject pivotEntryPrefab;

    [Header("Movement System")]
    public Transform movementListContainer;
    public GameObject movementEntryPrefab;

    private TableElement currentSelectedElement = null;
    private bool suppressToggleCallback = false;
    private List<MountPoint> mountPoints = new List<MountPoint>();
    private bool suppressMountPointCallback = false;

    private enum PanelMode { Component, Accessory }
    private PanelMode currentMode = PanelMode.Component;

    void Start()
    {
        mainPanel.SetActive(false);

        // Automatycznie znajdź i segreguj elementy
        if (autoDiscoverElements)
        {
            DiscoverAndSortElements();
        }

        // Setup button listeners
        componentModeButton.onClick.AddListener(() => SwitchMode(PanelMode.Component));
        accessoryModeButton.onClick.AddListener(() => SwitchMode(PanelMode.Accessory));

        // Setup component dropdown
        componentDropdown.onValueChanged.AddListener(OnComponentDropdownChange);
        componentVisibilityToggle.onValueChanged.AddListener(OnVisibilityToggleChanged);

        // Setup accessory dropdown
        accessoryDropdown.onValueChanged.AddListener(OnAccessoryDropdownChange);
        mountPointDropdown.onValueChanged.AddListener(OnMountPointDropdownChange);

        // Initialize dropdowns
        UpdateComponentDropdown();
        UpdateAccessoryDropdown();

        // Start with component mode
        SwitchMode(PanelMode.Component);
    }

    // ----------------------------------------------------
    // AUTO-DISCOVERY SYSTEM
    // ----------------------------------------------------

    void DiscoverAndSortElements()
    {
        componentElements.Clear();
        accessoryElements.Clear();

        TableElement[] allElements;

        // Jeśli podano kontener, szukaj tylko w nim (włącznie z nieaktywnymi)
        if (elementsContainer != null)
        {
            allElements = elementsContainer.GetComponentsInChildren<TableElement>(true);
        }
        else
        {
            // Unity 2018 - FindObjectsOfType nie ma parametru includeInactive
            // Znajdź tylko aktywne obiekty
            allElements = FindObjectsOfType<TableElement>();
        }

        Debug.Log("Znaleziono " + allElements.Length + " elementów TableElement");

        // Sortuj elementy według typu
        foreach (var element in allElements)
        {
            if (element == null) continue;

            if (element.type == ElementType.Component)
            {
                componentElements.Add(element);
                Debug.Log("Dodano KOMPONENT: " + element.elementName);
            }
            else if (element.type == ElementType.Accessory)
            {
                accessoryElements.Add(element);
                Debug.Log("Dodano AKCESORIUM: " + element.elementName);
            }
        }

        Debug.Log("Komponenty: " + componentElements.Count + ", Akcesoria: " + accessoryElements.Count);
    }

    /// <summary>
    /// Ręczne odświeżenie listy elementów (np. po dodaniu nowych obiektów w runtime)
    /// </summary>
    [ContextMenu("Refresh Element Lists")]
    public void RefreshElementLists()
    {
        DiscoverAndSortElements();
        UpdateComponentDropdown();
        UpdateAccessoryDropdown();

        // Odśwież aktualny widok
        if (currentMode == PanelMode.Component && componentElements.Count > 0)
        {
            componentDropdown.value = 0;
            OnComponentDropdownChange(0);
        }
        else if (currentMode == PanelMode.Accessory && accessoryElements.Count > 0)
        {
            accessoryDropdown.value = 0;
            OnAccessoryDropdownChange(0);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            mainPanel.SetActive(!mainPanel.activeSelf);

            if (mainPanel.activeSelf)
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

    // ----------------------------------------------------
    // MODE SWITCHING
    // ----------------------------------------------------

    void SwitchMode(PanelMode mode)
    {
        currentMode = mode;

        if (mode == PanelMode.Component)
        {
            componentPanel.SetActive(true);
            accessoryPanel.SetActive(false);

            // Highlight active button
            componentModeButton.interactable = false;
            accessoryModeButton.interactable = true;

            // Initialize component view
            if (componentElements.Count > 0)
            {
                componentDropdown.value = Mathf.Clamp(componentDropdown.value, 0, componentElements.Count - 1);
                OnComponentDropdownChange(componentDropdown.value);
            }
            else
            {
                Debug.LogWarning("Brak komponentów do wyświetlenia!");
                ClearPivotUI();
                ClearMovementUI();
            }
        }
        else // Accessory
        {
            componentPanel.SetActive(false);
            accessoryPanel.SetActive(true);

            // Highlight active button
            componentModeButton.interactable = true;
            accessoryModeButton.interactable = false;

            // Initialize accessory view
            if (accessoryElements.Count > 0)
            {
                accessoryDropdown.value = Mathf.Clamp(accessoryDropdown.value, 0, accessoryElements.Count - 1);
                OnAccessoryDropdownChange(accessoryDropdown.value);
            }
            else
            {
                Debug.LogWarning("Brak akcesoriów do wyświetlenia!");
                ClearPivotUI();
                ClearMovementUI();
            }
        }
    }

    // ----------------------------------------------------
    // COMPONENT DROPDOWN
    // ----------------------------------------------------

    void UpdateComponentDropdown()
    {
        componentDropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var element in componentElements)
        {
            if (element != null)
                names.Add(element.elementName);
            else
                names.Add("<null>");
        }

        componentDropdown.AddOptions(names);
    }

    void OnComponentDropdownChange(int index)
    {
        if (index < 0 || index >= componentElements.Count)
        {
            currentSelectedElement = null;
            componentVisibilityToggle.interactable = false;
            ClearPivotUI();
            ClearMovementUI();
            return;
        }

        currentSelectedElement = componentElements[index];

        if (currentSelectedElement == null)
        {
            componentVisibilityToggle.interactable = false;
            ClearPivotUI();
            ClearMovementUI();
            return;
        }

        componentVisibilityToggle.interactable = true;
        Debug.Log("Wybrano komponent: " + currentSelectedElement.elementName);

        UpdateComponentToggleState();
        BuildPivotUI();
        BuildMovementUI();
    }

    void UpdateComponentToggleState()
    {
        suppressToggleCallback = true;
        componentVisibilityToggle.isOn = currentSelectedElement.gameObject.activeSelf;
        suppressToggleCallback = false;
    }

    // ----------------------------------------------------
    // ACCESSORY DROPDOWN
    // ----------------------------------------------------

    void UpdateAccessoryDropdown()
    {
        accessoryDropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var element in accessoryElements)
        {
            if (element != null)
                names.Add(element.elementName);
            else
                names.Add("<null>");
        }

        accessoryDropdown.AddOptions(names);
    }

    void OnAccessoryDropdownChange(int index)
    {
        if (index < 0 || index >= accessoryElements.Count)
        {
            currentSelectedElement = null;
            ClearPivotUI();
            ClearMovementUI();
            return;
        }

        currentSelectedElement = accessoryElements[index];

        if (currentSelectedElement == null)
        {
            ClearPivotUI();
            ClearMovementUI();
            return;
        }
        
        Debug.Log("Wybrano akcesorium: " + currentSelectedElement.elementName);

        BuildPivotUI();
        BuildMovementUI();
        BuildMountPointDropdown();
    }

    // ----------------------------------------------------
    // VISIBILITY SYSTEM
    // ----------------------------------------------------

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;
        if (currentSelectedElement == null) return;

        currentSelectedElement.SetVisible(isOn);
    }

    // ----------------------------------------------------
    // MOUNT POINT SYSTEM
    // ----------------------------------------------------

    void OnMountPointDropdownChange(int index)
    {
        if (suppressMountPointCallback)
            return;

        if (currentSelectedElement == null)
            return;

        if (currentSelectedElement.type != ElementType.Accessory)
            return;

        if (index < 0 || index >= mountPoints.Count)
            return;

        MountPoint target = mountPoints[index];
        TableElement element = currentSelectedElement;

        if (element.currentMountPoint == target)
            return;

        target.Attach(element.gameObject);
        element.SetVisible(true);

        // odśwież dropdown (stan zajętości)
        BuildMountPointDropdown();
    }

    void BuildMountPointDropdown()
    {
        mountPoints.Clear();
        mountPointDropdown.ClearOptions();

        MountPoint[] points = FindObjectsOfType<MountPoint>();
        List<string> names = new List<string>();

        int selectedIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            MountPoint mp = points[i];

            // pokaż:
            // - aktualny mount
            // - wolne mountpointy
            if (!mp.IsOccupied || mp.HasAccessory(currentSelectedElement.gameObject))
            {
                if (mp.HasAccessory(currentSelectedElement.gameObject))
                    selectedIndex = mountPoints.Count;

                mountPoints.Add(mp);
                names.Add(mp.displayName);
            }
        }

        suppressMountPointCallback = true;
        mountPointDropdown.AddOptions(names);
        mountPointDropdown.value = selectedIndex;
        suppressMountPointCallback = false;
    }

    // ----------------------------------------------------
    // PIVOT UI SYSTEM
    // ----------------------------------------------------

    void BuildPivotUI()
    {
        ClearPivotUI();

        if (currentSelectedElement == null || !currentSelectedElement.HasRotationPivots())
        {
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
            }
            else if (entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(false);
            }
        }
    }

    void BuildMovementUI()
    {
        ClearMovementUI();

        if (currentSelectedElement == null || !currentSelectedElement.HasMovementAxes())
        {
            return;
        }

        foreach (var axis in currentSelectedElement.movementAxes)
        {
            if (axis == null) continue;

            GameObject entryObj = Instantiate(movementEntryPrefab, movementListContainer);
            MovementEntryUI entry = entryObj.GetComponent<MovementEntryUI>();

            entry.axis = axis;
            entry.selector = this;
            entry.nameText.text = axis.axisName;

            // ---------- X ----------
            if (axis.allowX && entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(true);
                entry.sliderX.minValue = axis.minDistanceX;
                entry.sliderX.maxValue = axis.maxDistanceX;

                float posX = axis.currentPositionX;
                entry.sliderX.value = posX;
                entry.lastX = posX;
            }
            else if (entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(false);
            }

            // ---------- Y ----------
            if (axis.allowY && entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(true);
                entry.sliderY.minValue = axis.minDistanceY;
                entry.sliderY.maxValue = axis.maxDistanceY;

                float posY = axis.currentPositionY;
                entry.sliderY.value = posY;
                entry.lastY = posY;
            }
            else if (entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(false);
            }

            // ---------- Z ----------
            if (axis.allowZ && entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(true);
                entry.sliderZ.minValue = axis.minDistanceZ;
                entry.sliderZ.maxValue = axis.maxDistanceZ;

                float posZ = axis.currentPositionZ;
                entry.sliderZ.value = posZ;
                entry.lastZ = posZ;
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

    void ClearMovementUI()
    {
        foreach (Transform child in movementListContainer)
            Destroy(child.gameObject);
    }
}