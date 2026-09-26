using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using xlLoc.Core;

namespace xlLoc.Fonts
{
    /// <summary>
    /// Value object defining default and mapped fonts for a single locale.
    /// </summary>
    [Serializable]
    public sealed class LocaleFontConfig
    {
        [SerializeField] private string _locale;
        [SerializeField] private TMP_FontAsset _defaultFont;
        [SerializeField] private List<FontMapping> _fontMappings = new();

        /// <summary>
        /// Validated locale key.
        /// </summary>
        public LocaleKey Locale => new(_locale);

        /// <summary>
        /// Default fallback font asset for this locale.
        /// </summary>
        public TMP_FontAsset DefaultFont => _defaultFont;

        /// <summary>
        /// Read-only list of source-to-target font mappings.
        /// </summary>
        public IReadOnlyList<FontMapping> FontMappings => _fontMappings;

        public LocaleFontConfig(LocaleKey locale, TMP_FontAsset defaultFont = null, IEnumerable<FontMapping> mappings = null)
        {
            if (locale.IsEmpty)
                throw new ArgumentException("Locale cannot be empty.", nameof(locale));

            _locale = locale.Value;
            _defaultFont = defaultFont;

            if (mappings != null)
            {
                foreach (var mapping in mappings)
                {
                    _fontMappings.Add(mapping);
                }
            }
        }
    }
}
