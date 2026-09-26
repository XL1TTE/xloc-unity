using System;
using TMPro;
using UnityEngine;

namespace xlLoc.Fonts
{
    /// <summary>
    /// Represents a mapping between a base font asset and a localized target font asset with an optional scale multiplier.
    /// </summary>
    [Serializable]
    public struct FontMapping : IEquatable<FontMapping>
    {
        [SerializeField] private TMP_FontAsset _sourceFont;
        [SerializeField] private TMP_FontAsset _targetFont;
        [SerializeField] private float _scaleMultiplier;

        /// <summary>
        /// Source font asset used by the UI elements.
        /// </summary>
        public TMP_FontAsset SourceFont => _sourceFont;

        /// <summary>
        /// Localized replacement font asset.
        /// </summary>
        public TMP_FontAsset TargetFont => _targetFont;

        /// <summary>
        /// Relative font size scale multiplier (defaults to 1.0f).
        /// </summary>
        public float ScaleMultiplier => _scaleMultiplier > 0f ? _scaleMultiplier : 1f;

        public FontMapping(TMP_FontAsset sourceFont, TMP_FontAsset targetFont, float scaleMultiplier = 1f)
        {
            _sourceFont = sourceFont;
            _targetFont = targetFont;
            _scaleMultiplier = scaleMultiplier > 0f ? scaleMultiplier : 1f;
        }

        public bool Equals(FontMapping other) =>
            Equals(_sourceFont, other._sourceFont) &&
            Equals(_targetFont, other._targetFont) &&
            Mathf.Approximately(ScaleMultiplier, other.ScaleMultiplier);

        public override bool Equals(object obj) =>
            obj is FontMapping other && Equals(other);

        public override int GetHashCode() =>
            HashCode.Combine(_sourceFont, _targetFont, ScaleMultiplier);

        public static bool operator ==(FontMapping left, FontMapping right) => left.Equals(right);
        public static bool operator !=(FontMapping left, FontMapping right) => !left.Equals(right);
    }
}
