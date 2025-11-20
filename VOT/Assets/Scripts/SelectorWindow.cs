using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Dropdown dropdown;
    public Toggle visibilityToggle; // jeden toggle zamiast dwóch

    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    private Transform currentSelectedElement = null;
    private bool suppressToggleCallback = false;

    void Start()
    {
        panel.SetActive(false);
        UpdateDropdown();

        // Listenery
        dropdown.onValueChanged.AddListener(OnDropdownChange);
        visibilityToggle.onValueChanged.AddListener(OnVisibilityToggleChanged);

        // wybierz pierwszy element (jeśli jest) i zaktualizuj toggle
        if (tableParts.Count > 0)
        {
            dropdown.value = Mathf.Clamp(dropdown.value, 0, Mathf.Max(0, tableParts.Count - 1));
            OnDropdownChange(dropdown.value);
        }
        else
        {
            // brak elementów — upewnij się, że toggle nie jest aktywny
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
        if (dropdown == null) return;

        dropdown.ClearOptions();
        List<string> names = new List<string>();

        foreach (var part in tableParts)
        {
            names.Add(part != null ? part.name : "<null>");
        }

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
            Debug.LogWarning("Wybrany element jest null!");
            visibilityToggle.interactable = false;
            return;
        }

        Debug.Log("Wybrano element: " + currentSelectedElement.name);

        // zaktualizuj toggle zgodnie ze stanem elementu
        UpdateToggleState();
        visibilityToggle.interactable = true;
    }

    void UpdateToggleState()
    {
        if (currentSelectedElement == null)
            return;

        bool isActive = currentSelectedElement.gameObject.activeSelf;

        // ustaw toggle bez wywoływania callbacka
        suppressToggleCallback = true;
        visibilityToggle.isOn = isActive;
        suppressToggleCallback = false;

        Debug.Log(currentSelectedElement.name + " jest " + (isActive ? "ATTACHED" : "DETACHED"));
    }

    void OnVisibilityToggleChanged(bool isOn)
    {
        if (suppressToggleCallback) return;

        if (currentSelectedElement == null)
        {
            Debug.LogWarning("Brak wybranego elementu do przełączenia.");
            // przywróć toggle do false, żeby nie wprowadzać w błąd
            suppressToggleCallback = true;
            visibilityToggle.isOn = false;
            suppressToggleCallback = false;
            return;
        }

        if (isOn)
            AttachElement(currentSelectedElement);
        else
            DetachElement(currentSelectedElement);
    }

    void AttachElement(Transform element)
    {
        element.gameObject.SetActive(true);
        Debug.Log("DOŁĄCZONO (pokazano): " + element.name);
    }

    void DetachElement(Transform element)
    {
        element.gameObject.SetActive(false);
        Debug.Log("ODŁĄCZONO (ukryto): " + element.name);
    }
}
