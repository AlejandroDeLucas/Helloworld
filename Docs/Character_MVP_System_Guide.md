# Character MVP System Guide

## Goals
- One reusable humanoid architecture for player and enemies.
- Shared rig + mesh with texture/material variants.
- Eye meshes separated from body material logic.
- Runtime weapon attachment using hand sockets.
- Clean path to modular armor/accessories without rewrite.

## Suggested Folder Structure
- `Assets/Scripts/Core/CharacterMVP`
  - Runtime character composition scripts (`HumanoidCharacter`, sockets, equipment, skins, eyes, setup helpers)
- `Assets/Scripts/Data/Characters`
  - `CharacterStatsDefinition` (base stat presets)
  - `CharacterSkinSetDefinition` (skin material presets)

## Prefab Hierarchy Example
```text
Player_Humanoid (root)
├── CharacterRoot (Animator on this object)
│   ├── Armature
│   │   └── Hips/.../Head
│   │       ├── Eye_L (MeshRenderer)
│   │       └── Eye_R (MeshRenderer)
│   ├── BodyMesh (SkinnedMeshRenderer)
│   ├── Socket_RightHand (Transform under RightHand bone)
│   ├── Socket_LeftHand (Transform under LeftHand bone)
│   ├── Socket_Back (Transform under spine/chest)
│   ├── Socket_Head
│   ├── Socket_Shoulders
│   ├── Socket_Belt
│   ├── Socket_Beard
│   └── Socket_Backpack
└── Components on root:
    - HumanoidCharacter
    - CharacterStats
    - CharacterSocketMap
    - CharacterEquipment
    - WeaponHolder
    - CharacterSkinApplicator
    - EyeColorController
    - PlayerMvpCharacterSetup
```

Enemy uses the same structure, swapping setup script for `EnemyMvpCharacterSetup`.

## Unity Setup
1. Create `CharacterStatsDefinition` assets (Player_BaseStats, Goblin_BaseStats).
2. Create `CharacterSkinSetDefinition` assets using materials that share the same shader + UV layout.
3. In character prefabs assign references:
   - `HumanoidCharacter`: body renderer, eye renderers, animator, stats, socket map.
   - `CharacterStats`: assign base definition.
   - `CharacterSocketMap`: map slot enum to each socket transform.
   - `CharacterSkinApplicator`: body renderer + skin set + assignment mode.
   - `EyeColorController`: left/right eye renderers.
   - `WeaponHolder`: character equipment + default slot (RightHand).
4. Player prefab:
   - `CharacterSkinApplicator` in manual mode.
   - `PlayerMvpCharacterSetup` with `skinIndex`, eye color and optional starting weapon.
5. Enemy prefab:
   - `CharacterSkinApplicator` in random mode or use `EnemyMvpCharacterSetup`.
   - Optional random weapon list.

## Eye Setup Notes (Blender -> Mixamo -> Unity)
- Keep eye meshes parented/skinned to the same head hierarchy before exporting.
- Ensure both eye objects are exported as separate mesh objects.
- In Unity, do not detach eyes from rig hierarchy; assign their renderers to `EyeColorController`.
- Since eyes are separate renderers, eye color can be changed with material properties without creating new body textures.

## Mixamo-Friendly Notes
- Upload one base humanoid mesh to Mixamo and keep skeleton naming stable.
- Keep sockets as empty transforms under the animated bones inside Unity prefab.
- Reuse the same avatar/rig across texture variants to avoid duplicate models.

## Shared Material Safety
- `CharacterSkinApplicator` creates a runtime material instance so enemy randomization does not modify shared project materials.
- `EyeColorController` uses `MaterialPropertyBlock` to avoid extra material instances where possible.

## Future Expansion Path
- Equip any category with `CharacterEquipment.Equip(slot, prefab)`.
- Add new `EquipmentSlotType` values for new modular parts.
- Keep armor/beard/backpack as separate prefab meshes attached to mapped sockets.
