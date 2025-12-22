using UnityEngine;
using UnityEngine.UI;

namespace Inventory.View
{
    public sealed class InventorySlotView : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private GameObject _selection;

        private int _index;
        public int Index => _index;

        public void Initialize(int index)
        {
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
    }
}
