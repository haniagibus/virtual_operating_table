using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectorWindow : MonoBehaviour
{
    [Header("UI")]
    public GameObject panel;
    public Dropdown dropdown;

    [Header("Elementy stołu")]
    public List<Transform> tableParts = new List<Transform>();

    void Start()
    {
        // Ukryj panel na starcie
        panel.SetActive(false);

        // Wypełnij dropdown nazwami elementów
        UpdateDropdown();
    }

    void Update()
    {
        // Włączenie/wyłączenie okna klawiszem I
        if (Input.GetKeyDown(KeyCode.I))
        {
            panel.SetActive(!panel.activeSelf);
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

    void OnDropdownChange(int index)
    {
        Transform selected = tableParts[index];

        Debug.Log("Wybrano element stołu: " + selected.name);

        // Przykładowa akcja: zaznaczenie obiektu lub wyróżnienie go
        // Możesz tu dodać np. świecenie, podświetlenie, obrót, info itp.
    }
}
