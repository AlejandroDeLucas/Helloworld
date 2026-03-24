# Tiny Hunter - MVP Second Pass Setup Guide (Unity)

## Control Scheme (standardized)
- **WASD**: Move
- **Mouse**: Camera look
- **Space**: Jump
- **C**: Crouch toggle
- **E**: Interact / pick up / confirm craft in crafting panel
- **LMB**: Primary attack
- **RMB**: Guard/block (if equipped weapon supports block)
- **R**: Lock/Unlock target
- **Left Shift (hold)**: Sprint/Run while moving
- **Left Shift (tap, no movement)**: Dodge roll
- **Tab**: Toggle Inventory + Equipment panels
- **Escape**: Close panels or open/close pause menu
- **F5/F9**: Save/Load debug hotkeys

Input is centralized in `PlayerInputReader` and consumed by gameplay/UI systems.

## Scenes
Create three scenes:
1. `MainMenu`
2. `Hub_MVP`
3. `Kitchen_MVP`

## Global bootstrap object (in MainMenu)
Create `GameBootstrap` with:
- `SceneFlowController`
- `SaveSystem`

Mark both as scene-persistent via built-in script behavior (`DontDestroyOnLoad`).

## Player prefab (`PF_Player`)
Required components:
- `CharacterController`
- `PlayerInputReader`
- `PlayerStats`
- `PlayerController`
- `PlayerAnimationBridge`
- `TargetLockSystem`
- `WeaponCombatSystem`
- `InventorySystem`
- `EquipmentSystem`
- `PlayerInteractionSystem`
- `PlayerDeathHandler`

Camera rig:
- `ThirdPersonCameraController` (assign target + input)
- Main Camera child with `CameraShake`

## Core UI in Hub and Kitchen
Canvas objects/scripts:
- `PlayerHUD`
- `QuestHUD`
- `InteractionPromptUI`
- `CraftingPanelUI`
- `InventoryPanelUI`
- `EquipmentPanelUI`
- `DebugChecklistUI`
- `PauseMenuUI`
- `UIInputRouter`


## HUD quick wiring checklist (avoid "New Text")
- In `PlayerHUD`, assign `playerStats`, `lockSystem`, `healthBar`, `staminaBar`, `targetText`.
- In `QuestHUD`, assign `questSystem`, `activeQuestText`, `progressText`.
- In `InteractionPromptUI`, assign `root` + `promptText` and set root inactive by default.
- Replace placeholder labels in scene (`New Text`) with intended HUD text objects and ensure they are referenced by the scripts above.

## Hub_MVP contents
- Player spawn
- `QuestBoard` (`QuestBoardInteractable`)
- `CraftingBench` (`CraftingStationInteractable`)
- `HuntGate` (`EnterHuntInteractable`)
- storage placeholder

## Kitchen_MVP contents
- Player spawn
- arena floor + cover clutter
- ant spawn (`PF_AntSoldier`)
- spider ambush spawn (`PF_Spider`)
- return trigger (`HuntExitTrigger`)

Bake NavMesh for enemy movement.

## Enemy prefab wiring
### Ant Soldier
- `AntSoldierAI` + `MonsterHealth` + `MonsterPartBreakSystem` + `NavMeshAgent`
- child hitbox colliders each with `MonsterPartHitbox` IDs: `head`, `mandible`, `leg`

### Spider
- `SpiderAmbushAI` + `MonsterHealth` + `MonsterPartBreakSystem` + `NavMeshAgent`
- child hitbox IDs: `fang`, `abdomen`, `leg`

## Data authoring
Use ScriptableObjects for all data and create:
- Item assets (drops + craft outputs)
- ItemDatabase asset (all item defs)
- Weapon/Armor defs
- Monster part defs
- Monster defs + loot tables
- Quest defs
- Crafting recipes

## Save/load wiring
`SaveSystem` needs:
- `ItemDatabase`
- known quest list for restore

Save data includes:
- inventory quantities
- equipped weapon
- equipped head armor
- active quest id

## Death/respawn loop
- `PlayerDeathHandler` watches `PlayerStats.IsDead`
- shows death pause state
- after delay, returns player to `Hub_MVP`

## What is testable now
1. Main menu start/continue.
2. Hub quest accept with `E`.
3. Enter hunt scene.
4. Standard third-person control + jump/crouch/dodge/guard.
5. Lock-on with `R`.
6. Kill enemies, break parts, pick up loot.
7. Return to hub, craft, auto-equip crafted gear.
8. Toggle inventory/equipment (`Tab`).
9. Save (`F5`) and load (`F9`) or continue from menu.


## Character creator (prepared, not active)
This MVP includes dormant preparation only:
- `CharacterAppearanceData`
- `PlayerProfileData`
- `CharacterCustomizer`

Keep `CharacterCustomizer.applyOnStart` **disabled** so current gameplay flow remains unchanged.

For full activation later, follow:
- `Docs/CharacterCreator_Preparation.md`
- `Docs/Tutorials/Beginner_Model_Animation_Import_Tutorial.md`


## Platform readiness components (PC first)
Add these optional components now for future PC/Android/iOS split:
- `PlatformBootstrap` on a persistent bootstrap object
- `AdaptiveCanvasScaler` on each gameplay canvas
- `PlayerInputReader` sources:
  - desktop source -> `DesktopInputSource`
  - mobile source -> `MobileInputSourceStub` (future touch wiring)

Create one `GamePlatformSettings` ScriptableObject and assign it to:
- `PlatformBootstrap`
- `AdaptiveCanvasScaler`

Keep `PlayerInputReader.forceDesktopInput = true` during current MVP PC playtests.
