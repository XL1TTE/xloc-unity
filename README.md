# 🌐 xLoc

**xLoc** is a lightweight YAML localization framework for **Unity** with zero-boilerplate **TextMeshPro** binding and in-inspector translation editing.

[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)
[![Unity](https://img.shields.io/badge/Unity-2022.3%2B-blue.svg)]()
[![TextMeshPro](https://img.shields.io/badge/TextMeshPro-Supported-brightgreen.svg)]()

---

## Features

* **Hierarchical YAML**: Clean nested YAML syntax (`items: sword: name: "Iron Sword"` -> `items.sword.name`).
* **Auto-Discovery**: Place `*_en.yml`, `*_es.yml`, etc. anywhere in `Resources/Localization/`. Available languages are detected automatically.
* **Auto-Updating UI**: Bind `TMP_Text` in one line (`text.BindLoc(...)`). Text updates automatically whenever language changes.
* **Inspector Integration**: Edit and add translations directly inside the Unity Inspector while configuring prefabs and ScriptableObjects.
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

Place your `.yml` files in `Assets/Resources/Localization/`:

**`game_en.yml`**:
```yaml
items:
  sword:
    name: "Iron Sword"
    desc: "A sharp steel blade."

ui:
  play: "Play"
  settings: "Settings"
```

**`game_es.yml`**:
```yaml
items:
  sword:
    name: "Espada de hierro"
    desc: "Una hoja de acero afilada."

ui:
  play: "Jugar"
  settings: "Ajustes"
```

> The locale code is extracted from the file suffix (e.g. `items_en.yml` -> `en`, `dialogue_es.yml` -> `es`).

---

### 2. Initialize in Code

Call `xLoc.Initialize()` once during your game bootstrap:

```csharp
using xLoc;

public class Bootstrapper
{
    public void Init()
    {
        xLoc.Initialize();
        
        // Optional: switch language explicitly
        xLoc.SetLocale((LocaleKey)"es");
    }
}
```

---

### 3. Usage

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
Bind any `TMP_Text` in a single line. The text automatically updates whenever the locale changes:

```csharp
using TMPro;
using UnityEngine;
using xLoc.UI;

public class ItemView : MonoBehaviour
{
    [SerializeField] private TMP_Text _titleText;

    public void Setup(ItemData item)
    {
        _titleText.BindLoc(item.Title);
        // or by key:
        // _titleText.BindLoc("ui.play");
    }
}
```

---

### 4. Inspector Workflow

When viewing a `LocalizedString` in the Inspector:

* **Key**: Enter the dot-notation key (e.g. `items.sword.name`).
* **Locale Button (`[EN]`)**: Click to cycle between available languages (`[EN]` ↔ `[ES]`).
* **Localized**: Displays the existing translation for the selected language. Edit text and click **Add** to save directly into that language's YAML file.
* **Fallback**: Default text returned if the translation is missing.

---

## License

This package is licensed under the [MIT License](LICENSE.md).
