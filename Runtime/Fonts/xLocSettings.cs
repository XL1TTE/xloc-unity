using System;
using System.Collections.Generic;
using UnityEngine;

namespace xLoc
{
    /// <summary>
    /// Runtime ScriptableObject storing font localization configurations.
    /// </summary>
    public sealed class xLocSettings : ScriptableObject
    {
        [SerializeField] private List<LocaleFontConfig> _localeFonts = new();

        /// <summary>
        /// Read-only collection of font configurations across all locales.
        /// </summary>
        public IReadOnlyList<LocaleFontConfig> LocaleFonts => _localeFonts;

        /// <summary>
        /// Atomically sets the font configurations while enforcing uniqueness.
        /// </summary>
        /// <param name="configs">Collection of locale font configurations.</param>
        public void SetConfigs(IEnumerable<LocaleFontConfig> configs)
        {
            if (configs == null)
                throw new ArgumentNullException(nameof(configs));

            _localeFonts.Clear();
            var registeredLocales = new HashSet<LocaleKey>();

            foreach (var config in configs)
            {
                if (config == null)
                    throw new ArgumentNullException(nameof(configs), "Config entry cannot be null.");

                if (!registeredLocales.Add(config.Locale))
                    throw new InvalidOperationException($"Duplicate font configuration detected for locale '{config.Locale}'.");

                _localeFonts.Add(config);
            }
        }
    }
}
