using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;

namespace Inventory.View
{
    public class ItemSourceView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private ItemDefinition _item;

        public ItemDefinition Item => _item;

        public void Refresh()
        {
            _icon.enabled = _item != null && _item.Icon != null;
            _icon.sprite = _item != null ? _item.Icon : null;
        }
    }
}