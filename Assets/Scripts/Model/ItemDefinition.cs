using UnityEngine;

namespace Inventory.Model
{
    [CreateAssetMenu(menuName = "Inventory/Item Definition")]
    public class ItemDefinition : ScriptableObject
    {
        [SerializeField] private string _displayName;
        [SerializeField] private string _description;
        [SerializeField] private Sprite _icon;

        public string DisplayName => _displayName;
        public string Description => _description;
        public Sprite Icon => _icon;
    }
}