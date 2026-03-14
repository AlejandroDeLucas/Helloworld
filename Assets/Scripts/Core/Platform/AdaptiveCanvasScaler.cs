using TinyHunter.Data.Platform;
using UnityEngine;
using UnityEngine.UI;

namespace TinyHunter.Core.Platform
{
    [RequireComponent(typeof(CanvasScaler))]
    public class AdaptiveCanvasScaler : MonoBehaviour
    {
        [SerializeField] private GamePlatformSettings settings;
        [SerializeField] private Vector2 referenceResolution = new(1920f, 1080f);
        [SerializeField] private bool treatEditorAsPc = true;

        private void Awake()
        {
            var scaler = GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = referenceResolution;

            if (settings == null)
            {
                scaler.matchWidthOrHeight = 0.5f;
                return;
            }

            bool isMobile = Application.isMobilePlatform && !(Application.isEditor && treatEditorAsPc);
            scaler.matchWidthOrHeight = isMobile
                ? settings.Mobile.CanvasMatchWidthOrHeight
                : settings.Pc.CanvasMatchWidthOrHeight;
        }
    }
}
