using UnityEngine;
using Inventory.Input;
using Inventory.View;

namespace Inventory.Controller
{
    public class InventoryUIController : MonoBehaviour
    {
        private const string MessageNothingToRemove = "Нечего удалять.";
        private const string MessageInventoryFull = "В инвентаре больше не места.";
        private const string MessageRemoveFailed = "Невозможно удалить.";

        private InventoryController _inventoryController;
        private InventoryView _view;
        private InputHandler _input;

        private bool _isInitialized;

        public void Initialize(InventoryController inventoryController, InventoryView view, InputHandler input)
        {
            Unsubscribe();

            _inventoryController = inventoryController;
            _view = view;
            _input = input;

            _view.Initialize(_inventoryController.Capacity);

            Subscribe();

            RefreshAll();
            OnSelectionChanged(_inventoryController != null ? _inventoryController.SelectedIndex : -1);

            _isInitialized = true;
        }

        private void OnEnable()
        {
            if (_isInitialized)
                Subscribe();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void Subscribe()
        {
            _inventoryController.InventoryChanged -= RefreshAll;
            _inventoryController.InventoryChanged += RefreshAll;

            _inventoryController.SelectionChanged -= OnSelectionChanged;
            _inventoryController.SelectionChanged += OnSelectionChanged;

            _view.DeleteClicked -= OnDeleteClicked;
            _view.DeleteClicked += OnDeleteClicked;

            _input.SlotLeftClicked -= OnSlotLeftClicked;
            _input.SlotLeftClicked += OnSlotLeftClicked;

            _input.SlotRightClicked -= OnSlotRightClicked;
            _input.SlotRightClicked += OnSlotRightClicked;

            _input.TopItemRightClicked -= OnTopItemRightClicked;
            _input.TopItemRightClicked += OnTopItemRightClicked;

            _input.ClickedOutside -= OnClickedOutside;
            _input.ClickedOutside += OnClickedOutside;

            _input.CancelPressed -= OnCancelPressed;
            _input.CancelPressed += OnCancelPressed;

            _input.DeletePressed -= OnDeleteKeyPressed;
            _input.DeletePressed += OnDeleteKeyPressed;

            _input.TopItemDoubleClicked -= OnTopItemDoubleClicked;
            _input.TopItemDoubleClicked += OnTopItemDoubleClicked;
        }

        private void Unsubscribe()
        {
            if (_inventoryController != null)
            {
                _inventoryController.InventoryChanged -= RefreshAll;
                _inventoryController.SelectionChanged -= OnSelectionChanged;
            }

            if (_view != null)
            {
                _view.DeleteClicked -= OnDeleteClicked;
            }

            if (_input != null)
            {
                _input.SlotLeftClicked -= OnSlotLeftClicked;
                _input.SlotRightClicked -= OnSlotRightClicked;
                _input.TopItemRightClicked -= OnTopItemRightClicked;
                _input.ClickedOutside -= OnClickedOutside;
                _input.CancelPressed -= OnCancelPressed;
                _input.DeletePressed -= OnDeleteKeyPressed;
                _input.TopItemDoubleClicked -= OnTopItemDoubleClicked;
            }
        }

        private void RefreshAll()
        {
            if (_view == null || _inventoryController == null)
                return;

            _view.ClearStatus();

            for (int i = 0; i < _inventoryController.Capacity; i++)
            {
                var item = _inventoryController.GetItemAt(i);
                _view.SetSlotIcon(i, item != null ? item.Icon : null);
            }

            OnSelectionChanged(_inventoryController.SelectedIndex);
        }

        private void OnSelectionChanged(int selectedIndex)
        {
            if (_view == null || _inventoryController == null)
                return;

            _view.SetSelectedIndex(selectedIndex);

            var item = selectedIndex >= 0 ? _inventoryController.GetItemAt(selectedIndex) : null;
            _view.SetDetailsItem(item);
        }

        private void OnDeleteClicked()
        {
            TryRemoveSelected();
        }

        private void OnDeleteKeyPressed()
        {
            TryRemoveSelected();
        }

        private void TryRemoveSelected()
        {
            if (_view == null || _inventoryController == null)
                return;

            _view.HideContextMenu();

            var wasRemoved = _inventoryController.TryRemoveSelected();

            if (wasRemoved == false)
                _view.ShowStatus(MessageNothingToRemove);
        }

        private void OnSlotLeftClicked(int index)
        {
            if (_view == null || _inventoryController == null)
                return;

            _view.HideContextMenu();
            _inventoryController.Select(index);
        }

        private void OnSlotRightClicked(int index, Vector2 screenPosition)
        {
            if (_view == null || _inventoryController == null)
                return;

            _inventoryController.Select(index);

            var item = _inventoryController.GetItemAt(index);

            if (item == null)
            {
                _view.HideContextMenu();
                return;
            }

            _view.ShowContextMenu(screenPosition, "Удалить", () =>
            {
                var wasRemoved = _inventoryController.TryRemoveAt(index);
                if (wasRemoved == false)
                    _view.ShowStatus(MessageRemoveFailed);
            });
        }

        private void OnTopItemRightClicked(Inventory.Model.ItemDefinition item, Vector2 screenPosition)
        {
            if (_view == null || _inventoryController == null || item == null)
                return;

            _view.ShowContextMenu(screenPosition, "Добавить", () =>
            {
                var wasAdded = _inventoryController.TryAdd(item);
                if (wasAdded == false)
                    _view.ShowStatus(MessageInventoryFull);
            });
        }

        private void OnTopItemDoubleClicked(Inventory.Model.ItemDefinition item)
        {
            if (_view == null || _inventoryController == null || item == null)
                return;

            _view.HideContextMenu();

            var wasAdded = _inventoryController.TryAdd(item);
            if (wasAdded == false)
                _view.ShowStatus(MessageInventoryFull);
        }

        private void OnClickedOutside()
        {
            _view?.HideContextMenu();
        }

        private void OnCancelPressed()
        {
            _view?.HideContextMenu();
        }
    }
}