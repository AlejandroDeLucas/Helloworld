using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class EyeColorController : MonoBehaviour
    {
        [SerializeField] private Renderer leftEyeRenderer;
        [SerializeField] private Renderer rightEyeRenderer;
        [SerializeField] private string colorProperty = "_BaseColor";
        [SerializeField] private Color eyeColor = Color.white;
        [SerializeField] private bool applyOnStart = true;

        private MaterialPropertyBlock block;
        private int colorPropertyId;

        private void Awake()
        {
            colorPropertyId = Shader.PropertyToID(colorProperty);
            block = new MaterialPropertyBlock();

            if (applyOnStart)
            {
                ApplyEyeColor(eyeColor);
            }
        }

        public void ApplyEyeColor(Color color)
        {
            eyeColor = color;
            ApplyToRenderer(leftEyeRenderer, eyeColor);
            ApplyToRenderer(rightEyeRenderer, eyeColor);
        }

        private void ApplyToRenderer(Renderer rendererRef, Color color)
        {
            if (rendererRef == null) return;

            rendererRef.GetPropertyBlock(block);
            block.SetColor(colorPropertyId, color);
            rendererRef.SetPropertyBlock(block);
        }
    }
}
