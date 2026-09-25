using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace xLoc
{
    /// <summary>
    /// Static facade providing global access to localization tables, active locale configuration, and translation resolution.
    /// </summary>
    public static class xLoc
    {
        private static LocaleRegistry _registry;
        private static FontRegistry _fontRegistry = FontRegistry.Create(null);

        /// <summary>
        /// Indicates whether <see cref="xLoc"/> has been initialized.
        /// </summary>
        public static bool IsInitialized => _registry != null;

        /// <summary>
        /// Gets the current <see cref="LocaleRegistry"/>.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown if accessed before <see cref="Initialize"/>.</exception>
        internal static LocaleRegistry Registry =>
            _registry ?? throw new InvalidOperationException("xLoc is not initialized. Call xLoc.Initialize() before accessing localization.");

        /// <summary>
        /// Gets the current active locale key.
        /// </summary>
        public static LocaleKey CurrentLocale => Registry.CurrentLocale;

        /// <summary>
        /// Gets all registered locale keys.
        /// </summary>
        public static IReadOnlyList<LocaleKey> Locales => Registry.Locales;

        /// <summary>
        /// Initializes the localization system by discovering and loading all localization assets from Resources.
        /// </summary>
        /// <param name="path">Resources subfolder path containing localization files. Defaults to "Localization".</param>
        public static void Initialize(string path = "Localization")
        {
            var loader = new LocaleLoader();
            try
            {
                _registry = loader.Load(path);
            }
            catch (LocalizationNotFoundException)
            {
                // Gracefully abort if no localization assets exist
            }

            var settings = Resources.Load<xLocSettings>($"{path}/xLocSettings");
            _fontRegistry = FontRegistry.Create(settings != null ? settings.LocaleFonts : null);
        }

        /// <summary>
        /// Resolves the localized font and scale multiplier for the specified base font.
        /// </summary>
        internal static (TMP_FontAsset Font, float Scale) ResolveFont(TMP_FontAsset sourceFont)
        {
            if (!IsInitialized)
                return (sourceFont, 1f);

            return _fontRegistry.ResolveFont(CurrentLocale, sourceFont);
        }

        /// <summary>
        /// Switches the active locale across the entire game.
        /// </summary>
        /// <param name="locale">Target locale key.</param>
        public static void SetLocale(LocaleKey locale) => Registry.SetLocale(locale);

        /// <summary>
        /// Resolves the translation for the specified key in the active locale, or returns the fallback.
        /// </summary>
        /// <param name="key">Dot-notation translation key.</param>
        /// <param name="fallback">Fallback string if key is missing.</param>
        /// <returns>Translated string, fallback, or key.</returns>
        public static string Get(string key, string fallback = null) => Registry.ActiveTable.Get(key, fallback);

        /// <summary>
        /// Resolves the translation for the given <see cref="LocalizedString"/> in the active locale.
        /// </summary>
        /// <param name="loc">Localized string reference.</param>
        /// <returns>Translated string or fallback.</returns>
        public static string Get(in LocalizedString loc) => Registry.ActiveTable.Get(loc.Key, loc.Fallback);

        /// <summary>
        /// Attempts to get a translated value for the specified key in the active locale.
        /// </summary>
        /// <param name="key">Dot-notation translation key.</param>
        /// <param name="value">Translated text if found.</param>
        /// <returns>True if translation was found, false otherwise.</returns>
        public static bool TryGet(string key, out string value) => Registry.ActiveTable.TryGet(key, out value);
    }
}
