# Tiny Hunter - Character Creator Preparation (Dormant Foundation)

This document explains what is prepared now for a **future** character creator and what is **not active yet**.

## Current status
- ✅ Data model for appearance exists.
- ✅ `CharacterCustomizer` component exists and is safe to keep dormant.
- ✅ Save data is forward-compatible with appearance profile info.
- ✅ Asset naming conventions and prefab hierarchy are documented.
- ❌ No creator UI/menu flow is active yet.
- ❌ No forced character creation step at game start.

## Data model prepared
- `CharacterAppearanceData`
  - `CharacterName`
  - `SexIndex` (0 male, 1 female)
  - `BodyTypeIndex` (0 normal, 1 slim, 2 strong, 3 heavy)
  - `HeadPresetIndex`
  - `HairPresetIndex`
  - `SkinToneIndex`
  - `HairColorIndex`
- `PlayerProfileData`
  - `Appearance`

## Dormant customizer component
Script: `CharacterCustomizer`

Prepared hooks:
- `bodyRoot`, `headRoot`, `hairSocket`
- male/female body preset arrays
- head preset array
- hair preset array
- skin/hair color palette arrays

Current behavior:
- Does not affect gameplay loop unless manually used.
- Can safely apply only skin/hair color if configured.
- `applyOnStart` defaults to off for non-invasive behavior.

## Expected future player visual hierarchy
Use this structure when you are ready to add final character visuals:

```text
CharacterRoot
  VisualRoot
    BodyRoot
    HeadRoot
    HairSocket
    Armor_Head
    Armor_Chest
    Armor_Arms
    Armor_Waist
    Armor_Legs
```

This keeps body/head/hair modular and armor-ready.

## Future asset naming convention
### Body presets
- `Male_Normal`
- `Male_Slim`
- `Male_Strong`
- `Male_Heavy`
- `Female_Normal`
- `Female_Slim`
- `Female_Strong`
- `Female_Heavy`

### Head presets
- `HeadPreset_01`
- `HeadPreset_02`
- `HeadPreset_03`
- `HeadPreset_04`
- `HeadPreset_05`

### Hair presets
- `Hair_01`
- `Hair_02`
- `Hair_03`
- `Hair_04`
- `Hair_05`
- `Hair_06`

## Save/load compatibility prepared
`SaveData` now includes `PlayerProfileData profile`.

Meaning:
- Existing saves can continue to work.
- Future creator selections can be stored without redesigning the save format.

## What to do later to activate character creator
1. Build a creator UI (name, sex, body type, head preset, hair preset, skin tone, hair color).
2. Add a creator scene or menu panel.
3. Fill `CharacterCustomizer` arrays with final body/head/hair assets.
4. Feed chosen values into `PlayerProfileData.Appearance`.
5. Save and load that profile through `SaveSystem`.
6. Call `CharacterCustomizer.ApplyAppearance(...)` when spawning player.
7. Optionally add preview camera and rotate character in creator UI.

## Intentionally not implemented yet
- No sliders/blendshape facial editor.
- No runtime body/head swap UI.
- No forced startup character creation flow.
- No additional complexity that can break current MVP.
