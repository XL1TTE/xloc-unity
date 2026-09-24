using TMPro;

namespace xLoc.UI
{
    /// <summary>
    /// Extension methods for binding <see cref="TMP_Text"/> components to localized strings with automatic locale change updates.
    /// </summary>
    public static class LocUIExtensions
    {
        /// <summary>
        /// Binds a <see cref="TMP_Text"/> component to a dot-notation localization key, auto-updating on locale changes.
        /// </summary>
        /// <param name="text">Text component to bind.</param>
        /// <param name="key">Dot-notation translation key.</param>
        /// <param name="fallback">Fallback string if missing.</param>
        public static void BindLoc(this TMP_Text text, string key, string fallback = null)
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
        public static void BindLoc(this TMP_Text text, in LocalizedString loc)
        {
            if (text == null) return;
            if (!text.TryGetComponent<LocTMPBinding>(out var binding))
                binding = text.gameObject.AddComponent<LocTMPBinding>();

            binding.Bind(loc);
        }
    }
}
