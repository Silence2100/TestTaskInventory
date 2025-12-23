using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Inventory.Model;
using Inventory.View;

namespace Inventory.Input
{
    public class InputHandler : MonoBehaviour
    {
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private EventSystem _eventSystem;

        private readonly List<RaycastResult> _raycastResults = new();

        private GameInputActions _actions;
        private PointerEventData _pointerEventData;

        public event Action<int> SlotLeftClicked;
        public event Action<int, Vector2> SlotRightClicked;
        public event Action<ItemDefinition, Vector2> TopItemRightClicked;
        public event Action<ItemDefinition> TopItemDoubleClicked;

        public event Action ClickedOutside;
        public event Action CancelPressed;
        public event Action DeletePressed;

        private void Awake()
        {
            if (_eventSystem == null)
                _eventSystem = EventSystem.current;

            _pointerEventData = new PointerEventData(_eventSystem);
            _actions = new GameInputActions();
        }

        private void OnEnable()
        {
            _actions.Enable();

            _actions.UI.Click.canceled += OnLeftClickReleased;
            _actions.UI.RightClick.performed += OnRightClickPerformed;
            _actions.UI.Cancel.performed += OnCancelPerformed;
            _actions.UI.Delete.performed += OnDeletePerformed;
            _actions.UI.DoubleClick.performed += OnDoubleClickPerformed;
        }

        private void OnDisable()
        {
            _actions.UI.Click.canceled -= OnLeftClickReleased;
            _actions.UI.RightClick.performed -= OnRightClickPerformed;
            _actions.UI.Cancel.performed -= OnCancelPerformed;
            _actions.UI.Delete.performed -= OnDeletePerformed;
            _actions.UI.DoubleClick.performed -= OnDoubleClickPerformed;

            _actions.Disable();
        }

        private void OnLeftClickReleased(InputAction.CallbackContext context)
        {
            if (IsReady() == false)
                return;

            if (TryRaycast(out var hitGo, out _) == false)
            {
                ClickedOutside?.Invoke();
                return;
            }

            if (hitGo.GetComponentInParent<Button>() != null)
                return;

            if (TryGetComponentInParent(hitGo, out InventorySlotView slot))
            {
                SlotLeftClicked?.Invoke(slot.Index);
                return;
            }

            ClickedOutside?.Invoke();
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            if (IsReady() == false)
                return;

            if (TryRaycast(out var hitGo, out var screenPos) == false)
            {
                ClickedOutside?.Invoke();
                return;
            }

            if (TryGetComponentInParent(hitGo, out ItemSourceView topItem))
            {
                if (topItem.Item != null)
                    TopItemRightClicked?.Invoke(topItem.Item, screenPos);
                else
                    ClickedOutside?.Invoke();

                return;
            }

            if (TryGetComponentInParent(hitGo, out InventorySlotView slot))
            {
                SlotRightClicked?.Invoke(slot.Index, screenPos);
                return;
            }

            ClickedOutside?.Invoke();
        }

        private void OnDoubleClickPerformed(InputAction.CallbackContext context)
        {
            if (IsReady() == false)
                return;

            if (TryRaycast(out var hitGo, out _) == false)
                return;

            if (hitGo.GetComponentInParent<Button>() != null)
                return;

            if (TryGetComponentInParent(hitGo, out ItemSourceView topItem) && topItem.Item != null)
                TopItemDoubleClicked?.Invoke(topItem.Item);
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            CancelPressed?.Invoke();
        }

        private void OnDeletePerformed(InputAction.CallbackContext context)
        {
            DeletePressed?.Invoke();
        }

        private bool TryRaycast(out GameObject hitGameObject, out Vector2 screenPosition)
        {
            screenPosition = ReadPointerPosition();
            hitGameObject = null;

            _pointerEventData.position = screenPosition;
            _raycastResults.Clear();
            _raycaster.Raycast(_pointerEventData, _raycastResults);

            if (_raycastResults.Count == 0)
                return false;

            hitGameObject = _raycastResults[0].gameObject;

            return true;
        }

        private Vector2 ReadPointerPosition()
        {
            return _actions.UI.Point.ReadValue<Vector2>();
        }

        private bool IsReady()
        {
            return _raycaster != null && _eventSystem != null;
        }

        private static bool TryGetComponentInParent<T>(GameObject gameObj, out T component) where T : Component
        {
            component = gameObj != null ? gameObj.GetComponentInParent<T>() : null;
            return component != null;
        }
    }
}