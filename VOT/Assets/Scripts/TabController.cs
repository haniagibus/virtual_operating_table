using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject tablePanel;        // przypisz TablePanel (z SelectorWindow inside)
    public GameObject accessoriesPanel;  // przypisz AccessoriesPanel

    [Header("Top buttons")]
    public Button tableButton;           // przycisk "Table"
    public Button accessoriesButton;     // przycisk "Accessories"

    [Header("Optional: default tab")]
    public bool startWithTable = true;

    void Start()
    {
        // przypnij listenery przycisków (jeśli nie chcesz ustawiać w Inspectorze)
        if (tableButton != null) tableButton.onClick.AddListener(ShowTablePanel);
        if (accessoriesButton != null) accessoriesButton.onClick.AddListener(ShowAccessoriesPanel);

        // pokaż domyślnie jedną z zakładek
        if (startWithTable)
            ShowTablePanel();
        else
            ShowAccessoriesPanel();
    }

    public void ShowTablePanel()
    {
        SetActivePanel(tablePanel, accessoriesPanel);
        SetButtonStates(tableButton, accessoriesButton);
    }

    public void ShowAccessoriesPanel()
    {
        SetActivePanel(accessoriesPanel, tablePanel);
        SetButtonStates(accessoriesButton, tableButton);
    }

    void SetActivePanel(GameObject toShow, GameObject toHide)
    {
        if (toShow != null) toShow.SetActive(true);
        if (toHide != null) toHide.SetActive(false);
    }

    void SetButtonStates(Button active, Button inactive)
    {
        if (active != null) active.interactable = false;   // aktywny przycisk nieklikalny
        if (inactive != null) inactive.interactable = true;

        // opcjonalnie: prosty wizualny feedback (zmiana koloru)
        // domyślny ColorBlock można ustawić ręcznie w Inspectorze; poniżej jest bezpieczne ustawienie alpha
        // (jeśli chcesz mocniejszy efekt, ustaw w Inspectorze ColorBlock dla każdego przycisku)
    }

    // Opcjonalnie: metoda do przełączenia (np. z klawisza) - nie używaj jednocześnie z SelectorWindow.Toggle
    public void TogglePanels()
    {
        if (tablePanel != null && tablePanel.activeSelf)
            ShowAccessoriesPanel();
        else
            ShowTablePanel();
    }
}
