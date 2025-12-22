using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Controller;
using Inventory.Model;

namespace Inventory.View
{
    public class InventoryView : MonoBehaviour
    {
        [Header("Grid")]
        [SerializeField] private Transform _gridRoot;
        [SerializeField] private InventorySlotView _slotPrefab;

        [Header("Top items")]
        [SerializeField] private ItemSourceView[] _topItems;

        [Header("Details")]
        [SerializeField] private Image _detailsIcon;
        [SerializeField] private TMP_Text _detailsName;
        [SerializeField] private TMP_Text _detailsDescription;
        [SerializeField] private Button _deleteButton;
        [SerializeField] private TMP_Text _statusText;

        [Header("Context Menu")]
        [SerializeField] private ContextMenuView _contextMenu;

        private InventoryController _controller;
        private readonly List<InventorySlotView> _slots = new();

        public void Initialize(InventoryController controller)
        {
            _controller = controller;

            foreach (var top in _topItems)
                top.Initialize(this);

            _deleteButton.onClick.RemoveAllListeners();
            _deleteButton.onClick.AddListener(OnDeleteClicked);

            _controller.InventoryChanged += RefreshAll;
            _controller.SelectionChanged += OnSelectionChanged;

            BuildGrid();
            RefreshAll();
            OnSelectionChanged(_controller.SelectedIndex);
        }

        private void OnDestroy()
        {
            if (_controller == null)
                return;

            _controller.InventoryChanged -= RefreshAll;
            _controller.SelectionChanged -= OnSelectionChanged;
        }

        private void BuildGrid()
        {
            for (int i = _gridRoot.childCount - 1; i >= 0; i--)
                Destroy(_gridRoot.GetChild(i).gameObject);

            _slots.Clear();

            for (int i = 0; i < _controller.Capacity; i++)
            {
                var slot = Instantiate(_slotPrefab, _gridRoot);
                slot.Initialize(this, i);
                _slots.Add(slot);
            }
        }

        private void RefreshAll()
        {
            ClearStatus();

            for (int i = 0; i < _slots.Count; i++)
            {
                var item = _controller.GetItemAt(i);
                _slots[i].SetItemIcon(item != null ? item.Icon : null);
            }

            OnSelectionChanged(_controller.SelectedIndex);
        }

        private void OnSelectionChanged(int selectedIndex)
        {
            for (int i = 0; i < _slots.Count; i++)
                _slots[i].SetSelected(i == selectedIndex);

            var item = selectedIndex >= 0 ? _controller.GetItemAt(selectedIndex) : null;

            if (item == null)
            {
                _detailsIcon.enabled = false;
                _detailsIcon.sprite = null;
                _detailsName.text = string.Empty;
                _detailsDescription.text = string.Empty;
                _deleteButton.interactable = false;

                return;
            }

            _detailsIcon.enabled = item.Icon != null;
            _detailsIcon.sprite = item.Icon;
            _detailsName.text = item.DisplayName;
            _detailsDescription.text = item.Description;
            _deleteButton.interactable = true;
        }

        private void OnDeleteClicked()
        {
            var ok = _controller.TryRemoveSelected();
            if (ok == false)
                ShowStatus("Nothing to delete.");
        }

        private void ShowStatus(string message)
        {
            if (_statusText != null)
                _statusText.text = message;
        }

        private void ClearStatus()
        {
            if (_statusText != null)
                _statusText.text = string.Empty;
        }

        public void OnSlotLeftClicked(int index)
        {
            _contextMenu.Hide();
            _controller.Select(index);
        }

        public void OnSlotRightClicked(int index, Vector2 screenPosition)
        {
            _controller.Select(index);

            var item = _controller.GetItemAt(index);
            if (item == null)
            {
                _contextMenu.Hide();
                return;
            }

            _contextMenu.Show(screenPosition, "Remove", () =>
            {
                var ok = _controller.TryRemoveAt(index);
                if (ok == false)
                    ShowStatus("Remove failed.");
            });
        }

        public void OnTopItemRightClicked(ItemDefinition item, Vector2 screenPosition)
        {
            _contextMenu.Show(screenPosition, "Add", () =>
            {
                var ok = _controller.TryAdd(item);
                if (ok == false)
                    ShowStatus("Inventory is full.");
            });
        }
    }
}