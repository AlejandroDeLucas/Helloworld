using TinyHunter.Core.Input;
using UnityEngine;

namespace TinyHunter.Core.Camera
{
    public class ThirdPersonCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private PlayerInputReader input;
        [SerializeField] private Vector3 offset = new(0f, 1.2f, -2.5f);
        [SerializeField] private float smoothSpeed = 8f;
        [SerializeField] private float lookSensitivity = 120f;
        [SerializeField] private float minPitch = -20f;
        [SerializeField] private float maxPitch = 45f;

        private float yaw;
        private float pitch = 20f;

        private void LateUpdate()
        {
            if (target == null || input == null) return;

            var look = input.Look;
            yaw += look.x * lookSensitivity * Time.deltaTime;
            pitch -= look.y * lookSensitivity * Time.deltaTime;
            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
            Vector3 desiredPosition = target.position + rotation * offset;
            transform.position = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed * Time.deltaTime);
            transform.LookAt(target.position + Vector3.up * 0.5f);
        }
    }
}
