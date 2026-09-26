namespace xlLoc.Core
{
    /// <summary>
    /// Contract for loaders that populate a <see cref="LocaleTable"/> from serialized text.
    /// </summary>
    internal interface ILocaleTableLoader
    {
        /// <summary>
        /// Parses the input text and populates the given locale table with translation key-value pairs.
        /// </summary>
        /// <param name="text">Raw source text content.</param>
        /// <param name="table">Reference to the target locale table to populate.</param>
        void Populate(string text, ref LocaleTable table);
    }
}
