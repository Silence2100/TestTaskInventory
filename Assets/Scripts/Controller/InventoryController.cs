using System;
using UnityEngine;
using Inventory.Model;

namespace Inventory.Controller
{
    public class InventoryController : MonoBehaviour
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
            if (IsIndexValid(slotIndex) == false) 
                return null;

            return _model.Slots[slotIndex];
        }

        public void Select(int slotIndex)
        {
            if (IsIndexValid(slotIndex) == false)
                slotIndex = -1;

            if (_selectedIndex == slotIndex)
                return;

            _selectedIndex = slotIndex;
            SelectionChanged?.Invoke(_selectedIndex);
        }

        public bool TryAdd(ItemDefinition item)
        {
            var slots = _model.Slots;

            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i] != null)
                    continue;

                slots[i] = item;
                InventoryChanged?.Invoke();

                return true;
            }

            return false;
        }

        public bool TryRemoveAt(int slotIndex)
        {
            if (IsIndexValid(slotIndex) == false)
                return false;

            var slots = _model.Slots;

            if (slots[slotIndex] == null)
                return false;

            slots[slotIndex] = null;

            CompactSlots();

            InventoryChanged?.Invoke();

            return true;
        }

        public bool TryRemoveSelected()
        {
            if (_selectedIndex < 0) 
                return false;

            return TryRemoveAt(_selectedIndex);
        }

        private void CompactSlots()
        {
            var slots = _model.Slots;

            int writeIndex = 0;

            for (int readIndex = 0; readIndex < slots.Length; readIndex++)
            {
                var item = slots[readIndex];
                if (item == null)
                    continue;

                if (writeIndex != readIndex)
                    slots[writeIndex] = item;

                writeIndex++;
            }

            for (int i = writeIndex; i < slots.Length; i++)
                slots[i] = null;
        }

        private bool IsIndexValid(int index)
        {
            return index >= 0 && index < _model.Slots.Length;
        }
    }
}