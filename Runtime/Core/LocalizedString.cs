using System;

namespace xlLoc.Core
{
    /// <summary>
    /// Represents a localized text reference containing a dot-notation key and optional fallback string.
    /// </summary>
    [Serializable]
    public struct LocalizedString : IEquatable<LocalizedString>
    {
        /// <summary>
        /// Dot-notation key identifying the translation entry (e.g. "chips.red.name").
        /// </summary>
        public string Key;

        /// <summary>
        /// Fallback string to use if translation is missing for the active locale.
        /// </summary>
        public string Fallback;

        /// <summary>
        /// Initializes a new instance of <see cref="LocalizedString"/>.
        /// </summary>
        /// <param name="key">Dot-notation translation key.</param>
        /// <param name="fallback">Optional fallback string.</param>
        public LocalizedString(string key, string fallback = null)
        {
            Key = key;
            Fallback = fallback;
        }

        /// <summary>
        /// Resolves the current translated value from <see cref="xLoc"/>.
        /// </summary>
        public string Value => xLoc.Get(this);

        /// <summary>
        /// Indicates whether both key and fallback are empty or null.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(Key) && string.IsNullOrEmpty(Fallback);

        public static implicit operator string(LocalizedString loc) => loc.Value;

        public override string ToString() => Value;

        public bool Equals(LocalizedString other) =>
            string.Equals(Key, other.Key, StringComparison.Ordinal) &&
            string.Equals(Fallback, other.Fallback, StringComparison.Ordinal);

        public override bool Equals(object obj) => obj is LocalizedString other && Equals(other);

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Key != null ? StringComparer.Ordinal.GetHashCode(Key) : 0) * 397) ^
                       (Fallback != null ? StringComparer.Ordinal.GetHashCode(Fallback) : 0);
            }
        }

        public static bool operator ==(LocalizedString left, LocalizedString right) => left.Equals(right);
        public static bool operator !=(LocalizedString left, LocalizedString right) => !left.Equals(right);
    }
}
