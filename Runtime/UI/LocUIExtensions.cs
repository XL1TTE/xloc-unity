using TMPro;
using xlLoc.Core;

namespace xlLoc.UI
{
    /// <summary>
    /// Extension methods for binding <see cref="TMP_Text"/> components to localized strings and fonts with automatic locale change updates.
    /// </summary>
    public static class LocUIExtensions
    {
        /// <summary>
        /// Binds a <see cref="TMP_Text"/> component to an optional dot-notation localization key and localized font, auto-updating on locale changes.
        /// </summary>
        /// <param name="text">Text component to bind.</param>
        /// <param name="key">Optional dot-notation translation key. If null or empty, only font localization applies.</param>
        /// <param name="fallback">Fallback string if missing.</param>
        public static void BindLoc(this TMP_Text text, string key = null, string fallback = null)
        {
            if (text == null) return;
            if (!text.TryGetComponent<LocTMPBinding>(out var binding))
                binding = text.gameObject.AddComponent<LocTMPBinding>();

            binding.Bind(key, fallback);
        }

        /// <summary>
        /// Binds a <see cref="TMP_Text"/> component to a <see cref="LocalizedString"/>, auto-updating on locale changes.
        /// </summary>
        /// <param name="text">Text component to bind.</param>
        /// <param name="loc">Localized string reference.</param>
        public static void BindLoc(this TMP_Text text, in LocalizedString loc) =>
            text.BindLoc(loc.Key, loc.Fallback);
    }
}
