using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Inventory.Model;

namespace Inventory.View
{
    public class ItemSourceView : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private ItemDefinition _item;

        private InventoryView _owner;

        public ItemDefinition Item => _item;

        public void Initialize(InventoryView owner)
        {
            _owner = owner;

            if (_icon != null)
            {
                _icon.enabled = _item != null && _item.Icon != null;
                _icon.sprite = _item != null ? _item.Icon : null;
            }

            if (_label != null)
                _label.text = _item != null ? _item.DisplayName : string.Empty;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_owner == null || _item == null)
                return;

            if (eventData.button == PointerEventData.InputButton.Right)
                _owner.OnTopItemRightClicked(_item, eventData.position);
        }
    }
}