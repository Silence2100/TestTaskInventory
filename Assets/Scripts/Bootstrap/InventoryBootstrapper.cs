using UnityEngine;
using Inventory.Controller;
using Inventory.Model;
using Inventory.View;

namespace Inventory.Bootstrap
{
    public sealed class InventoryBootstrapper : MonoBehaviour
    {
        [SerializeField] private InventoryModel _inventoryTemplate;
        [SerializeField] private InventoryController _controller;
        [SerializeField] private InventoryView _view;

        private void Awake()
        {
            var runtimeModel = Instantiate(_inventoryTemplate);
            _controller.Initialize(runtimeModel);
            _view.Initialize(_controller);
        }
    }
}