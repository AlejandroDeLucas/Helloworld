using TinyHunter.Data.Characters;
using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public enum SkinAssignmentMode
    {
        ManualIndex = 0,
        RandomOnStart = 1
    }

    public class CharacterSkinApplicator : MonoBehaviour
    {
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private CharacterSkinSetDefinition skinSet;
        [SerializeField] private SkinAssignmentMode assignmentMode = SkinAssignmentMode.ManualIndex;
        [SerializeField] private int manualIndex;

        private Material runtimeMaterial;

        private void Awake()
        {
            ApplySkin();
        }

        public void ApplySkin()
        {
            if (bodyRenderer == null || skinSet == null || !skinSet.HasEntries) return;

            Material source = assignmentMode == SkinAssignmentMode.RandomOnStart
                ? skinSet.GetRandom()
                : skinSet.GetClamped(manualIndex);

            if (source == null) return;

            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
            }

            runtimeMaterial = new Material(source);
            bodyRenderer.material = runtimeMaterial;
        }


        private void OnDestroy()
        {
            if (runtimeMaterial != null)
            {
                Destroy(runtimeMaterial);
                runtimeMaterial = null;
            }
        }

        public void RandomizeSkin()
        {
            assignmentMode = SkinAssignmentMode.RandomOnStart;
            ApplySkin();
        }

        public void SetManualIndex(int index)
        {
            manualIndex = index;
            assignmentMode = SkinAssignmentMode.ManualIndex;
            ApplySkin();
        }
    }
}
