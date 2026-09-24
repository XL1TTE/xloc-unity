using System;
using System.Collections.Generic;

namespace xLoc
{
    /// <summary>
    /// Holds a collection of key-value translation pairs for a single locale.
    /// </summary>
    internal sealed class LocaleTable
    {
        private readonly Dictionary<string, string> _entries = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets the locale associated with this table.
        /// </summary>
        public LocaleKey Locale { get; }

        /// <summary>
        /// Gets the number of translation entries stored in this table.
        /// </summary>
        public int Count => _entries.Count;

        /// <summary>
        /// Initializes a new instance of <see cref="LocaleTable"/> for the specified locale.
        /// </summary>
        /// <param name="locale">Target locale.</param>
        public LocaleTable(LocaleKey locale)
        {
            Locale = locale;
        }

        /// <summary>
        /// Attempts to retrieve a translation for the given key.
        /// </summary>
        /// <param name="key">Dot-separated translation key.</param>
        /// <param name="value">The translated value if found.</param>
        /// <returns>True if translation was found, false otherwise.</returns>
        public bool TryGet(string key, out string value) => _entries.TryGetValue(key, out value);

        /// <summary>
        /// Retrieves a translation for the given key, or returns the fallback if missing.
        /// </summary>
        /// <param name="key">Dot-separated translation key.</param>
        /// <param name="fallback">Fallback string if key is missing.</param>
        /// <returns>The translated value, fallback, or key if fallback is null.</returns>
        public string Get(string key, string fallback = null)
        {
            if (_entries.TryGetValue(key, out var val))
                return val;

            return fallback ?? key ?? string.Empty;
        }

        /// <summary>
        /// Adds or overwrites a translation key-value entry.
        /// </summary>
        /// <param name="key">Dot-separated translation key.</param>
        /// <param name="value">Translation text.</param>
        public void Set(string key, string value)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentException("Translation key cannot be null or empty.", nameof(key));

            _entries[key] = value ?? string.Empty;
        }

        /// <summary>
        /// Checks whether this table contains the given translation key.
        /// </summary>
        /// <param name="key">Dot-separated translation key.</param>
        /// <returns>True if present, false otherwise.</returns>
        public bool Contains(string key) => _entries.ContainsKey(key);
    }
}
