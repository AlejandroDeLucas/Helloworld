using TinyHunter.Data.Combat;
using UnityEngine;

namespace TinyHunter.Data.Items
{
    public enum WeaponClass
    {
        NeedleSpear,
        PinBlade,
        ButtonHammer,
        StapleBow,
        RazorShardAxe
    }

    [CreateAssetMenu(menuName = "TinyHunter/Data/Weapon")]
    public class WeaponDefinition : ScriptableObject
    {
        public string WeaponId;
        public string DisplayName;
        public WeaponClass WeaponClass;
        public DamageType DamageType;
        public float AttackPower = 10f;
        public float StaminaCostLight = 8f;
        public float StaminaCostHeavy = 18f;
        public float Reach = 1.2f;
        public bool CanBlock;
        public float GuardDamageReduction = 0.5f;
    }
}
