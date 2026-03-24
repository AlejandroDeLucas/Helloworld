using UnityEngine;

namespace TinyHunter.Data.Characters
{
    [CreateAssetMenu(menuName = "TinyHunter/Data/Character Skin Set")]
    public class CharacterSkinSetDefinition : ScriptableObject
    {
        [Tooltip("All materials/textures must use the same UV layout and shader.")]
        public Material[] SkinMaterials;

        public bool HasEntries => SkinMaterials != null && SkinMaterials.Length > 0;

        public Material GetClamped(int index)
        {
            if (!HasEntries) return null;
            index = Mathf.Clamp(index, 0, SkinMaterials.Length - 1);
            return SkinMaterials[index];
        }

        public Material GetRandom()
        {
            if (!HasEntries) return null;
            return SkinMaterials[Random.Range(0, SkinMaterials.Length)];
        }
    }
}
