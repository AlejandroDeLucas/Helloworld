using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class HumanoidCharacter : MonoBehaviour
    {
        [Header("Core")]
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterStats stats;

        [Header("Body & Eyes")]
        [SerializeField] private Renderer bodyRenderer;
        [SerializeField] private Renderer leftEyeRenderer;
        [SerializeField] private Renderer rightEyeRenderer;

        [Header("Sockets")]
        [SerializeField] private CharacterSocketMap socketMap;

        public Animator Animator => animator;
        public CharacterStats Stats => stats;
        public Renderer BodyRenderer => bodyRenderer;
        public Renderer LeftEyeRenderer => leftEyeRenderer;
        public Renderer RightEyeRenderer => rightEyeRenderer;
        public CharacterSocketMap SocketMap => socketMap;

        private void Reset()
        {
            if (animator == null) animator = GetComponentInChildren<Animator>();
            if (stats == null) stats = GetComponent<CharacterStats>();
            if (socketMap == null) socketMap = GetComponent<CharacterSocketMap>();
        }
    }
}
