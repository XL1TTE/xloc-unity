using TMPro;
using UnityEngine;
using xlLoc.Core;

namespace xlLoc.UI
{
    /// <summary>
    /// Lightweight helper component attached to a <see cref="TMP_Text"/> GameObject that automatically updates text and localized fonts when the locale changes.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(TMP_Text))]
    [AddComponentMenu("")]
    internal sealed class LocTMPBinding : MonoBehaviour
    {
        private TMP_Text _text;
        private string _key;
        private string _fallback;
        private TMP_FontAsset _baseFont;
        private float _baseFontSize;
        private bool _isSubscribed;

        private TMP_Text Text => _text != null ? _text : _text = GetComponent<TMP_Text>();

        /// <summary>
        /// Binds an optional dot-notation key and fallback to the text component. If key is null or empty, only font localization applies.
        /// </summary>
        /// <param name="key">Optional dot-notation translation key.</param>
        /// <param name="fallback">Fallback string if missing.</param>
        public void Bind(string key = null, string fallback = null)
        {
            InitBaseFont();
            _key = key;
            _fallback = fallback;
            Subscribe();
            UpdateVisuals();
        }

        /// <summary>
        /// Binds a <see cref="LocalizedString"/> to the text component.
        /// </summary>
        /// <param name="loc">Localized string reference.</param>
        public void Bind(in LocalizedString loc) => Bind(loc.Key, loc.Fallback);

        private void OnEnable()
        {
            InitBaseFont();
            Subscribe();
            UpdateVisuals();
        }

        private void InitBaseFont()
        {
            if (Text != null && _baseFont == null)
            {
                _baseFont = Text.font;
                _baseFontSize = Text.fontSize;
            }
        }

        private void OnDisable()
        {
            Unsubscribe();
        }

        private void OnDestroy()
        {
            Unsubscribe();
        }

        private void UpdateVisuals()
        {
            if (!xLoc.IsInitialized || _baseFont == null)
                return;

            var (targetFont, scale) = xLoc.ResolveFont(_baseFont);
            if (targetFont != null && Text.font != targetFont)
                Text.font = targetFont;

            if (_baseFontSize > 0f)
                Text.fontSize = _baseFontSize * scale;

            if (!string.IsNullOrEmpty(_key))
                Text.text = xLoc.Get(_key, _fallback);
        }

        private void Subscribe()
        {
            if (_isSubscribed || !xLoc.IsInitialized)
                return;

            xLoc.Registry.OnLocaleChanged += UpdateVisuals;
            _isSubscribed = true;
        }

        private void Unsubscribe()
        {
            if (!_isSubscribed || !xLoc.IsInitialized)
                return;

            xLoc.Registry.OnLocaleChanged -= UpdateVisuals;
            _isSubscribed = false;
        }
    }
}
