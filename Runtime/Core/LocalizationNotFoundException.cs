using System;

namespace xLoc
{
    /// <summary>
    /// Exception thrown when no localization files or assets can be found at the specified path.
    /// </summary>
    public sealed class LocalizationNotFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of <see cref="LocalizationNotFoundException"/>.
        /// </summary>
        /// <param name="path">Resources or directory path searched.</param>
        public LocalizationNotFoundException(string path)
            : base($"No localization assets found at '{path}'.") { }
    }
}
