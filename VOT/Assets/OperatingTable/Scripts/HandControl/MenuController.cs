using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

namespace VirtualOperatingTable
{
    public class MenuController : MonoBehaviour
    {
        [Header("Control Buttons")]
        public Button leftButton;
        public Button middleButton;
        public Button rightButton;

        [Header("Panels")]
        public GameObject menuPanel;
        public GameObject positionPanel;

        [Header("Menu Buttons")]
        public List<Button> menuButtons;

        [Header("Position Buttons")]
        public List<Button> positionButtons;

        [Header("Selection Colors")]
        public Color selectedColor = Color.blue;
        public Color normalColor = Color.white;
        public Color occupiedColor = Color.green;
        public Color emptyColor = Color.white;

        [Header("Event System (optional)")]
        public EventSystem eventSystem;

        [Header("Table Position Manager")]
        public TablePositionManager positionManager;

        [Header("Current State")]
        public MenuState currentState = MenuState.Closed;
        public int currentIndex = -1;

        private MenuAction selectedAction = MenuAction.None;

        void Awake()
        {
            if (eventSystem == null)
                eventSystem = EventSystem.current;

            if (positionManager == null)
            {
                Debug.LogError("[MenuController] TablePositionManager is not assigned!");
            }
        }

        void Start()
        {
            UpdatePositionButtonColors();
        }

        // ==========================
        // BUTTON ACTIONS
        // ==========================
        public void OnMiddleButtonPressed()
        {
            switch (currentState)
            {
                case MenuState.Closed:
                    EnterMenu();
                    break;

                case MenuState.Menu:
                    SelectMenuAction();
                    break;

                case MenuState.Positions:
                    ConfirmPosition();
                    break;
            }
        }

        public void OnLeftButtonPressed()
        {
            switch (currentState)
            {
                case MenuState.Closed:
                    positionManager.RestorePredefinedPosition(PredefinedPositionType.Flex);
                    break;

                case MenuState.Menu:
                    Iterate(menuButtons);
                    break;

                case MenuState.Positions:
                    Iterate(positionButtons);
                    break;
            }
        }

        public void OnRightButtonPressed()
        {
            switch (currentState)
            {
                case MenuState.Closed:
                    positionManager.RestorePredefinedPosition(PredefinedPositionType.Reflex);
                    break;

                case MenuState.Menu:
                    ExitAll();
                    break;

                case MenuState.Positions:
                    ExitToMenu();
                    break;
            }
        }

        // ==========================
        // STATE CONTROL
        // ==========================
        public void EnterMenu()
        {
            currentState = MenuState.Menu;
            menuPanel.SetActive(true);
            positionPanel.SetActive(false);

            currentIndex = -1;
            selectedAction = MenuAction.None;
            ClearSelection(menuButtons);
            UpdateControlButtonColors();
        }

        public void EnterPositions()
        {
            currentState = MenuState.Positions;
            menuPanel.SetActive(false);
            positionPanel.SetActive(true);

            currentIndex = -1;
            ClearSelection(positionButtons);
            UpdatePositionButtonColors();
            UpdateControlButtonColors();
        }

        public void ExitToMenu()
        {
            selectedAction = MenuAction.None;
            EnterMenu();
        }

        public void ExitAll()
        {
            currentState = MenuState.Closed;
            menuPanel.SetActive(false);
            positionPanel.SetActive(false);

            currentIndex = -1;
            selectedAction = MenuAction.None;
            ClearSelection(menuButtons);
            ClearSelection(positionButtons);
            UpdateControlButtonColors();
        }

        private void UpdateControlButtonColors()
        {
            Color color = (currentState == MenuState.Closed) ? Color.white : Color.red;

            SetButtonColorOnly(middleButton, color);
            SetButtonColorOnly(leftButton, color);
            SetButtonColorOnly(rightButton, color);
        }

        private void SetButtonColorOnly(Button button, Color color)
        {
            if (button == null) return;

            var colors = button.colors;
            colors.normalColor = color;
            colors.highlightedColor = color;
            colors.pressedColor = color;
            button.colors = colors;
        }

        // ==========================
        // ITERATION & SELECTION
        // ==========================
        private void Iterate(List<Button> buttonList)
        {
            if (buttonList == null || buttonList.Count == 0)
                return;

            if (currentIndex >= 0)
                SetButtonNormal(buttonList[currentIndex]);

            currentIndex = (currentIndex + 1) % buttonList.Count;

            SetSelected(buttonList[currentIndex]);
        }

        private void SetSelected(Button button)
        {
            var colors = button.colors;
            colors.normalColor = selectedColor;
            colors.highlightedColor = selectedColor;
            colors.pressedColor = selectedColor;
            button.colors = colors;

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
                text.color = Color.white;

            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(button.gameObject);
        }

        private void SetButtonNormal(Button button)
        {
            Color baseColor = normalColor;

            if (currentState == MenuState.Positions && positionButtons.Contains(button))
            {
                int index = positionButtons.IndexOf(button);
                if (positionManager != null && positionManager.IsSlotOccupied(index))
                {
                    baseColor = occupiedColor;
                }
                else
                {
                    baseColor = emptyColor;
                }
            }

            var colors = button.colors;
            colors.normalColor = baseColor;
            colors.highlightedColor = baseColor;
            colors.pressedColor = baseColor;
            button.colors = colors;

            var text = button.GetComponentInChildren<Text>();
            if (text != null)
                text.color = Color.black;
        }

        private void ClearSelection(List<Button> buttonList)
        {
            foreach (var btn in buttonList)
            {
                SetButtonNormal(btn);
            }

            currentIndex = -1;
            if (eventSystem != null)
                eventSystem.SetSelectedGameObject(null);
        }

        // ==========================
        // MENU ACTION SELECTION
        // ==========================
        private void SelectMenuAction()
        {
            if (currentIndex < 0) return;

            selectedAction = (MenuAction)currentIndex;

            Debug.Log("[MenuController] Selected action: " + selectedAction);

            EnterPositions();
        }

        // ==========================
        // POSITION CONFIRMATION
        // ==========================
        private void ConfirmPosition()
        {
            if (currentIndex < 0)
            {
                Debug.LogWarning("[MenuController] No position selected");
                return;
            }

            if (positionManager == null)
            {
                Debug.LogError("[MenuController] TablePositionManager is not assigned!");
                return;
            }

            int positionSlot = currentIndex;

            switch (selectedAction)
            {
                case MenuAction.SavePosition:
                    SavePosition(positionSlot);
                    break;

                case MenuAction.LoadPosition:
                    LoadPosition(positionSlot);
                    break;

                default:
                    Debug.LogWarning("[MenuController] No menu action selected");
                    break;
            }
        }

        private void SavePosition(int slotIndex)
        {
            if (positionManager.lockFirstPosition && slotIndex == 0)
            {
                Debug.LogWarning("[MenuController] Cannot save position 1 - it is predefined");
                return;
            }

            positionManager.SavePosition(slotIndex);

            UpdatePositionButtonColors();

            Debug.Log("[MenuController] Position saved to slot " + (slotIndex + 1));
        }

        private void LoadPosition(int slotIndex)
        {
            if (!positionManager.IsSlotOccupied(slotIndex))
            {
                Debug.LogWarning("[MenuController] Slot " + (slotIndex + 1) + " is empty - cannot load");
                return;
            }

            positionManager.LoadPosition(slotIndex);

            Debug.Log("[MenuController] Loading position from slot " + (slotIndex + 1) + " started");
        }

        // ==========================
        // UI UPDATES
        // ==========================
        private void UpdatePositionButtonColors()
        {
            if (positionManager == null || positionButtons == null)
                return;

            for (int i = 0; i < positionButtons.Count; i++)
            {
                if (i != currentIndex)
                {
                    SetButtonNormal(positionButtons[i]);
                }
            }
        }
    }
}
