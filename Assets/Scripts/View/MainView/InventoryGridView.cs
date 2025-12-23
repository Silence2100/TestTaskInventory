using System.Collections.Generic;
using UnityEngine;

namespace Inventory.View
{
    public class InventoryGridView : MonoBehaviour
    {
        [SerializeField] private Transform _gridRoot;
        [SerializeField] private InventorySlotView _slotPrefab;

        private readonly List<InventorySlotView> _slots = new();

        public int SlotCount => _slots.Count;

        public void Build(int capacity)
        {
            Clear();

            if (_gridRoot == null || _slotPrefab == null)
            {
                Debug.LogError("InventoryGridView.Build: не назначены ссылки в инспекторе (gridRoot/slotPrefab).");

                return;
            }

            for (int i = 0; i < capacity; i++)
            {
                var slot = Instantiate(_slotPrefab, _gridRoot);
                slot.Initialize(i);
                _slots.Add(slot);
            }
        }

        public void SetSlotIcon(int index, Sprite icon)
        {
            if (IsIndexValid(index) == false)
                return;

            _slots[index].SetItemIcon(icon);
        }

        public void SetSelectedIndex(int selectedIndex)
        {
            for (int i = 0; i < _slots.Count; i++)
                _slots[i].SetSelected(i == selectedIndex);
        }

        public void Clear()
        {
            for (int i = 0; i < _slots.Count; i++)
            {
                if (_slots[i] != null)
                    Destroy(_slots[i].gameObject);
            }

            _slots.Clear();
        }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < _slots.Count;
        }
    }
}