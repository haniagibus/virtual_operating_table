using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using VirtualOperatingTable;

public class TableElementControlPanel : MonoBehaviour
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
    public bool autoDiscoverElements = true;

    public Transform elementsContainer;

    [Header("Component Pivot & Movement Containers")]
    public Transform componentPivotListContainer;
    public Transform componentMovementListContainer;

    [Header("Accessory Pivot & Movement Containers")]
    public Transform accessoryPivotListContainer;
    public Transform accessoryMovementListContainer;

    [Header("Prefabs (Shared)")]
    public GameObject pivotEntryPrefab;
    public GameObject movementEntryPrefab;

    public TableElement currentSelectedElement = null;

    private bool suppressToggleCallback = false;
    private List<MountPoint> mountPoints = new List<MountPoint>();
    private bool suppressMountPointCallback = false;

    private enum PanelMode { Component, Accessory }
    private PanelMode currentMode = PanelMode.Component;

    void Start()
    {
        mainPanel.SetActive(false);

        if (autoDiscoverElements)
        {
            DiscoverAndSortElements();
        }

        componentModeButton.onClick.AddListener(() => SwitchMode(PanelMode.Component));
        accessoryModeButton.onClick.AddListener(() => SwitchMode(PanelMode.Accessory));

        componentDropdown.onValueChanged.AddListener(OnComponentDropdownChange);
        componentVisibilityToggle.onValueChanged.AddListener(OnVisibilityToggleChanged);

        accessoryDropdown.onValueChanged.AddListener(OnAccessoryDropdownChange);
        mountPointDropdown.onValueChanged.AddListener(OnMountPointDropdownChange);

        UpdateComponentDropdown();
        UpdateAccessoryDropdown();

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

        if (elementsContainer != null)
        {
            allElements = elementsContainer.GetComponentsInChildren<TableElement>(true);
        }
        else
        {
            allElements = FindObjectsOfType<TableElement>();
            
            List<TableElement> elementsList = new List<TableElement>();
            GameObject[] rootObjects = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();
            foreach (GameObject root in rootObjects)
            {
                TableElement[] elements = root.GetComponentsInChildren<TableElement>(true);
                elementsList.AddRange(elements);
            }
            allElements = elementsList.ToArray();
        }

        foreach (var element in allElements)
        {
            if (element == null) continue;

            if (element.type == ElementType.Component)
            {
                componentElements.Add(element);
                Debug.Log("Added COMPONENT: " + element.elementName);
            }
            else if (element.type == ElementType.Accessory)
            {
                accessoryElements.Add(element);
                Debug.Log("Added ACCESSORY: " + element.elementName);
            }
        }

        Debug.Log("Components: " + componentElements.Count + ", Accessories: " + accessoryElements.Count);
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

            componentModeButton.interactable = false;
            accessoryModeButton.interactable = true;

            if (componentElements.Count > 0)
            {
                componentDropdown.value = Mathf.Clamp(componentDropdown.value, 0, componentElements.Count - 1);
                OnComponentDropdownChange(componentDropdown.value);
            }
            else
            {
                Debug.LogWarning("No components to display!");
                ClearPivotUI();
                ClearMovementUI();
            }
        }
        else
        {
            componentPanel.SetActive(false);
            accessoryPanel.SetActive(true);

            componentModeButton.interactable = true;
            accessoryModeButton.interactable = false;

            if (accessoryElements.Count > 0)
            {
                accessoryDropdown.value = Mathf.Clamp(accessoryDropdown.value, 0, accessoryElements.Count - 1);
                OnAccessoryDropdownChange(accessoryDropdown.value);
            }
            else
            {
                Debug.LogWarning("No accessories to display!");
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
        Debug.Log("Selected component: " + currentSelectedElement.elementName);

        UpdateComponentToggleState();
        
        if (currentSelectedElement.isAttached)
        {
            BuildPivotUI();
            BuildMovementUI();
        }
        else
        {
            ClearPivotUI();
            ClearMovementUI();
        }
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
        
        Debug.Log("Selected accessory: " + currentSelectedElement.elementName);

        if (currentSelectedElement.isAttached)
        {
            BuildPivotUI();
            BuildMovementUI();
        }
        else
        {
            ClearPivotUI();
            ClearMovementUI();
        }
        
        BuildMountPointDropdown();
    }

    // ----------------------------------------------------
    // VISIBILITY SYSTEM
    // ----------------------------------------------------

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;
        if (currentSelectedElement == null) return;

        currentSelectedElement.SetAttached(isOn);
        
        if (isOn)
        {
            BuildPivotUI();
            BuildMovementUI();
        }
        else
        {
            ClearPivotUI();
            ClearMovementUI();
        }
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

        if (index == 0)
        {
            if (currentSelectedElement.currentMountPoint != null)
            {
                currentSelectedElement.currentMountPoint.Detach();
                Debug.Log("Detached accessory: " + currentSelectedElement.elementName);
                
                ClearPivotUI();
                ClearMovementUI();
            }
            return;
        }

        int mountPointIndex = index - 1;
        
        if (mountPointIndex < 0 || mountPointIndex >= mountPoints.Count)
            return;

        MountPoint target = mountPoints[mountPointIndex];
        TableElement element = currentSelectedElement;

        if (element.currentMountPoint == target)
            return;

        target.Attach(element.gameObject);
        element.SetAttached(true);

        BuildPivotUI();
        BuildMovementUI();

        BuildMountPointDropdown();
    }

    void BuildMountPointDropdown()
    {
        mountPoints.Clear();
        mountPointDropdown.ClearOptions();

        MountPoint[] points = FindObjectsOfType<MountPoint>();
        List<string> names = new List<string>();

        names.Add("Not Attached");
        int selectedIndex = 0;

        for (int i = 0; i < points.Length; i++)
        {
            MountPoint mp = points[i];

            if (!mp.IsOccupied || mp.HasAccessory(currentSelectedElement.gameObject))
            {
                if (mp.HasAccessory(currentSelectedElement.gameObject))
                {
                    selectedIndex = mountPoints.Count + 1;
                }

                mountPoints.Add(mp);
                names.Add(mp.displayName);
            }
        }

        if (currentSelectedElement.currentMountPoint == null)
        {
            selectedIndex = 0;
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

        Transform targetContainer = (currentMode == PanelMode.Component) 
            ? componentPivotListContainer 
            : accessoryPivotListContainer;

        foreach (var pivot in currentSelectedElement.rotationPivots)
        {
            if (pivot == null) continue;

            GameObject entryObj = Instantiate(pivotEntryPrefab, targetContainer);
            PivotEntryUI entry = entryObj.GetComponent<PivotEntryUI>();

            entry.pivot = pivot;
            entry.tableElementControl = this;
            entry.nameText.text = pivot.pivotName;

            if (pivot.allowX && entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(true);
                entry.sliderX.minValue = pivot.minAngleX;
                entry.sliderX.maxValue = pivot.maxAngleX;

                float angleX = pivot.currentAngleX;
                entry.lastX = angleX;
                entry.sliderX.value = angleX;
            }
            else if (entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(false);
            }

            if (pivot.allowY && entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(true);
                entry.sliderY.minValue = pivot.minAngleY;
                entry.sliderY.maxValue = pivot.maxAngleY;

                float angleY = pivot.currentAngleY;
                entry.lastY = angleY;
                entry.sliderY.value = angleY;
            }
            else if (entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(false);
            }

            if (pivot.allowZ && entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(true);
                entry.sliderZ.minValue = pivot.minAngleZ;
                entry.sliderZ.maxValue = pivot.maxAngleZ;

                float angleZ = pivot.currentAngleZ;
                entry.lastZ = angleZ;
                entry.sliderZ.value = angleZ;
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

        Transform targetContainer = (currentMode == PanelMode.Component) 
            ? componentMovementListContainer 
            : accessoryMovementListContainer;

        foreach (var axis in currentSelectedElement.movementAxes)
        {
            if (axis == null) continue;

            GameObject entryObj = Instantiate(movementEntryPrefab, targetContainer);
            MovementEntryUI entry = entryObj.GetComponent<MovementEntryUI>();

            entry.axis = axis;
            entry.tableElementControl = this;
            entry.nameText.text = axis.axisName;

            if (axis.allowX && entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(true);
                entry.sliderX.minValue = axis.minDistanceX;
                entry.sliderX.maxValue = axis.maxDistanceX;

                float posX = axis.currentPositionX;
                entry.lastX = posX;
                entry.sliderX.value = posX;
            }
            else if (entry.sliderX != null)
            {
                entry.sliderX.gameObject.SetActive(false);
            }

            if (axis.allowY && entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(true);
                entry.sliderY.minValue = axis.minDistanceY;
                entry.sliderY.maxValue = axis.maxDistanceY;

                float posY = axis.currentPositionY;
                entry.lastY = posY;
                entry.sliderY.value = posY;
            }
            else if (entry.sliderY != null)
            {
                entry.sliderY.gameObject.SetActive(false);
            }

            if (axis.allowZ && entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(true);
                entry.sliderZ.minValue = axis.minDistanceZ;
                entry.sliderZ.maxValue = axis.maxDistanceZ;

                float posZ = axis.currentPositionZ;
                entry.lastZ = posZ;
                entry.sliderZ.value = posZ;
            }
            else if (entry.sliderZ != null)
            {
                entry.sliderZ.gameObject.SetActive(false);
            }
        }
    }

    void ClearPivotUI()
    {
        foreach (Transform child in componentPivotListContainer)
            Destroy(child.gameObject);
        
        foreach (Transform child in accessoryPivotListContainer)
            Destroy(child.gameObject);
    }

    void ClearMovementUI()
    {
        foreach (Transform child in componentMovementListContainer)
            Destroy(child.gameObject);
        
        foreach (Transform child in accessoryMovementListContainer)
            Destroy(child.gameObject);
    }
}