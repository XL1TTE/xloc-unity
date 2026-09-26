using System;
using System.Collections;
using System.IO;
using YamlDotNet.Serialization;

namespace xlLoc.Core
{
    /// <summary>
    /// Loads and parses YAML content into a <see cref="LocaleTable"/>, flattening nested mappings into dot-notation keys.
    /// </summary>
    internal sealed class YamlLocaleTableLoader : ILocaleTableLoader
    {
        private readonly IDeserializer _deserializer = new DeserializerBuilder().Build();

        /// <summary>
        /// Populates the given <see cref="LocaleTable"/> by deserializing YAML text and flattening keys.
        /// </summary>
        /// <param name="text">YAML string content.</param>
        /// <param name="table">Reference to target locale table.</param>
        public void Populate(string text, ref LocaleTable table)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            using var reader = new StringReader(text);
            object root = _deserializer.Deserialize(reader);
            Flatten(root, string.Empty, ref table);
        }

        private static void Flatten(object node, string prefix, ref LocaleTable table)
        {
            if (node is IDictionary dict)
            {
                foreach (DictionaryEntry entry in dict)
                {
                    if (entry.Key == null) continue;
                    string key = entry.Key.ToString();
                    string nextPrefix = string.IsNullOrEmpty(prefix) ? key : $"{prefix}.{key}";
                    Flatten(entry.Value, nextPrefix, ref table);
                }
            }
            else if (node is IList list)
            {
                for (int i = 0; i < list.Count; i++)
                {
                    string nextPrefix = string.IsNullOrEmpty(prefix) ? i.ToString() : $"{prefix}.{i}";
                    Flatten(list[i], nextPrefix, ref table);
                }
            }
            else if (node != null && !string.IsNullOrEmpty(prefix))
            {
                table.Set(prefix, node.ToString());
            }
        }
    }
}
