# Tiny Hunter - Platform Readiness Guide (PC First, Mobile Ready Later)

This pass keeps Tiny Hunter **PC-first playable** while preparing architecture for future Android/iOS adaptation.

## 1) Input architecture readiness

### Current setup
- `PlayerInputReader` is now an input facade.
- `DesktopInputSource` provides the current PC control scheme.
- `MobileInputSourceStub` exists as future touch extension point.

### Why this helps
Gameplay scripts read from `PlayerInputReader`, not directly from `UnityEngine.Input`. This reduces rework when adding touch controls.

### Current PC mapping
- WASD: Move
- Mouse: Camera look
- LMB: Attack
- RMB: Guard
- Left Shift: Dodge
- Space: Jump
- C: Crouch
- E: Interact/confirm
- R: Lock-on toggle
- Tab: Inventory/Equipment toggle
- Escape: Pause/close panels
- F5/F9: Save/Load debug

### Future mobile mapping (proposal)
- Left virtual stick: Move
- Right drag zone: Camera look
- Attack button
- Guard button
- Dodge button
- Interact button
- Lock-on button
- Jump button
- Crouch button
- Optional pause/settings button

`MobileInputSourceStub` is where UI button events should feed gameplay input later.

## 2) UI scaling / responsiveness readiness

### Current preparation
- Add `AdaptiveCanvasScaler` to each gameplay canvas.
- `AdaptiveCanvasScaler` applies `ScaleWithScreenSize` and platform-specific `matchWidthOrHeight`.
- Uses reference resolution `1920x1080` by default.

### Anchor/layout guidance
- Keep core HUD anchored to corners (health top-left, quest top-right, prompts bottom-center).
- Keep large panels centered with margins.
- Avoid fixed pixel offsets near notches/safe areas for phone layouts.

## 3) Platform config foundation

### New data object
`GamePlatformSettings` ScriptableObject:
- PC tuning (target FPS, quality level, UI scale matching, recommended enemy budget)
- Mobile tuning (lower quality/FPS defaults, mobile-leaning UI scale match)

### Runtime application
`PlatformBootstrap` applies:
- `Application.targetFrameRate`
- `QualitySettings` level

This keeps platform divergence in data instead of hardcoded logic.

## 4) Performance awareness (mobile-oriented notes)

Prepared by architecture/docs:
- Mobile profile defaults to lower quality and frame target.
- Enemy budget documented in platform settings.
- Input abstraction minimizes expensive per-platform branching in gameplay scripts.

Still recommended later:
- Reduce post-processing for mobile.
- Use LODs for creatures/player visuals.
- Keep active enemy count modest in mobile hunts.
- Simplify VFX and reduce transparent overdraw.

## 5) Build preparation notes (not full publishing)

### Windows (PC)
Later steps:
1. Add `Windows` build target.
2. Set resolution defaults and fullscreen mode.
3. Build and run local executable.

### Android
Later steps:
1. Switch to Android platform in Build Settings.
2. Configure package ID + min/target SDK.
3. Add touch UI and wire to `MobileInputSourceStub`.
4. Test performance profile + UI scale on real devices.

### iOS
Later steps:
1. Switch to iOS platform in Build Settings.
2. Configure bundle identifier/signing in Xcode.
3. Add touch UI and safe-area adjustments.
4. Validate performance profile on iPhone hardware.

## 6) What is prepared vs postponed

### Prepared now
- Input abstraction for desktop vs mobile source swapping.
- Platform config ScriptableObject and runtime bootstrap hooks.
- Adaptive canvas scaling hook for multi-resolution behavior.
- Documentation for controls, mobile layout proposal, and build paths.

### Intentionally postponed
- Final touch control UI implementation.
- Safe-area specific iPhone layout pass.
- Store packaging/signing/certificates.
- Full graphics optimization pass for production mobile performance.
