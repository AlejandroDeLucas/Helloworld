using TinyHunter.Core.Input;
using UnityEngine;

namespace TinyHunter.Core.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Vector3 offset = new(0f, 0.9f, -2.5f);
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float lookSensitivity = 120f;
        [SerializeField] private bool scaleLookByDeltaTime = true;
        [SerializeField] private float minPitch = -20f;
        [SerializeField] private float maxPitch = 45f;

        private float yaw;
        private float pitch = 20f;

        private void LateUpdate()
        {
            if (target == null || input == null) return;

            Vector2 look = ShouldApplyLookInput() ? input.Look : Vector2.zero;
            float lookScale = scaleLookByDeltaTime ? Time.deltaTime : 1f;
            yaw += look.x * lookSensitivity * lookScale;
            pitch -= look.y * lookSensitivity * lookScale;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + rotation * offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 0.5f);
        }
        private static bool ShouldApplyLookInput()
        {
            if (!Application.isFocused) return false;

            Vector3 mousePos = UnityEngine.Input.mousePosition;
            return mousePos.x >= 0f
                   && mousePos.y >= 0f
                   && mousePos.x <= Screen.width
                   && mousePos.y <= Screen.height;
        }
    }
}
