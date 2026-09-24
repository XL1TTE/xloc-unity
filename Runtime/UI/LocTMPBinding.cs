using TMPro;
using UnityEngine;

namespace xLoc.UI
{
    /// <summary>
    /// Lightweight helper component attached to a <see cref="TMP_Text"/> GameObject that automatically updates text when the locale changes.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("")]
    internal sealed class LocTMPBinding : MonoBehaviour
    {
        private TMP_Text _text;
        private string _key;
        private string _fallback;
        private bool _isSubscribed;

        private TMP_Text Text => _text != null ? _text : _text = GetComponent<TMP_Text>();

        /// <summary>
        /// Binds a dot-notation key and optional fallback to the text component.
        /// </summary>
        /// <param name="key">Dot-notation translation key.</param>
        /// <param name="fallback">Fallback string if missing.</param>
        public void Bind(string key, string fallback = null)
        {
            _key = key;
            _fallback = fallback;
            UpdateText();
        }

        /// <summary>
        /// Binds a <see cref="LocalizedString"/> to the text component.
        /// </summary>
        /// <param name="loc">Localized string reference.</param>
        public void Bind(in LocalizedString loc)
        {
            _key = loc.Key;
            _fallback = loc.Fallback;
            UpdateText();
        }

        private void OnEnable()
        {
            Subscribe();
            UpdateText();
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void UpdateText()
        {
            if (string.IsNullOrEmpty(_key) || !xLoc.IsInitialized)
                return;

            Text.text = xLoc.Get(_key, _fallback);
        }

        private void Subscribe()
        {
            if (_isSubscribed || !xLoc.IsInitialized)
                return;

            xLoc.Registry.OnLocaleChanged += UpdateText;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || !xLoc.IsInitialized)
                return;

            xLoc.Registry.OnLocaleChanged -= UpdateText;
            _isSubscribed = false;
        }
    }
}
