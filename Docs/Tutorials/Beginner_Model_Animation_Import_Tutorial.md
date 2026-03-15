# Tiny Hunter Beginner Tutorial (Very Simple)

This tutorial is written for beginners. Imagine you are 12 and adding assets for the first time.

You will learn how to add:
- player models (male + female)
- creature models (ant, spider)
- animations
- prefab links

Do not worry. Go step by step.

---

## 1) Before you start

You need:
- Unity project open
- Your model files (`.fbx` is common)
- Texture files (`.png`, `.tga`, etc.)
- Animation files (`.fbx` or `.anim`)

Make folders in Unity Project panel:

```text
Assets/
  Art/
    Characters/
      Male/
      Female/
    Creatures/
      Ant/
      Spider/
    Materials/
    Textures/
    Animations/
```

---

## 2) Import player models (male/female)

1. Drag your male model file into `Assets/Art/Characters/Male`.
2. Drag your female model file into `Assets/Art/Characters/Female`.
3. Click model file in Unity.
4. In Inspector, check:
   - **Rig** tab
   - Animation Type = `Humanoid` (or `Generic` if your model is not humanoid-rigged)
5. Press **Apply**.

Tip: If rig errors appear, use `Generic` first so you can keep moving.

---

## 3) Import creature models (ant/spider)

1. Drag ant files into `Assets/Art/Creatures/Ant`.
2. Drag spider files into `Assets/Art/Creatures/Spider`.
3. Set Rig = `Generic` for most creature rigs.
4. Press **Apply**.

---

## 4) Create materials

1. Right click `Assets/Art/Materials` -> Create -> Material.
2. Make materials like:
   - `MAT_MaleBody`
   - `MAT_FemaleBody`
   - `MAT_Ant`
   - `MAT_Spider`
3. Drag textures into material slots (Base Map, Normal, etc.).
4. Drag materials onto model meshes.

---

## 5) Import animations

1. Put player animations in `Assets/Art/Animations/Player`.
2. Put ant animations in `Assets/Art/Animations/Ant`.
3. Put spider animations in `Assets/Art/Animations/Spider`.
4. Select each animation source file.
5. In Inspector -> Animations tab, verify clips exist.

Player clips you usually want:
- Idle
- Walk/Run
- LightAttack
- HeavyAttack
- Dodge
- Hit
- Death

Creature clips you usually want:
- Idle
- Move
- Attack
- Hit
- Death

---

## 6) Build the player visual hierarchy (important)

Inside your player prefab, make this structure:

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

Put your body mesh under `BodyRoot`.
Put head mesh under `HeadRoot`.
Put hair under `HairSocket`.

---

## 7) Add CharacterCustomizer (future-ready)

1. Select player prefab root.
2. Add `CharacterCustomizer` component.
3. Assign:
   - `bodyRoot`
   - `headRoot`
   - `hairSocket`
4. Keep `applyOnStart = false` for now (very important, so MVP flow stays stable).
5. Optionally set skin/hair color palettes.

---

## 8) Add Animator safely

1. Add `Animator` to player visual root.
2. Create an Animator Controller:
   - `AC_Player`
3. Add parameters/triggers:
   - `LightAttack`
   - `HeavyAttack`
   - `Dodge`
   - `Hit`
   - `Death`
4. Assign `Animator` reference in `PlayerAnimationBridge`.

If you don’t finish state machine now, it’s okay. Triggers can stay ready.

---

## 9) Make creature prefabs

### Ant prefab
- Add mesh + material
- Keep AI scripts already in project
- Add collider(s) and child `MonsterPartHitbox` objects:
  - `head`
  - `mandible`
  - `leg`

### Spider prefab
- Add mesh + material
- Keep AI scripts already in project
- Add child `MonsterPartHitbox` objects:
  - `fang`
  - `abdomen`
  - `leg`

---

## 10) Quick test checklist

After importing, press Play and test:
1. Player appears correctly.
2. Ant and spider appear correctly.
3. Attack can hit enemies.
4. Loot still drops.
5. No missing material (pink mesh).
6. No missing script errors in Console.

---

## 11) Common beginner mistakes

- **Pink model**: material/shader missing.
- **Model too big/small**: set import scale in model import settings.
- **No animation**: Animator controller not assigned.
- **No hit detection**: forgot colliders or `MonsterPartHitbox` IDs.
- **Character creator breaks MVP**: forgot to keep `applyOnStart` disabled.

---

## 12) When you are ready for full creator later

Use `Docs/CharacterCreator_Preparation.md`.
It tells you exactly how to activate the creator in steps.
