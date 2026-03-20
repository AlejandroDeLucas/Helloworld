using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class PlayerMvpCharacterSetup : MonoBehaviour
    {
        [SerializeField] private CharacterSkinApplicator skinApplicator;
        [SerializeField] private EyeColorController eyeColorController;
        [SerializeField] private WeaponHolder weaponHolder;

        [Header("Initial Setup")]
        [SerializeField] private int skinIndex;
        [SerializeField] private Color eyeColor = Color.white;
        [SerializeField] private GameObject startingWeapon;

        private void Awake()
        {
            if (skinApplicator != null)
            {
                skinApplicator.SetManualIndex(skinIndex);
            }

            eyeColorController?.ApplyEyeColor(eyeColor);

            if (startingWeapon != null)
            {
                weaponHolder?.EquipWeapon(startingWeapon);
            }
        }

        public void ApplyAppearance(int newSkinIndex, Color newEyeColor)
        {
            skinApplicator?.SetManualIndex(newSkinIndex);
            eyeColorController?.ApplyEyeColor(newEyeColor);
        }
    }
}
