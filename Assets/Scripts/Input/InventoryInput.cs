using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using Inventory.View;

namespace Inventory.Input
{
    public sealed class InventoryInput : MonoBehaviour
    {
        [SerializeField] private InventoryView _view;
        [SerializeField] private GraphicRaycaster _raycaster;
        [SerializeField] private EventSystem _eventSystem;

        [Header("Input Actions (drag from .inputactions asset)")]
        [SerializeField] private InputActionReference _point;
        [SerializeField] private InputActionReference _leftClick;
        [SerializeField] private InputActionReference _rightClick;
        [SerializeField] private InputActionReference _cancel;

        private PointerEventData _pointerEventData;
        private readonly List<RaycastResult> _raycastResults = new();

        private void Awake()
        {
            if (_eventSystem == null)
                _eventSystem = EventSystem.current;

            _pointerEventData = new PointerEventData(_eventSystem);
        }

        private void OnEnable()
        {
            EnableAction(_point);
            EnableAction(_leftClick);
            EnableAction(_rightClick);
            EnableAction(_cancel);

            if (_leftClick != null) _leftClick.action.canceled += OnLeftClickReleased;
            if (_rightClick != null) _rightClick.action.performed += OnRightClickPerformed;
            if (_cancel != null) _cancel.action.performed += OnCancelPerformed;
        }

        private void OnDisable()
        {
            if (_leftClick != null) 
                _leftClick.action.canceled -= OnLeftClickReleased;

            if (_rightClick != null) 
                _rightClick.action.performed -= OnRightClickPerformed;

            if (_cancel != null) 
                _cancel.action.performed -= OnCancelPerformed;

            DisableAction(_point);
            DisableAction(_leftClick);
            DisableAction(_rightClick);
            DisableAction(_cancel);
        }

        private void OnLeftClickReleased(UnityEngine.InputSystem.InputAction.CallbackContext context)
        {
            if (IsReady() == false)
                return;

            if (TryRaycast(out var hitGo, out _) == false)
            {
                _view.HideContextMenu();

                return;
            }

            if (hitGo.GetComponentInParent<UnityEngine.UI.Button>() != null)
                return;

            if (TryGetComponentInParent(hitGo, out Inventory.View.InventorySlotView slot))
            {
                _view.OnSlotLeftClicked(slot.Index);

                return;
            }

            _view.HideContextMenu();
        }

        private void OnRightClickPerformed(InputAction.CallbackContext context)
        {
            if (IsReady() == false)
                return;

            if (TryRaycast(out var hitGo, out var screenPos) == false)
            {
                _view.HideContextMenu();

                return;
            }

            if (TryGetComponentInParent(hitGo, out ItemSourceView topItem))
            {
                if (topItem.Item != null)
                    _view.OnTopItemRightClicked(topItem.Item, screenPos);

                return;
            }

            if (TryGetComponentInParent(hitGo, out InventorySlotView slot))
            {
                _view.OnSlotRightClicked(slot.Index, screenPos);

                return;
            }

            _view.HideContextMenu();
        }

        private void OnCancelPerformed(InputAction.CallbackContext context)
        {
            if (_view == null) 
                return;

            _view.HideContextMenu();
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
            if (_point != null)
                return _point.action.ReadValue<Vector2>();

            if (Mouse.current != null)
                return Mouse.current.position.ReadValue();

            return Vector2.zero;
        }

        private bool IsReady()
        {
            return _view != null && _raycaster != null && _eventSystem != null;
        }

        private static void EnableAction(InputActionReference reference)
        {
            if (reference == null) 
                return;

            reference.action.Enable();
        }

        private static void DisableAction(InputActionReference reference)
        {
            if (reference == null) 
                return;

            reference.action.Disable();
        }

        private static bool TryGetComponentInParent<T>(GameObject go, out T component) where T : Component
        {
            component = go != null ? go.GetComponentInParent<T>() : null;

            return component != null;
        }
    }
}