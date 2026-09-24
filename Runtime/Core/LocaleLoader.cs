using System;
using System.Collections.Generic;
using UnityEngine;

namespace xLoc
{
    /// <summary>
    /// Discovers and loads localization <see cref="TextAsset"/> resources, constructing a populated <see cref="LocaleRegistry"/>.
    /// </summary>
    internal sealed class LocaleLoader
    {
        private readonly ILocaleTableLoader _yamlLoader = new YamlLocaleTableLoader();

        /// <summary>
        /// Loads all localization assets from the specified Resources folder and builds a <see cref="LocaleRegistry"/>.
        /// </summary>
        /// <param name="path">Resources subfolder path.</param>
        /// <returns>A populated <see cref="LocaleRegistry"/>.</returns>
        /// <exception cref="LocalizationNotFoundException">Thrown if no valid assets exist at the given path.</exception>
        public LocaleRegistry Load(string path = "Localization")
        {
            var textAssets = Resources.LoadAll<TextAsset>(path);
            if (textAssets == null || textAssets.Length == 0)
                throw new LocalizationNotFoundException(path);

            var tables = new Dictionary<LocaleKey, LocaleTable>();

            for (int i = 0; i < textAssets.Length; i++)
            {
                var asset = textAssets[i];
                if (asset == null || string.IsNullOrWhiteSpace(asset.text))
                    continue;

                string rawLocale = asset.name.ExtractLocaleSuffix();
                var localeKey = new LocaleKey(rawLocale);

                if (!tables.TryGetValue(localeKey, out var table))
                {
                    table = new LocaleTable(localeKey);
                    tables.Add(localeKey, table);
                }

                ILocaleTableLoader loader = asset.text.GetExpectedFileFormat() switch
                {
                    FileFormat.Yaml => _yamlLoader,
                    _ => throw new NotSupportedException($"Unsupported or unparseable format in asset '{asset.name}'.")
                };

                loader.Populate(asset.text, ref table);
            }

            if (tables.Count == 0)
                throw new LocalizationNotFoundException(path);

            var registry = LocaleRegistry.Create(tables.Count);
            foreach (var table in tables.Values)
            {
                registry.WithLocale(table);
            }

            return registry;
        }
    }
}
