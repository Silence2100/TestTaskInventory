using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;

namespace Inventory.View
{
    public sealed class ItemSourceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _label;
        [SerializeField] private ItemDefinition _item;

        public ItemDefinition Item => _item;

        public void Refresh()
        {
            if (_icon != null)
            {
                _icon.enabled = _item != null && _item.Icon != null;
                _icon.sprite = _item != null ? _item.Icon : null;
            }

            if (_label != null)
                _label.text = _item != null ? _item.DisplayName : string.Empty;
        }
    }
}