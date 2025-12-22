using UnityEngine;
using Inventory.Model;

namespace Inventory.Controller
{
    public sealed class InventoryControllerDebug : MonoBehaviour
    {
        [SerializeField] private InventoryController _controller;
        [SerializeField] private InventoryModel _inventoryTemplate;
        [SerializeField] private ItemDefinition _itemA;
        [SerializeField] private ItemDefinition _itemB;

        private void Awake()
        {
            var runtimeModel = Instantiate(_inventoryTemplate);
            _controller.Initialize(runtimeModel);

            _controller.InventoryChanged += () => Debug.Log("InventoryChanged");
            _controller.SelectionChanged += i => Debug.Log($"SelectionChanged: {i}");
        }

        [ContextMenu("Add Item A")]
        private void AddA()
        {
            var ok = _controller.TryAdd(_itemA);
            Debug.Log($"TryAdd A: {ok}");
        }

        [ContextMenu("Add Item B")]
        private void AddB()
        {
            var ok = _controller.TryAdd(_itemB);
            Debug.Log($"TryAdd B: {ok}");
        }

        [ContextMenu("Remove Selected")]
        private void RemoveSelected()
        {
            var ok = _controller.TryRemoveSelected();
            Debug.Log($"RemoveSelected: {ok}");
        }

        [ContextMenu("Select 0")]
        private void Select0()
        {
            _controller.Select(0);
        }
    }
}