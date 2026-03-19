using System.Collections;
using UnityEngine;

namespace TinyHunter.Core.Camera
{
    public class CameraShake : MonoBehaviour
    {
        private Vector3 defaultLocalPosition;

        private void Awake()
        {
            defaultLocalPosition = transform.localPosition;
        }

        public void Shake(float intensity, float duration)
        {
            StopAllCoroutines();
            StartCoroutine(ShakeCoroutine(intensity, duration));
        }

        private IEnumerator ShakeCoroutine(float intensity, float duration)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.unscaledDeltaTime;
                transform.localPosition = defaultLocalPosition + Random.insideUnitSphere * intensity;
                yield return null;
            }

            transform.localPosition = defaultLocalPosition;
        }
    }
}
