using System.Collections;
using TinyHunter.Core.Camera;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public class HitFeedbackSystem : MonoBehaviour
    {
        public static HitFeedbackSystem Instance { get; private set; }

        [SerializeField] private float hitStopDuration = 0.03f;
        [SerializeField] private CameraShake cameraShake;

        private void Awake()
        {
            Instance = this;
        }

        public void PlayHitFeedback(float shakeIntensity = 0.08f, float shakeTime = 0.08f)
        {
            StartCoroutine(HitStopCoroutine(hitStopDuration));
            if (cameraShake != null)
            {
                cameraShake.Shake(shakeIntensity, shakeTime);
            }
        }

        private IEnumerator HitStopCoroutine(float duration)
        {
            Time.timeScale = 0f;
            yield return new WaitForSecondsRealtime(duration);
            Time.timeScale = 1f;
        }
    }
}
