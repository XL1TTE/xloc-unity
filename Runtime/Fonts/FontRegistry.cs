using System.Collections.Generic;
using TMPro;

namespace xLoc
{
    /// <summary>
    /// Internal domain service providing fast font resolution for locales.
    /// </summary>
    internal sealed class FontRegistry
    {
        private readonly struct ResolvedLocaleFonts
        {
            public readonly TMP_FontAsset DefaultFont;
            public readonly Dictionary<TMP_FontAsset, (TMP_FontAsset Target, float Scale)> Mappings;

            public ResolvedLocaleFonts(TMP_FontAsset defaultFont, Dictionary<TMP_FontAsset, (TMP_FontAsset Target, float Scale)> mappings)
            {
                DefaultFont = defaultFont;
                Mappings = mappings;
            }
        }

        private readonly Dictionary<LocaleKey, ResolvedLocaleFonts> _configs = new();

        public static FontRegistry Create(IReadOnlyList<LocaleFontConfig> configs)
        {
            var registry = new FontRegistry();
            if (configs == null)
                return registry;

            for (int i = 0; i < configs.Count; i++)
            {
                var cfg = configs[i];
                if (cfg == null)
                    continue;

                var map = new Dictionary<TMP_FontAsset, (TMP_FontAsset Target, float Scale)>();
                var mappings = cfg.FontMappings;
                if (mappings != null)
                {
                    for (int m = 0; m < mappings.Count; m++)
                    {
                        var entry = mappings[m];
                        if (entry.SourceFont != null && entry.TargetFont != null)
                        {
                            map[entry.SourceFont] = (entry.TargetFont, entry.ScaleMultiplier);
                        }
                    }
                }

                registry._configs[cfg.Locale] = new ResolvedLocaleFonts(cfg.DefaultFont, map);
            }

            return registry;
        }

        public (TMP_FontAsset Font, float Scale) ResolveFont(LocaleKey locale, TMP_FontAsset sourceFont)
        {
            if (!_configs.TryGetValue(locale, out var resolved))
                return (sourceFont, 1f);

            if (sourceFont != null && resolved.Mappings.TryGetValue(sourceFont, out var mapped))
                return (mapped.Target, mapped.Scale);

            if (resolved.DefaultFont != null)
                return (resolved.DefaultFont, 1f);

            return (sourceFont, 1f);
        }
    }
}
