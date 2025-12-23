using UnityEngine;

namespace Inventory.View
{
    public class TopBarView : MonoBehaviour
    {
        [SerializeField] private ItemSourceView[] _items;

        public void RefreshAll()
        {
            foreach (var item in _items)
            {
                if (item != null)
                    item.Refresh();
            }
        }
    }
}