using UnityEngine;

namespace TinyHunter.Core.Character
{
    public class CharacterCustomizer : MonoBehaviour
    {
        [Header("Future Visual Roots")]
        [SerializeField] private Transform bodyRoot;
        [SerializeField] private Transform headRoot;
        [SerializeField] private Transform hairSocket;

        [Header("Future Preset Prefabs (Dormant)")]
        [SerializeField] private GameObject[] maleBodyPresets;
        [SerializeField] private GameObject[] femaleBodyPresets;
        [SerializeField] private GameObject[] headPresets;
        [SerializeField] private GameObject[] hairPresets;

        [Header("Future Materials")]
        [SerializeField] private Renderer[] skinRenderers;
        [SerializeField] private Renderer[] hairRenderers;
        [SerializeField] private Color[] skinTonePalette;
        [SerializeField] private Color[] hairColorPalette;

        [Header("Safety")]
        [SerializeField] private bool applyOnStart;

        [SerializeField] private CharacterAppearanceData debugAppearance = CharacterAppearanceData.Default();

        private void Start()
        {
            if (!applyOnStart) return;
            ApplyAppearance(debugAppearance);
        }

        public void ApplyAppearance(CharacterAppearanceData appearance)
        {
            if (appearance == null) return;

            // Dormant foundation:
            // 1) Body preset by SexIndex + BodyTypeIndex
            // 2) Head preset by HeadPresetIndex
            // 3) Hair preset by HairPresetIndex
            // 4) Skin/hair material colors by palette index
            // This currently validates indices and applies only colors to avoid affecting active MVP flow.

            ApplySkinColor(appearance.SkinToneIndex);
            ApplyHairColor(appearance.HairColorIndex);
        }

        public void ApplySkinColor(int index)
        {
            if (skinTonePalette == null || skinTonePalette.Length == 0) return;
            index = Mathf.Clamp(index, 0, skinTonePalette.Length - 1);
            var color = skinTonePalette[index];

            foreach (var rendererRef in skinRenderers)
            {
                if (rendererRef == null || rendererRef.material == null) continue;
                rendererRef.material.color = color;
            }
        }

        public void ApplyHairColor(int index)
        {
            if (hairColorPalette == null || hairColorPalette.Length == 0) return;
            index = Mathf.Clamp(index, 0, hairColorPalette.Length - 1);
            var color = hairColorPalette[index];

            foreach (var rendererRef in hairRenderers)
            {
                if (rendererRef == null || rendererRef.material == null) continue;
                rendererRef.material.color = color;
            }
        }

        public void ApplyAppearanceFromProfile(PlayerProfileData profile)
        {
            if (profile == null) return;
            ApplyAppearance(profile.Appearance);
        }
    }
}
