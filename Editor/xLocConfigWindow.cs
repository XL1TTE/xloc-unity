using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEngine;

namespace xLoc.Editor
{
    public sealed class xLocConfigWindow : EditorWindow
    {
        private const string AssetFolderPath = "Assets/Resources/Localization";
        private const string AssetPath = "Assets/Resources/Localization/xLocSettings.asset";

        private xLocSettings _settings;
        private List<WorkingConfig> _workingConfigs = new();
        private int _selectedLocaleIndex;
        private Vector2 _leftScroll;
        private Vector2 _rightScroll;
        private string _newLocaleInput = string.Empty;

        private sealed class WorkingConfig
        {
            public LocaleKey Locale;
            public TMP_FontAsset DefaultFont;
            public List<WorkingMapping> Mappings = new();
        }

        private sealed class WorkingMapping
        {
            public TMP_FontAsset SourceFont;
            public TMP_FontAsset TargetFont;
            public float ScaleMultiplier = 1f;
        }

        [MenuItem("Tools/xLoc/Configuration")]
        public static void Open()
        {
            var window = GetWindow<xLocConfigWindow>("xLoc Config");
            window.minSize = new Vector2(550, 350);
            window.Show();
        }

        private void OnEnable()
        {
            EnsureAssetExists();
            LoadWorkingConfigs();
            SyncWithYamlLocales();
        }

        private void EnsureAssetExists()
        {
            _settings = AssetDatabase.LoadAssetAtPath<xLocSettings>(AssetPath);
            if (_settings == null)
            {
                if (!Directory.Exists(AssetFolderPath))
                    Directory.CreateDirectory(AssetFolderPath);

                _settings = CreateInstance<xLocSettings>();
                AssetDatabase.CreateAsset(_settings, AssetPath);
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        private void LoadWorkingConfigs()
        {
            _workingConfigs.Clear();
            if (_settings == null)
                return;

            foreach (var cfg in _settings.LocaleFonts)
            {
                var working = new WorkingConfig
                {
                    Locale = cfg.Locale,
                    DefaultFont = cfg.DefaultFont,
                    Mappings = cfg.FontMappings.Select(m => new WorkingMapping
                    {
                        SourceFont = m.SourceFont,
                        TargetFont = m.TargetFont,
                        ScaleMultiplier = m.ScaleMultiplier
                    }).ToList()
                };
                _workingConfigs.Add(working);
            }
        }

        private void SyncWithYamlLocales()
        {
            var yamlLocales = YamlFileManager.GetAvailableLocales();
            bool changed = false;

            foreach (var locale in yamlLocales)
            {
                if (!_workingConfigs.Any(c => c.Locale == locale))
                {
                    _workingConfigs.Add(new WorkingConfig { Locale = locale });
                    changed = true;
                }
            }

            if (changed)
                SaveSettings();
        }

        private void SaveSettings()
        {
            if (_settings == null)
                return;

            var configsToSave = new List<LocaleFontConfig>();
            foreach (var w in _workingConfigs)
            {
                var mappings = w.Mappings.Select(m => new FontMapping(m.SourceFont, m.TargetFont, m.ScaleMultiplier));
                configsToSave.Add(new LocaleFontConfig(w.Locale, w.DefaultFont, mappings));
            }

            Undo.RecordObject(_settings, "Modify xLoc Settings");
            _settings.SetConfigs(configsToSave);
            EditorUtility.SetDirty(_settings);
            AssetDatabase.SaveAssets();
        }

        private void OnGUI()
        {
            if (_settings == null)
                EnsureAssetExists();

            DrawHeader();

            EditorGUILayout.Space(6);

            EditorGUILayout.BeginHorizontal();
            DrawLeftPanel();
            DrawRightPanel();
            EditorGUILayout.EndHorizontal();
        }

        private void DrawHeader()
        {
            using (new EditorGUILayout.HorizontalScope(EditorStyles.toolbar))
            {
                GUILayout.Label("xLoc Font & Locale Configuration", EditorStyles.boldLabel);
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Sync YAML", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    SyncWithYamlLocales();
                }
                if (GUILayout.Button("Ping Asset", EditorStyles.toolbarButton, GUILayout.Width(70)))
                {
                    EditorGUIUtility.PingObject(_settings);
                }
            }
        }

        private void DrawLeftPanel()
        {
            EditorGUILayout.BeginVertical(GUILayout.Width(170));
            GUILayout.Label("Locales", EditorStyles.boldLabel);

            _leftScroll = EditorGUILayout.BeginScrollView(_leftScroll, EditorStyles.helpBox);
            for (int i = 0; i < _workingConfigs.Count; i++)
            {
                var cfg = _workingConfigs[i];
                bool isSelected = i == _selectedLocaleIndex;

                GUIStyle style = isSelected ? EditorStyles.miniButtonMid : EditorStyles.miniButton;
                if (isSelected)
                {
                    GUI.backgroundColor = new Color(0.24f, 0.48f, 0.90f, 1f);
                }

                if (GUILayout.Button(cfg.Locale.Value.ToUpperInvariant(), style, GUILayout.Height(24)))
                {
                    _selectedLocaleIndex = i;
                    GUI.FocusControl(null);
                }

                GUI.backgroundColor = Color.white;
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.BeginHorizontal();
            _newLocaleInput = EditorGUILayout.TextField(_newLocaleInput);
            if (GUILayout.Button("+", GUILayout.Width(25)))
            {
                if (!string.IsNullOrWhiteSpace(_newLocaleInput))
                {
                    try
                    {
                        var key = new LocaleKey(_newLocaleInput);
                        if (!_workingConfigs.Any(c => c.Locale == key))
                        {
                            _workingConfigs.Add(new WorkingConfig { Locale = key });
                            _selectedLocaleIndex = _workingConfigs.Count - 1;
                            _newLocaleInput = string.Empty;
                            SaveSettings();
                            GUI.FocusControl(null);
                        }
                    }
                    catch (Exception ex)
                    {
                        EditorUtility.DisplayDialog("Invalid Locale", ex.Message, "OK");
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
        }

        private void DrawRightPanel()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            if (_workingConfigs.Count == 0)
            {
                EditorGUILayout.HelpBox("No locales found. Add a locale or create a YAML file in Assets/Resources/Localization.", MessageType.Info);
                EditorGUILayout.EndVertical();
                return;
            }

            if (_selectedLocaleIndex < 0 || _selectedLocaleIndex >= _workingConfigs.Count)
                _selectedLocaleIndex = 0;

            var current = _workingConfigs[_selectedLocaleIndex];

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label($"Locale: {current.Locale.Value.ToUpperInvariant()}", EditorStyles.boldLabel);
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Remove Locale", EditorStyles.miniButton, GUILayout.Width(100)))
            {
                if (EditorUtility.DisplayDialog("Remove Locale", $"Remove font configuration for '{current.Locale.Value}'?", "Yes", "No"))
                {
                    _workingConfigs.RemoveAt(_selectedLocaleIndex);
                    if (_selectedLocaleIndex >= _workingConfigs.Count)
                        _selectedLocaleIndex = Mathf.Max(0, _workingConfigs.Count - 1);
                    SaveSettings();
                    EditorGUILayout.EndHorizontal();
                    EditorGUILayout.EndVertical();
                    return;
                }
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(4);

            EditorGUI.BeginChangeCheck();

            // Default Font
            current.DefaultFont = (TMP_FontAsset)EditorGUILayout.ObjectField(
                new GUIContent("Default Font", "Fallback font asset for this locale when no specific mapping matches."),
                current.DefaultFont,
                typeof(TMP_FontAsset),
                false
            );

            EditorGUILayout.Space(8);
            GUILayout.Label("Font Mappings (Source -> Target)", EditorStyles.boldLabel);

            _rightScroll = EditorGUILayout.BeginScrollView(_rightScroll);

            int removeIndex = -1;
            for (int i = 0; i < current.Mappings.Count; i++)
            {
                var mapping = current.Mappings[i];
                EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);

                EditorGUILayout.BeginVertical();
                mapping.SourceFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Source Font", mapping.SourceFont, typeof(TMP_FontAsset), false);
                mapping.TargetFont = (TMP_FontAsset)EditorGUILayout.ObjectField("Target Font", mapping.TargetFont, typeof(TMP_FontAsset), false);
                mapping.ScaleMultiplier = EditorGUILayout.FloatField("Scale Multiplier", mapping.ScaleMultiplier <= 0f ? 1f : mapping.ScaleMultiplier);
                if (mapping.ScaleMultiplier <= 0f) mapping.ScaleMultiplier = 1f;
                EditorGUILayout.EndVertical();

                if (GUILayout.Button("X", GUILayout.Width(22), GUILayout.Height(40)))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
                EditorGUILayout.Space(2);
            }

            if (removeIndex >= 0)
            {
                current.Mappings.RemoveAt(removeIndex);
            }

            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("+ Add Font Mapping", GUILayout.Height(24)))
            {
                current.Mappings.Add(new WorkingMapping());
            }

            if (EditorGUI.EndChangeCheck())
            {
                SaveSettings();
            }

            EditorGUILayout.EndVertical();
        }
    }
}
