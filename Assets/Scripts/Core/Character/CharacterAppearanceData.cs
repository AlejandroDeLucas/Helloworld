using System;
using UnityEngine;

namespace TinyHunter.Core.Character
{
    [Serializable]
    public class CharacterAppearanceData
    {
        public string CharacterName = "Hunter";
        [Tooltip("0=Male, 1=Female")]
        public int SexIndex;
        [Tooltip("0=Normal, 1=Slim, 2=Strong, 3=Heavy")]
        public int BodyTypeIndex;
        public int HeadPresetIndex;
        public int HairPresetIndex;
        public int SkinToneIndex;
        public int HairColorIndex;

        public static CharacterAppearanceData Default()
        {
            return new CharacterAppearanceData
            {
                CharacterName = "Hunter",
                SexIndex = 0,
                BodyTypeIndex = 0,
                HeadPresetIndex = 0,
                HairPresetIndex = 0,
                SkinToneIndex = 0,
                HairColorIndex = 0
            };
        }
    }
}
