using TinyHunter.Data.Platform;
using UnityEngine;

namespace TinyHunter.Core.Platform
{
    public class PlatformBootstrap : MonoBehaviour
    {
        [SerializeField] private GamePlatformSettings settings;
        [SerializeField] private bool treatEditorAsPc = true;

        private void Awake()
        {
            if (settings == null) return;

            bool isMobile = Application.isMobilePlatform && !(Application.isEditor && treatEditorAsPc);
            var selected = isMobile ? settings.Mobile : settings.Pc;

            Application.targetFrameRate = selected.TargetFrameRate;

            if (selected.QualityLevel >= 0 && selected.QualityLevel < QualitySettings.names.Length)
            {
                QualitySettings.SetQualityLevel(selected.QualityLevel, true);
            }
        }
    }
}
