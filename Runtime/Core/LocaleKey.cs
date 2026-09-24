using System;

namespace xLoc
{
    /// <summary>
    /// Value object representing a validated locale identifier (e.g. "en", "ru", "pirate").
    /// </summary>
    [Serializable]
    public readonly struct LocaleKey : IEquatable<LocaleKey>, IComparable<LocaleKey>
    {
        private static readonly char[] InvalidChars = { '/', '\\', '.', ':', '*', '?', '"', '<', '>', '|' };

        private readonly string _value;

        /// <summary>
        /// Gets the normalized lowercase locale string.
        /// </summary>
        public string Value => _value ?? throw new InvalidOperationException("LocaleKey is uninitialized.");

        /// <summary>
        /// Indicates whether this instance is uninitialized or empty.
        /// </summary>
        public bool IsEmpty => string.IsNullOrEmpty(_value);

        /// <summary>
        /// Initializes a new instance of <see cref="LocaleKey"/> after validating and normalizing the input string.
        /// </summary>
        /// <param name="value">Locale identifier string.</param>
        public LocaleKey(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Locale key cannot be null or whitespace.", nameof(value));

            string trimmed = value.Trim();

            if (trimmed.IndexOfAny(InvalidChars) >= 0)
                throw new ArgumentException($"Locale key '{value}' contains invalid characters.", nameof(value));

            _value = trimmed.ToLowerInvariant();
        }

        public static implicit operator string(LocaleKey key) => key.Value;
        public static explicit operator LocaleKey(string value) => new(value);

        public override string ToString() => _value ?? string.Empty;

        public bool Equals(LocaleKey other) =>
            string.Equals(_value, other._value, StringComparison.Ordinal);

        public override bool Equals(object obj) =>
            obj is LocaleKey other && Equals(other);

        public override int GetHashCode() =>
            _value != null ? StringComparer.Ordinal.GetHashCode(_value) : 0;

        public int CompareTo(LocaleKey other) =>
            string.Compare(_value, other._value, StringComparison.Ordinal);

        public static bool operator ==(LocaleKey left, LocaleKey right) => left.Equals(right);
        public static bool operator !=(LocaleKey left, LocaleKey right) => !left.Equals(right);
    }
}
