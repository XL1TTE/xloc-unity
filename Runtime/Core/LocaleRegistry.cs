using System;
using System.Collections.Generic;

namespace xLoc
{
    /// <summary>
    /// Registry managing registered <see cref="LocaleTable"/> instances and active locale state.
    /// </summary>
    internal sealed class LocaleRegistry
    {
        private readonly Dictionary<LocaleKey, LocaleTable> _tables;
        private readonly List<LocaleKey> _locales;
        private LocaleKey _currentLocale;
        private LocaleTable _activeTable;

        /// <summary>
        /// Event fired whenever the active locale changes.
        /// </summary>
        public event Action OnLocaleChanged;

        /// <summary>
        /// Gets the current active locale key.
        /// </summary>
        public LocaleKey CurrentLocale => _currentLocale;

        /// <summary>
        /// Gets the list of all registered locale keys.
        /// </summary>
        public IReadOnlyList<LocaleKey> Locales => _locales;

        /// <summary>
        /// Gets the active locale table.
        /// </summary>
        public LocaleTable ActiveTable => _activeTable ?? throw new InvalidOperationException("No active locale table is set in LocaleRegistry.");

        private LocaleRegistry(int capacity)
        {
            _tables = new Dictionary<LocaleKey, LocaleTable>(capacity);
            _locales = new List<LocaleKey>(capacity);
        }

        /// <summary>
        /// Creates a new <see cref="LocaleRegistry"/> with pre-allocated capacity.
        /// </summary>
        /// <param name="capacity">Expected count of locales.</param>
        /// <returns>A new <see cref="LocaleRegistry"/> instance.</returns>
        public static LocaleRegistry Create(int capacity = 8) => new(capacity);

        /// <summary>
        /// Registers a <see cref="LocaleTable"/> into the registry. The first registered table becomes active by default.
        /// </summary>
        /// <param name="table">The table to register.</param>
        /// <returns>Current registry instance for fluent chaining.</returns>
        public LocaleRegistry WithLocale(LocaleTable table)
        {
            if (table == null)
                throw new ArgumentNullException(nameof(table));

            if (_tables.ContainsKey(table.Locale))
                throw new InvalidOperationException($"Locale '{table.Locale}' is already registered.");

            _tables.Add(table.Locale, table);
            _locales.Add(table.Locale);

            if (_activeTable == null)
            {
                _currentLocale = table.Locale;
                _activeTable = table;
            }

            return this;
        }

        /// <summary>
        /// Switches the active locale to the specified locale key.
        /// </summary>
        /// <param name="locale">Target locale.</param>
        public void SetLocale(LocaleKey locale)
        {
            if (!_tables.TryGetValue(locale, out var table))
                throw new KeyNotFoundException($"Locale '{locale}' is not registered in LocaleRegistry.");

            if (_currentLocale == locale && _activeTable != null)
                return;

            _currentLocale = locale;
            _activeTable = table;

            OnLocaleChanged?.Invoke();
        }

        /// <summary>
        /// Checks whether the specified locale is registered.
        /// </summary>
        /// <param name="locale">Target locale.</param>
        /// <returns>True if registered, false otherwise.</returns>
        public bool HasLocale(LocaleKey locale) => _tables.ContainsKey(locale);

        /// <summary>
        /// Attempts to get the <see cref="LocaleTable"/> for the specified locale.
        /// </summary>
        /// <param name="locale">Target locale.</param>
        /// <param name="table">The found table if successful.</param>
        /// <returns>True if found, false otherwise.</returns>
        public bool TryGetTable(LocaleKey locale, out LocaleTable table) => _tables.TryGetValue(locale, out table);
    }
}
