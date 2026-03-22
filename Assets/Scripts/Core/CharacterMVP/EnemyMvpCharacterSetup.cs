using UnityEngine;

namespace TinyHunter.Core.CharacterMVP
{
    public class EnemyMvpCharacterSetup : MonoBehaviour
    {
        [SerializeField] private CharacterSkinApplicator skinApplicator;
        [SerializeField] private WeaponHolder weaponHolder;
        [SerializeField] private GameObject[] possibleWeapons;
        [SerializeField] private bool randomWeaponOnStart;

        private void Awake()
        {
            if (skinApplicator != null)
            {
                // Keep enemy variation cheap: randomize only materials on same rig/mesh.
                skinApplicator.RandomizeSkin();
            }

            if (randomWeaponOnStart && weaponHolder != null && possibleWeapons != null && possibleWeapons.Length > 0)
            {
                var prefab = possibleWeapons[Random.Range(0, possibleWeapons.Length)];
                if (prefab != null) weaponHolder.EquipWeapon(prefab);
            }
        }
    }
}
