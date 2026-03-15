using System;

namespace TinyHunter.Core.Character
{
    [Serializable]
    public class PlayerProfileData
    {
        public CharacterAppearanceData Appearance = CharacterAppearanceData.Default();
    }
}
