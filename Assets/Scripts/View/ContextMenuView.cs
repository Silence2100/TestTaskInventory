using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Inventory.View
{
    public class ContextMenuView : MonoBehaviour
    {
        [SerializeField] private Button _actionButton;
        [SerializeField] private TMP_Text _actionLabel;

        private Action OnClick;

        private void Awake()
        {
            Hide();
            _actionButton.onClick.AddListener(OnActionClicked);
        }

        public void Show(Vector2 screenPosition, string label, Action onClick)
        {
            OnClick = onClick;
            _actionLabel.text = label;

            transform.position = screenPosition;

            gameObject.SetActive(true);
        }

        public void Hide()
        {
            OnClick = null;
            gameObject.SetActive(false);
        }

        private void OnActionClicked()
        {
            var action = OnClick;
            Hide();
            action?.Invoke();
        }
    }
}