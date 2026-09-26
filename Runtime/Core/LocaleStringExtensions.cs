using System;

namespace xlLoc.Core
{
    internal static class LocaleStringExtensions
    {
        internal static string ExtractLocaleSuffix(this string assetName)
        {
            if (string.IsNullOrWhiteSpace(assetName))
                throw new ArgumentException("Asset name cannot be null or whitespace.", nameof(assetName));

            int lastUnderscore = assetName.LastIndexOf('_');
            if (lastUnderscore >= 0 && lastUnderscore < assetName.Length - 1)
                return assetName.Substring(lastUnderscore + 1);

            return assetName;
        }

        internal static FileFormat GetExpectedFileFormat(this string text)
        {
            if (string.IsNullOrWhiteSpace(text))
                return FileFormat.Unknown;

            ReadOnlySpan<char> span = text.AsSpan().TrimStart();
            if (span.Length == 0)
                return FileFormat.Unknown;

            if (span[0] == '{' || span[0] == '[')
                return FileFormat.Json;

            return FileFormat.Yaml;
        }
    }
}
