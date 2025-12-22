using System;
using UnityEngine;
using Inventory.Model;

namespace Inventory.Controller
{
    public sealed class InventoryController : MonoBehaviour
    {
        private InventoryModel _model;
        private int _selectedIndex = -1;

        public event Action InventoryChanged;
        public event Action<int> SelectionChanged;

        public int Capacity => _model != null ? _model.Capacity : 0;
        public int SelectedIndex => _selectedIndex;

        public void Initialize(InventoryModel runtimeModel)
        {
            _model = runtimeModel;
            _selectedIndex = -1;

            InventoryChanged?.Invoke();
            SelectionChanged?.Invoke(_selectedIndex);
        }

        public ItemDefinition GetItemAt(int slotIndex)
        {
            if (_model == null) 
                return null;

            if (IsIndexValid(slotIndex) == false) 
                return null;

            return _model.Slots[slotIndex];
        }

        public void Select(int slotIndex)
        {
            if (_model == null) 
                return;

            if (IsIndexValid(slotIndex) == false)
                slotIndex = -1;

            if (_selectedIndex == slotIndex)
                return;

            _selectedIndex = slotIndex;
            SelectionChanged?.Invoke(_selectedIndex);
        }

        public bool TryAdd(ItemDefinition item)
        {
            if (_model == null) 
                return false;

            if (item == null) 
                return false;

            var slots = _model.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                    continue;

                slots[i] = item;
                InventoryChanged?.Invoke();

                if (_selectedIndex < 0)
                    Select(i);

                return true;
            }

            return false;
        }

        public bool TryRemoveAt(int slotIndex)
        {
            if (_model == null) 
                return false;

            if (IsIndexValid(slotIndex) == false) 
                return false;

            var slots = _model.Slots;

            if (slots[slotIndex] == null)
                return false;

            slots[slotIndex] = null;
            InventoryChanged?.Invoke();

            if (_selectedIndex == slotIndex)
                SelectionChanged?.Invoke(_selectedIndex);

            return true;
        }

        public bool TryRemoveSelected()
        {
            if (_selectedIndex < 0) 
                return false;

            return TryRemoveAt(_selectedIndex);
        }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < _model.Slots.Length;
        }
    }
}