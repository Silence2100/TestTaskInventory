using TMPro;
using UnityEngine;

namespace Inventory.View
{
    public class StatusView : MonoBehaviour
    {
        [SerializeField] private TMP_Text _text;

        public void Show(string message)
        {
            if (_text == null)
                return;

            _text.text = message;
        }

        public void Clear()
        {
            if (_text == null)
                return;

            _text.text = string.Empty;
        }
    }
}