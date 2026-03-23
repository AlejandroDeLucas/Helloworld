# Tiny Hunter MVP (Unity/C#)

This repository contains a playable PC-first MVP for a grounded 2 mm-scale third-person action-hunt prototype.

## Current gameplay scope
- Hub -> Hunt -> Return loop
- Third-person combat and lock-on
- Part breaking and loot pickup
- Crafting and equipment
- Quest progression
- Save/load and scene flow

## Character creator status (important)
A **future-ready, dormant** character creator foundation is prepared, but NOT active in current gameplay flow.

Prepared now:
- Appearance/profile data classes
- `CharacterCustomizer` component hooks
- Save compatibility for appearance profile data
- Naming conventions + prefab hierarchy docs
- Beginner-friendly model/animation import tutorial

Not active yet:
- No character creator UI screen
- No forced creator at game start
- No advanced facial morph/sliders

## Platform readiness status (important)
This pass prepares architecture for future PC/Android/iOS adaptation while keeping MVP PC-first.

Prepared now:
- Input source abstraction (`PlayerInputReader` + desktop/mobile source components)
- Platform settings data profile (`GamePlatformSettings`)
- Runtime platform bootstrap for quality/FPS defaults
- Adaptive canvas scaling hook for cross-resolution UI behavior
- Platform documentation and future build path notes

Not active yet:
- No final touch controls UI
- No Android/iOS publishing/signing/store packaging
- No full mobile optimization pass

## Controls (PC)
- WASD move, Mouse look
- Space jump, C crouch
- LMB attack, RMB guard
- Left Shift dodge, R lock target
- E interact/confirm
- Tab inventory + equipment
- Escape close/open pause
- F5/F9 save/load debug

## Setup docs
- MVP scene setup: `Docs/Unity_MVP_Scene_Setup_Guide.md`
- Creator preparation: `Docs/CharacterCreator_Preparation.md`
- Beginner tutorial: `Docs/Tutorials/Beginner_Model_Animation_Import_Tutorial.md`
- Platform readiness: `Docs/Platform_Readiness_Guide.md`
