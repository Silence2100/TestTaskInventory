using UnityEngine;
using Inventory.Controller;
using Inventory.Input;
using Inventory.Model;
using Inventory.View;

namespace Inventory.Bootstrap
{
    public class InventoryBootstraper : MonoBehaviour
    {
        [SerializeField] private InventoryModel _inventoryTemplate;
        [SerializeField] private InventoryController _controller;
        [SerializeField] private InventoryView _view;
        [SerializeField] private InputHandler _input;
        [SerializeField] private InventoryUIController _uiController;

        private void Awake()
        {
            var runtimeModel = Instantiate(_inventoryTemplate);

            _controller.Initialize(runtimeModel);
            _uiController.Initialize(_controller, _view, _input);
        }
    }
}