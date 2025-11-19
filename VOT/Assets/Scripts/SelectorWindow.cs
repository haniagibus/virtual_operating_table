using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Dropdown dropdown;
    public Toggle attachToggle;
    public Toggle detachToggle;

    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    private Transform currentSelectedElement = null;

    void Start()
    {
        panel.SetActive(false);
        UpdateDropdown();
        
        attachToggle.onValueChanged.AddListener(OnAttachToggleChanged);
        detachToggle.onValueChanged.AddListener(OnDetachToggleChanged);
        dropdown.onValueChanged.AddListener(OnDropdownChange);

        attachToggle.isOn = true;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            panel.SetActive(!panel.activeSelf);
            
            // Odblokuj/zablokuj kursor w zależności od stanu panelu
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
        {
            if (part != null)
                names.Add(part.name);
        }
        
        dropdown.AddOptions(names);
    }

    void OnDropdownChange(int index)
    {
        if (index < 0 || index >= tableParts.Count)
            return;
            
        currentSelectedElement = tableParts[index];
        
        if (currentSelectedElement == null)
        {
            Debug.LogWarning("Wybrany element jest null!");
            return;
        }
        
        Debug.Log("Wybrano element: " + currentSelectedElement.name);
        
        // Zaktualizuj stan togglei na podstawie aktualnego stanu elementu
        UpdateTogglesState();
    }

    void UpdateTogglesState()
    {
        if (currentSelectedElement == null)
            return;

        // Tymczasowo usuń listenery, żeby uniknąć zapętlenia
        attachToggle.onValueChanged.RemoveAllListeners();
        detachToggle.onValueChanged.RemoveAllListeners();

        // Ustaw toggles w zależności od stanu elementu
        bool isActive = currentSelectedElement.gameObject.activeSelf;
        attachToggle.isOn = isActive;
        detachToggle.isOn = !isActive;

        // Przywróć listenery
        attachToggle.onValueChanged.AddListener(OnAttachToggleChanged);
        detachToggle.onValueChanged.AddListener(OnDetachToggleChanged);

        Debug.Log(currentSelectedElement.name + " jest " + (isActive ? "ATTACHED" : "DETACHED"));
    }

    void OnAttachToggleChanged(bool isOn)
    {
        if (isOn && currentSelectedElement != null)
        {
            AttachElement(currentSelectedElement);
            detachToggle.isOn = false;
        }
    }

    void OnDetachToggleChanged(bool isOn)
    {
        if (isOn && currentSelectedElement != null)
        {
            DetachElement(currentSelectedElement);
            attachToggle.isOn = false;
        }
    }

    void AttachElement(Transform element)
    {
        // Pokazuje element na scenie
        element.gameObject.SetActive(true);
        Debug.Log("DOŁĄCZONO (pokazano): " + element.name);
    }

    void DetachElement(Transform element)
    {
        // Ukrywa element ze sceny
        element.gameObject.SetActive(false);
        Debug.Log("ODŁĄCZONO (ukryto): " + element.name);
    }
}