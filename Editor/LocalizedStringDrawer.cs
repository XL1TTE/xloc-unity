using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace xLoc.Editor
{
    [CustomPropertyDrawer(typeof(LocalizedString))]
    public sealed class LocalizedStringDrawer : PropertyDrawer
    {
        private static int _selectedLocaleIndex;
        private static Object _lastTarget;
        private static string _lastPropertyPath;
        private static string _lastSyncKey;
        private static LocaleKey _lastSyncLocale;
        private static string _draftTranslation = string.Empty;

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;

            return (line * 3) + (spacing * 3) + 4;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginProperty(position, label, property);

            var keyProp = property.FindPropertyRelative("Key");
            var fallbackProp = property.FindPropertyRelative("Fallback");

            Object currentTarget = property.serializedObject.targetObject;
            string currentPath = property.propertyPath;
            string currentKey = keyProp.stringValue ?? string.Empty;

            float line = EditorGUIUtility.singleLineHeight;
            float spacing = EditorGUIUtility.standardVerticalSpacing;
            float y = position.y + 2;

            // --- 1. Key Field (Plain string, no file mutations) ---
            var keyRect = new Rect(position.x, y, position.width, line);
            EditorGUI.PropertyField(keyRect, keyProp, label);

            y += line + spacing;

            // --- 2. Localized Row (Preview + Manual Add/Save) ---
            List<LocaleKey> locales = YamlFileManager.GetAvailableLocales();
            var rowRect = new Rect(position.x, y, position.width, line);

            if (locales.Count == 0)
            {
                EditorGUI.LabelField(rowRect, "Localized", "(No YAML in Resources/Localization)");
            }
            else
            {
                if (_selectedLocaleIndex >= locales.Count)
                    _selectedLocaleIndex = 0;

                LocaleKey currentLocale = locales[_selectedLocaleIndex];

                float btnWidth = 42f;
                float addBtnWidth = 46f;
                float labelWidth = EditorGUIUtility.labelWidth;

                var lblRect = new Rect(position.x, y, labelWidth - btnWidth - 4, line);
                var btnRect = new Rect(position.x + labelWidth - btnWidth, y, btnWidth, line);
                var textRect = new Rect(position.x + labelWidth + 2, y, position.width - labelWidth - addBtnWidth - 6, line);
                var addRect = new Rect(position.x + position.width - addBtnWidth, y, addBtnWidth, line);

                EditorGUI.LabelField(lblRect, "Localized");

                if (GUI.Button(btnRect, currentLocale.Value.ToUpperInvariant(), EditorStyles.miniButton))
                {
                    _selectedLocaleIndex = (_selectedLocaleIndex + 1) % locales.Count;
                    currentLocale = locales[_selectedLocaleIndex];
                }

                // Synchronize draft when target, path, key, or locale changes
                if (_lastTarget != currentTarget ||
                    _lastPropertyPath != currentPath ||
                    _lastSyncKey != currentKey ||
                    _lastSyncLocale != currentLocale)
                {
                    _lastTarget = currentTarget;
                    _lastPropertyPath = currentPath;
                    _lastSyncKey = currentKey;
                    _lastSyncLocale = currentLocale;

                    if (!string.IsNullOrEmpty(currentKey) && YamlFileManager.TryGetTranslation(currentLocale, currentKey, out var existing))
                    {
                        _draftTranslation = existing;
                    }
                    else
                    {
                        _draftTranslation = string.Empty;
                    }
                }

                if (string.IsNullOrEmpty(currentKey))
                {
                    var emptyRect = new Rect(position.x + labelWidth + 2, y, position.width - labelWidth - 2, line);
                    EditorGUI.LabelField(emptyRect, "(Enter key first)");
                }
                else
                {
                    _draftTranslation = EditorGUI.TextField(textRect, _draftTranslation);

                    using (new EditorGUI.DisabledScope(string.IsNullOrWhiteSpace(_draftTranslation)))
                    {
                        if (GUI.Button(addRect, "Add", EditorStyles.miniButton))
                        {
                            YamlFileManager.SetTranslation(currentLocale, currentKey, _draftTranslation);
                            // Refresh draft to ensure in sync
                            if (YamlFileManager.TryGetTranslation(currentLocale, currentKey, out var saved))
                                _draftTranslation = saved;
                        }
                    }
                }
            }

            y += line + spacing;

            // --- 3. Fallback Field ---
            var fallbackRect = new Rect(position.x, y, position.width, line);
            EditorGUI.PropertyField(fallbackRect, fallbackProp, new GUIContent("Fallback"));

            EditorGUI.EndProperty();
        }
    }
}
