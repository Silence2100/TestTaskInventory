using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Inventory.View
{
    public class InventorySlotView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _selection;

        private int _index;
        private InventoryView _owner;

        public void Initialize(InventoryView owner, int index)
        {
            _owner = owner;
            _index = index;
            SetSelected(false);
            SetItemIcon(null);
        }

        public void SetItemIcon(Sprite sprite)
        {
            _icon.enabled = sprite != null;
            _icon.sprite = sprite;
        }

        public void SetSelected(bool selected)
        {
            if (_selection != null)
                _selection.SetActive(selected);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_owner == null)
                return;

            if (eventData.button == PointerEventData.InputButton.Left)
                _owner.OnSlotLeftClicked(_index);

            if (eventData.button == PointerEventData.InputButton.Right)
                _owner.OnSlotRightClicked(_index, eventData.position);
        }
    }
}