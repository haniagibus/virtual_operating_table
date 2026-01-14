using UnityEngine;
using UnityEngine.UI;

public class TabController : MonoBehaviour
{
    [Header("Panels")]
    public GameObject componentsPanel;      
    public GameObject accessoriesPanel;  

    [Header("Top buttons")]
    public Button componentsButton;        
    public Button accessoriesButton;   

    [Header("Optional: default tab")]
    public bool startWithComponents = true;

    void Start()
    {
        if (componentsButton != null) componentsButton.onClick.AddListener(ShowComponentsPanel);
        if (accessoriesButton != null) accessoriesButton.onClick.AddListener(ShowAccessoriesPanel);

        if (startWithComponents)
            ShowComponentsPanel();
        else
            ShowAccessoriesPanel();
    }

    public void ShowComponentsPanel()
    {
        SetActivePanel(componentsPanel, accessoriesPanel);
        SetButtonStates(componentsButton, accessoriesButton);
    }

    public void ShowAccessoriesPanel()
    {
        SetActivePanel(accessoriesPanel, componentsPanel);
        SetButtonStates(accessoriesButton, componentsButton);
    }

    void SetActivePanel(GameObject toShow, GameObject toHide)
    {
        if (toShow != null) toShow.SetActive(true);
        if (toHide != null) toHide.SetActive(false);
    }

    void SetButtonStates(Button active, Button inactive)
    {
        if (active != null) active.interactable = false; 
        if (inactive != null) inactive.interactable = true;
    }
}
