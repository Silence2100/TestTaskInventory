using System;
using UnityEngine;
using Inventory.Model;

namespace Inventory.View
{
    public class InventoryView : MonoBehaviour
    {
        private const int NoSelectionIndex = -1;

        [SerializeField] private TopBarView _topBarView;
        [SerializeField] private InventoryGridView _gridView;
        [SerializeField] private InventoryDetailsView _detailsView;
        [SerializeField] private StatusView _statusView;
        [SerializeField] private ContextMenuView _contextMenu;

        public event Action DeleteClicked;

        public void Initialize(int capacity)
        {
            _topBarView?.RefreshAll();
            _gridView?.Build(capacity);

            if (_detailsView != null)
            {
                _detailsView.DeleteClicked -= OnDeleteClicked;
                _detailsView.DeleteClicked += OnDeleteClicked;
            }

            ClearStatus();
            HideContextMenu();

            SetSelectedIndex(NoSelectionIndex);
            SetDetailsItem(null);
        }

        private void OnDestroy()
        {
            _detailsView.DeleteClicked -= OnDeleteClicked;
        }

        public void SetSlotIcon(int index, Sprite icon)
        {
            _gridView?.SetSlotIcon(index, icon);
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            _gridView?.SetSelectedIndex(selectedIndex);
        }

        public void SetDetailsItem(ItemDefinition item)
        {
            _detailsView?.SetItem(item);
        }

        public void ShowStatus(string message)
        {
            _statusView?.Show(message);
        }

        public void ClearStatus()
        {
            _statusView?.Clear();
        }

        public void ShowContextMenu(Vector2 screenPosition, string label, Action onClick)
        {
            _contextMenu?.Show(screenPosition, label, onClick);
        }

        public void HideContextMenu()
        {
            _contextMenu?.Hide();
        }

        private void OnDeleteClicked()
        {
            DeleteClicked?.Invoke();
        }
    }
}