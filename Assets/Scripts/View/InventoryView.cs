using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Controller;
using Inventory.Model;

namespace Inventory.View
{
    public sealed class InventoryView : MonoBehaviour
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

            if (_topItems != null)
            {
                foreach (var top in _topItems)
                {
                    if (top != null)
                        top.Refresh();
                }
            }

            if (_deleteButton != null)
            {
                _deleteButton.onClick.RemoveAllListeners();
                _deleteButton.onClick.AddListener(OnDeleteClicked);
            }

            if (_controller != null)
            {
                _controller.InventoryChanged += RefreshAll;
                _controller.SelectionChanged += OnSelectionChanged;
            }

            BuildGrid();
            RefreshAll();
            OnSelectionChanged(_controller != null ? _controller.SelectedIndex : -1);
        }

        private void OnDestroy()
        {
            if (_controller == null)
                return;

            _controller.InventoryChanged -= RefreshAll;
            _controller.SelectionChanged -= OnSelectionChanged;
        }

        public void HideContextMenu()
        {
            if (_contextMenu != null)
                _contextMenu.Hide();
        }

        private void BuildGrid()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null)
                    Destroy(_slots[i].gameObject);
            }
            _slots.Clear();

            if (_controller == null || _gridRoot == null || _slotPrefab == null)
            {
                Debug.LogError("InventoryView.BuildGrid: missing references (controller/gridRoot/slotPrefab).");
                return;
            }

            for (int i = 0; i < _controller.Capacity; i++)
            {
                var slot = Instantiate(_slotPrefab, _gridRoot);
                slot.Initialize(i);
                _slots.Add(slot);
            }
        }

        private void RefreshAll()
        {
            ClearStatus();

            if (_controller == null)
                return;

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

            var item = (selectedIndex >= 0 && _controller != null) ? _controller.GetItemAt(selectedIndex) : null;

            if (item == null)
            {
                if (_detailsIcon != null)
                {
                    _detailsIcon.enabled = false;
                    _detailsIcon.sprite = null;
                }

                if (_detailsName != null)
                    _detailsName.text = string.Empty;

                if (_detailsDescription != null)
                    _detailsDescription.text = string.Empty;

                if (_deleteButton != null)
                    _deleteButton.interactable = false;

                return;
            }

            if (_detailsIcon != null)
            {
                _detailsIcon.enabled = item.Icon != null;
                _detailsIcon.sprite = item.Icon;
            }

            if (_detailsName != null)
                _detailsName.text = item.DisplayName;

            if (_detailsDescription != null)
                _detailsDescription.text = item.Description;

            if (_deleteButton != null)
                _deleteButton.interactable = true;
        }

        private void OnDeleteClicked()
        {
            HideContextMenu();

            if (_controller == null)
                return;

            var ok = _controller.TryRemoveSelected();
            if (!ok)
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
            HideContextMenu();
            _controller?.Select(index);
        }

        public void OnSlotRightClicked(int index, Vector2 screenPosition)
        {
            if (_controller == null)
                return;

            _controller.Select(index);

            var item = _controller.GetItemAt(index);

            if (item == null)
            {
                HideContextMenu();
                return;
            }

            if (_contextMenu == null)
                return;

            _contextMenu.Show(screenPosition, "Remove", () =>
            {
                var ok = _controller.TryRemoveAt(index);

                if (ok == false)
                    ShowStatus("Remove failed.");
            });
        }

        public void OnTopItemRightClicked(ItemDefinition item, Vector2 screenPosition)
        {
            if (_controller == null || item == null || _contextMenu == null)
                return;

            _contextMenu.Show(screenPosition, "Add", () =>
            {
                var ok = _controller.TryAdd(item);

                if (ok == false)
                    ShowStatus("Inventory is full.");
            });
        }
    }
}