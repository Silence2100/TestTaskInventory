using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Inventory.Model;

namespace Inventory.View
{
    public class InventoryDetailsView : MonoBehaviour
    {
        public event Action DeleteClicked;

        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private TMP_Text _description;
        [SerializeField] private Button _deleteButton;

        private void Awake()
        {
            _deleteButton.onClick.RemoveListener(OnDeleteButtonClicked);
            _deleteButton.onClick.AddListener(OnDeleteButtonClicked);
        }

        public void SetItem(ItemDefinition item)
        {
            if (item == null)
            {
                _icon.enabled = false;
                _icon.sprite = null;

                _name.text = string.Empty;

                _description.text = string.Empty;

                SetDeleteInteractable(false);

                return;
            }

            _icon.enabled = item.Icon != null;
            _icon.sprite = item.Icon;

            _name.text = item.DisplayName;

            _description.text = item.Description;

            SetDeleteInteractable(true);
        }

        public void SetDeleteInteractable(bool interactable)
        {
            _deleteButton.interactable = interactable;
        }

        private void OnDeleteButtonClicked()
        {
            DeleteClicked?.Invoke();
        }
    }
}