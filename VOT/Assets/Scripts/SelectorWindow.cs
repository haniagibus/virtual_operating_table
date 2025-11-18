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
    //klasa element i tam przecowywac dane takie jak is attatched/detatch,
    // możliwosci czyli czy mozna obracac czy do goty do dolu 
    
    private bool isAttachMode = false;
    void Start()
    {
        panel.SetActive(false);
        UpdateDropdown();

        attachToggle.onValueChanged.AddListener(OnAttachToggleChanged);
        detachToggle.onValueChanged.AddListener(OnDetachToggleChanged);
    }

    void Update()
{
    if (Input.GetKeyDown(KeyCode.I))
    {
        panel.SetActive(!panel.activeSelf);
        
        // unblock coursor if panel is open
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

        dropdown.onValueChanged.AddListener(OnDropdownChange);
    }

    void OnAttachToggleChanged(bool isOn)
    {
        if (isOn)
        {
            isAttachMode = true;
            Debug.Log("element attached ");
        }
    }

    void OnDetachToggleChanged(bool isOn)
    {
        if (isOn)
        {
            isAttachMode = false;
            Debug.Log("element detatched");
        }
    }

    void OnDropdownChange(int index)
    {
        Transform selected = tableParts[index];
        Debug.Log("Wybrano element stołu: " + selected.name);
        
        if (isAttachMode)
        {
            AttachElement(selected);
        }
        else
        {
            DetachElement(selected);
        }
    }

    void AttachElement(Transform element)
    {
        // Tu będzie kod dołączania
        Debug.Log("DOŁĄCZAM: " + element.name);
    }

    void DetachElement(Transform element)
    {
        // Tu będzie kod odłączania
        Debug.Log("ODŁĄCZAM: " + element.name);
    }
}
