using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Inventory/Inventory Model")]
    public class InventoryModel : ScriptableObject
    {
        [SerializeField] private int _capacity = 20;
        [SerializeField] private ItemDefinition[] _slots;

        public int Capacity => _capacity;
        public ItemDefinition[] Slots => _slots;

        private void OnEnable()
        {
            EnsureSlotsSize();
        }

        private void OnValidate()
        {
            EnsureSlotsSize();
        }

        private void EnsureSlotsSize()
        {
            if (_slots == null || _slots.Length != _capacity)
                _slots = new ItemDefinition[_capacity];
        }
    }
}