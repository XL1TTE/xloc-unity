# 🌐 xLoc

**xLoc** is a lightweight YAML localization framework for **Unity** with zero-boilerplate **TextMeshPro** binding, per-language font mapping, and in-inspector translation editing.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg)]()
[![TextMeshPro](https://img.shields.io/badge/TextMeshPro-Supported-brightgreen.svg)]()

---

## Features

* **Hierarchical YAML**: Clean nested YAML syntax (`items: sword: name: "Iron Sword"` -> `items.sword.name`).
* **Auto-Discovery**: Place `*_en.yaml`, `*_ru.yaml`, etc. anywhere in `Resources/Localization/`. Available languages are detected automatically.
* **Per-Language Font Mapping**: Assign default and source-to-target font mappings with custom scale multipliers per locale.
* **Auto-Updating UI**: Bind `TMP_Text` in one line (`text.BindLoc(...)`). Text and fonts update automatically when language changes.
* **Inspector Integration**: Edit and add translations directly inside the Unity Inspector while configuring prefabs and ScriptableObjects.
* **Configuration Window**: Standalone editor window (`Tools > xLoc > Configuration`) to manage locales and font assignments.
* **Typed Access**: Use `LocalizedString` struct in your data models with automatic conversion to `string` and fallback support.

---

## Installation

### Via Unity Package Manager (Git URL)

1. Open **Window** > **Package Manager** in Unity.
2. Click **+** > **Add package from git URL...**
3. Enter:
   ```text
   https://github.com/XL1TTE/xloc-unity.git
   ```

---

## Quick Start

### 1. Create Localization Files

Place your `.yaml` or `.yml` files in `Assets/Resources/Localization/`:

**`game_en.yaml`**:
```yaml
items:
  sword:
    name: "Iron Sword"
    desc: "A sharp steel blade."

ui:
  play: "Play"
  settings: "Settings"
```

**`game_ru.yaml`**:
```yaml
items:
  sword:
    name: "Железный меч"
    desc: "Острое стальное лезвие."

ui:
  play: "Играть"
  settings: "Настройки"
```

> The locale code is extracted from the file suffix (e.g. `items_en.yaml` -> `en`, `dialogue_ru.yaml` -> `ru`).

---

### 2. Configure Fonts

Open **Tools** > **xLoc** > **Configuration**:

1. Locales found in `Resources/Localization/` are listed on the left panel.
2. For each locale:
   * **Default Font**: Fallback `TMP_FontAsset` for text without a specific mapping.
   * **Font Mappings**: Map authoring fonts (`Source Font`) to regional equivalents (`Target Font`) (e.g. `EnglishPixelFont` -> `CyrillicPixelFont`).
   * **Scale Multiplier**: Adjust point size per font mapping (defaults to `1.0`) to compensate for language-specific character metrics.

Configurations are automatically saved to `Assets/Resources/Localization/xLocSettings.asset`.

---

### 3. Initialize in Code

Call `xLoc.Initialize()` once during your game bootstrap:

```csharp
using xLoc;

public class Bootstrapper
{
    public void Init()
    {
        xLoc.Initialize();
        
        // Optional: switch language explicitly
        xLoc.SetLocale((LocaleKey)"ru");
    }
}
```

---

### 4. Usage

#### Direct Lookup
```csharp
string name = xLoc.Get("items.sword.name");
string play = xLoc.Get("ui.play", fallback: "Play");

if (xLoc.TryGet("items.sword.desc", out string description))
{
    // ...
}
```

#### In Components & Prefabs
Use `LocalizedString` in your components, prefabs, and ScriptableObjects:

```csharp
using UnityEngine;
using xLoc;

public class ItemData : ScriptableObject
{
    public LocalizedString Title;
    public LocalizedString Description;
}

// Automatically converts to string:
string title = item.Title;
```

#### TextMeshPro Auto-Binding
Bind any `TMP_Text` in a single line. The text and font automatically update whenever the locale changes:

```csharp
using TMPro;
using UnityEngine;
using xLoc.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;
    [SerializeField] private TMP_Text _dynamicCounterText;

    public void Setup(ItemData item)
    {
        // 1. Localized text + font auto-swapping:
        _titleText.BindLoc(item.Title);
        // or by key:
        // _titleText.BindLoc("ui.play");

        // 2. Font-only auto-swapping (for numbers, player names, etc.):
        _dynamicCounterText.BindLoc();
    }
}
```

---

### 5. Inspector Workflow

When viewing a `LocalizedString` in the Inspector:

* **Key**: Enter the dot-notation key (e.g. `items.sword.name`).
* **Locale Button (`[EN]`)**: Click to cycle between available languages (`[EN]` ↔ `[RU]`).
* **Localized**: Displays the existing translation for the selected language. Edit text and click **Add** to save directly into that language's YAML file.
* **Fallback**: Default text returned if the translation is missing.

---

## License

This package is licensed under the [MIT License](LICENSE.md).
