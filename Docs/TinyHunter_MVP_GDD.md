# Tiny Hunter - Production Foundation (Unity/C#)

## 1) High-level game vision summary
Tiny Hunter is a third-person 3D action-hunting game where the player is a **2 mm-tall human hunter** navigating a modern house as a lethal macro-scale wilderness. The game loop is intentionally Monster Hunter-like: prepare loadout, track target, fight a high-threat arthropod, break specific body parts, harvest anatomical materials, and craft stronger equipment at a safe hub.

The fantasy is grounded: familiar domestic spaces (kitchen tile, cabinet gaps, sink runoff, crumbs, dust, and clutter) become traversal and combat challenges at miniature human scale.

## 2) Core gameplay pillars
1. **Hunting**: tracking and challenging real-time boss-like insect encounters.
2. **Preparation**: gear, resistances, consumables, and route planning before departure.
3. **Anatomical progression**: parts and organs unlock weapon/armor branches.
4. **Scale-first exploration**: every room is realistic, but transformed by perspective.
5. **High tension**: player fragility, readable enemy attacks, stamina/positioning decisions.
6. **Grounded domestic ecology**: creature behavior tied to moisture, darkness, crumbs, and nesting.

## 3) Worldbuilding and scale rules
- Player is always around **2 mm tall**; do not treat them as microscopic.
- Household assets remain recognizable and realistic.
- Terrain conversions at this scale:
  - Dust = loose debris field.
  - Crumb = carryable resource and partial cover.
  - Tile seam = trench/crevice.
  - Water droplet = impact hazard.
  - Small puddle = route blocker unless equipped.
  - Cabinet toe-kick gap = tunnel system.
- Keep naming and visual language non-comedic and non-whimsical.

## 4) Main areas of the house and design rules
- **Kitchen (MVP combat zone)**: crumbs, sink splash, grease streaks, under-cabinet darkness; ant and spider overlaps.
- **Bathroom**: humidity, drain suction risk, soap slip lanes, silverfish/mosquito habitats.
- **Bedroom**: under-bed predator routes, textile folds for stealth, cable crossing lines.
- **Living room**: carpet friction terrain, warm electronics, sofa underside nests.
- **Hallway**: fast-travel connector with periodic airflow hazards.
- **Laundry**: heat/vibration hazards near appliances, roach activity corridors.
- **Garage**: dry dust, oils, metallic scrap for late-tier weapon crafting.
- **Patio/Garden**: open arena-style hunts, larger beetles and mantis classes.

## 5) Creature roster (early/mid/late)
### Early
- **Ant Worker/Ant Soldier** (group pressure, route denial).
- **House Spider (small)** (web traps, ambush).
- **Silverfish** (fast nuisance, gather quest target).

### Mid
- **Cockroach** (armored sprinter, hard stagger).
- **Wasp Scout** (aerial dive attacker).
- **Cricket** (burst displacement attacks).

### Late
- **Widow Spider** (arena control, venom focus).
- **Wasp Matriarch** (air superiority + summon behavior).
- **Mantis Stalker** (duel boss, long windup precision strikes).

## 6) Combat system design
- Third-person lock-on, stamina-gated offense/defense.
- Light/heavy attacks with recovery windows.
- Directional dodge with i-frame tuning by gear.
- Weak-point multiplier and damage-type matching.
- **Part break system** with separate durability pools.
- Status channels: poison, acid, impact trauma, water stagger.
- Time-to-kill target: 4–8 minutes for MVP major hunts.

## 7) Weapon classes
1. **Needle Spear**: reach + precise weak-point pressure.
2. **Pin Blade**: fast, low commitment, ideal for evasive targets.
3. **Button Hammer**: slow blunt breaker (future milestone).
4. **Staple Bow**: anti-air option (post-MVP).
5. **Razor Shard Axe**: sever-centric hybrid (post-MVP).

MVP ships with Needle Spear + Pin Blade.

## 8) Armor and crafting progression
- Slots: Head/Chest/Arms/Waist/Legs.
- Ant set: traction + stagger resistance.
- Spider set: poison mitigation + web slow resistance.
- Armor stats influence movement/stamina handling instead of pure HP inflation.
- Crafting requires anatomical drops plus household scrap/fiber binders.

## 9) Hunt loop and quest flow
1. Accept quest at hub board.
2. Equip weapon, armor, consumables.
3. Enter kitchen route instance.
4. Track signs (pheromone trail, web anchors).
5. Fight target; break relevant parts.
6. Harvest rewards.
7. Return to hub.
8. Craft or upgrade.
9. Unlock next hunt tier.

## 10) Hub/base design
**Wall-void refuge near kitchen utility line**:
- Craft bench (forge equivalent).
- Quest board.
- Storage trunk.
- NPCs: scout, fabricator, anatomist, quartermaster.
- Training lane with dummy insect carapace targets.

## 11) Technical architecture in Unity
Layered modular architecture:
- **Data layer**: ScriptableObjects for definitions.
- **Runtime systems**: inventory, combat, part break, crafting, quests.
- **Presentation layer**: camera, animation hooks, VFX/SFX events.

Avoid monoliths: each concern has dedicated component and clear interfaces (`IDamageable`).

## 12) Recommended folder structure
```text
Assets/
  Scripts/
    Core/
      Player/
      Camera/
      Combat/
      Inventory/
      Crafting/
      Quest/
      Equipment/
      Hub/
      Save/
    Data/
      Combat/
      Items/
      Monsters/
      Crafting/
      Quests/
    MVP/
      Enemies/
Docs/
  TinyHunter_MVP_GDD.md
```

## 13) ScriptableObject data model proposal
- `ItemDefinition` (all item primitives).
- `WeaponDefinition` / `ArmorDefinition`.
- `MonsterDefinition` (stats, ecology, parts, weakness).
- `MonsterPartDefinition` (part durability + break reward/effect).
- `LootTableDefinition` (`LootEntry` rarity/chance/amount).
- `CraftingRecipeDefinition` (`MaterialCost[]`, unlock condition).
- `QuestDefinition` (objective, target, timer, reward).

## 14) MVP scope
Playable target in 1 scene + 1 hub:
- Hub with quest accept + crafting station.
- Kitchen floor/lower cabinet hunt arena.
- Creatures: Ant Soldier + Spider.
- Weapons: Needle Spear + Pin Blade.
- Systems: third-person movement, lock-on, stamina, combat hits, part breaks, loot, inventory, crafting, basic quest completion.

## 15) Step-by-step implementation plan
1. Create ScriptableObject classes (items/monsters/quests/crafting).
2. Implement `InventorySystem`, `CraftingSystem`, `QuestSystem`.
3. Build player core: `PlayerStats`, `PlayerController`, lock-on, camera.
4. Build combat core: `WeaponCombatSystem`, `IDamageable`.
5. Build enemy health and part break systems.
6. Implement Ant Soldier AI and Spider ambush AI.
7. Set up kitchen graybox arena and navmesh.
8. Author initial SO assets for ant/spider drops and recipes.
9. Connect quest completion to monster defeat events.
10. Add UI pass (HP/stamina, lock target, quest progress, inventory/craft panel).
11. Balance pass for stamina drain, dodge windows, TTK.

## 16) Initial C# scripts for MVP
Implemented in repository:
- Core player/camera/combat/inventory/crafting/quest scripts.
- ScriptableObject definitions for all MVP data domains.

## 17) Example enemy implementation
- `AntSoldierAI`: chase + bite cooldown, tuned for pressure combat.
- `SpiderAmbushAI`: hidden hold, trigger radius, lunge + venom burst behavior.

## 18) Example crafting recipe setup
**Needle Spear Mk1** (unlock: first Ant Soldier slay)
- 4x Chitin Shard (common)
- 2x Mandible Edge (uncommon)
- 1x Thread Fiber (common)
- 1x Metal Splinter (common)

**Spider Hood** (unlock: first Spider slay)
- 3x Silk Gland (common)
- 1x Fang Shard (rare)
- 2x Dust Resin Clump (common)

## 19) Suggestions for future expansion
- Turf interactions (ant vs spider territorial fights).
- Environmental traps (water droplet timing, fan airflow windows).
- Capture tools (thread net, sedative resin).
- Multi-room expedition chains.
- Advanced AI sensing (pheromone, vibration, light aversion).
- Co-op (2 hunters) with role-specialized loadouts.
