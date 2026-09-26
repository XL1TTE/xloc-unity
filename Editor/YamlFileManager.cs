using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using xlLoc.Core;
using YamlDotNet.Serialization;

namespace xlLoc.Editor
{
    internal static class YamlFileManager
    {
        private const string LocalizationFolderPath = "Assets/Resources/Localization";
        private static readonly IDeserializer _deserializer = new DeserializerBuilder().Build();
        private static readonly ISerializer _serializer = new SerializerBuilder().Build();

        internal static List<LocaleKey> GetAvailableLocales()
        {
            var list = new List<LocaleKey>();
            if (!Directory.Exists(LocalizationFolderPath))
                return list;

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(LocalizationFolderPath, "*.yaml", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(LocalizationFolderPath, "*.yml", SearchOption.AllDirectories));

            var set = new HashSet<LocaleKey>();

            for (int i = 0; i < files.Count; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                string suffix = fileName.ExtractLocaleSuffix();
                if (!string.IsNullOrWhiteSpace(suffix))
                {
                    try
                    {
                        var key = new LocaleKey(suffix);
                        if (set.Add(key))
                            list.Add(key);
                    }
                    catch
                    {
                        // Ignore malformed file suffixes
                    }
                }
            }

            return list;
        }

        internal static List<string> GetFilesForLocale(LocaleKey locale)
        {
            var result = new List<string>();
            if (!Directory.Exists(LocalizationFolderPath))
                return result;

            var files = new List<string>();
            files.AddRange(Directory.GetFiles(LocalizationFolderPath, "*.yaml", SearchOption.AllDirectories));
            files.AddRange(Directory.GetFiles(LocalizationFolderPath, "*.yml", SearchOption.AllDirectories));

            for (int i = 0; i < files.Count; i++)
            {
                string fileName = Path.GetFileNameWithoutExtension(files[i]);
                string suffix = fileName.ExtractLocaleSuffix();
                if (string.Equals(suffix, locale.Value, StringComparison.OrdinalIgnoreCase))
                    result.Add(files[i]);
            }

            return result;
        }

        internal static string GetFilePathForLocale(LocaleKey locale)
        {
            var files = GetFilesForLocale(locale);
            if (files.Count > 0)
                return files[0];

            string localeDir = Path.Combine(LocalizationFolderPath, locale.Value);
            if (Directory.Exists(localeDir))
                return Path.Combine(localeDir, $"{locale.Value}.yaml");

            return Path.Combine(LocalizationFolderPath, $"{locale.Value}.yaml");
        }

        internal static bool TryGetTranslation(LocaleKey locale, string dotKey, out string value)
        {
            value = null;
            if (string.IsNullOrEmpty(dotKey))
                return false;

            var files = GetFilesForLocale(locale);
            for (int i = 0; i < files.Count; i++)
            {
                if (!File.Exists(files[i])) continue;
                var root = ReadYaml(files[i]);
                if (TryGetPath(root, dotKey, out value))
                    return true;
            }

            return false;
        }

        internal static void SetTranslation(LocaleKey locale, string dotKey, string value)
        {
            if (string.IsNullOrEmpty(dotKey))
                return;

            var files = GetFilesForLocale(locale);
            string targetFile = files.Count > 0 ? files[0] : GetFilePathForLocale(locale);

            for (int i = 0; i < files.Count; i++)
            {
                if (!File.Exists(files[i])) continue;
                var testRoot = ReadYaml(files[i]);
                if (TryGetPath(testRoot, dotKey, out _))
                {
                    targetFile = files[i];
                    break;
                }
            }

            var root = File.Exists(targetFile) ? ReadYaml(targetFile) : new Dictionary<object, object>();
            SetPath(root, dotKey, value);
            WriteYaml(targetFile, root);
        }

        internal static void RenameKeyAcrossAllLocales(string oldKey, string newKey)
        {
            if (string.IsNullOrWhiteSpace(oldKey) || string.IsNullOrWhiteSpace(newKey) || oldKey == newKey)
                return;

            var locales = GetAvailableLocales();
            for (int i = 0; i < locales.Count; i++)
            {
                var locale = locales[i];
                string filePath = GetFilePathForLocale(locale);
                if (!File.Exists(filePath))
                    continue;

                var root = ReadYaml(filePath);
                if (TryGetPath(root, oldKey, out var existingValue))
                {
                    RemovePath(root, oldKey);
                    SetPath(root, newKey, existingValue);
                    WriteYaml(filePath, root);
                }
            }
        }

        private static Dictionary<object, object> ReadYaml(string filePath)
        {
            string content = File.ReadAllText(filePath);
            if (string.IsNullOrWhiteSpace(content))
                return new Dictionary<object, object>();

            using var reader = new StringReader(content);
            object obj = _deserializer.Deserialize(reader);
            if (obj is Dictionary<object, object> dict)
                return dict;

            return new Dictionary<object, object>();
        }

        private static void WriteYaml(string filePath, Dictionary<object, object> root)
        {
            string yaml = _serializer.Serialize(root);
            File.WriteAllText(filePath, yaml);
            AssetDatabase.Refresh();
        }

        private static bool TryGetPath(IDictionary dict, string dotKey, out string value)
        {
            value = null;
            string[] segments = dotKey.Split('.');
            IDictionary current = dict;

            for (int i = 0; i < segments.Length; i++)
            {
                string seg = segments[i];
                if (!current.Contains(seg))
                    return false;

                object next = current[seg];
                if (i == segments.Length - 1)
                {
                    value = next?.ToString();
                    return true;
                }

                if (next is IDictionary childDict)
                    current = childDict;
                else
                    return false;
            }

            return false;
        }

        private static void SetPath(IDictionary dict, string dotKey, string value)
        {
            string[] segments = dotKey.Split('.');
            IDictionary current = dict;

            for (int i = 0; i < segments.Length - 1; i++)
            {
                string seg = segments[i];
                if (current.Contains(seg) && current[seg] is IDictionary child)
                {
                    current = child;
                }
                else
                {
                    var newChild = new Dictionary<object, object>();
                    current[seg] = newChild;
                    current = newChild;
                }
            }

            current[segments[segments.Length - 1]] = value ?? string.Empty;
        }

        private static bool RemovePath(IDictionary dict, string dotKey)
        {
            string[] segments = dotKey.Split('.');
            var stack = new List<(IDictionary d, string k)>();
            IDictionary current = dict;

            for (int i = 0; i < segments.Length - 1; i++)
            {
                string seg = segments[i];
                if (!current.Contains(seg) || current[seg] is not IDictionary child)
                    return false;

                stack.Add((current, seg));
                current = child;
            }

            string leaf = segments[segments.Length - 1];
            bool removed = current.Contains(leaf);
            current.Remove(leaf);

            if (removed)
            {
                for (int i = stack.Count - 1; i >= 0; i--)
                {
                    var (parent, key) = stack[i];
                    if (parent[key] is IDictionary childDict && childDict.Count == 0)
                        parent.Remove(key);
                    else
                        break;
                }
            }

            return removed;
        }
    }
}
