using System;
using UnityEngine;

namespace TinyHunter.Data.Platform
{
    [Serializable]
    public class PlatformTuning
    {
        public int TargetFrameRate = 60;
        public int QualityLevel = 2;
        [Range(0f, 1f)] public float CanvasMatchWidthOrHeight = 0.5f;
        public int MaxRecommendedEnemies = 3;
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Platform Settings")]
    public class GamePlatformSettings : ScriptableObject
    {
        public PlatformTuning Pc = new();
        public PlatformTuning Mobile = new()
        {
            TargetFrameRate = 30,
            QualityLevel = 1,
            CanvasMatchWidthOrHeight = 0.65f,
            MaxRecommendedEnemies = 2
        };
    }
}
