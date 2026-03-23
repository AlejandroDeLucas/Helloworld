using UnityEngine;

namespace TinyHunter.Data.Characters
{
    [CreateAssetMenu(menuName = "TinyHunter/Data/Character Stats")]
    public class CharacterStatsDefinition : ScriptableObject
    {
        [Min(1f)] public float MaxHealth = 100f;
        [Min(0f)] public float Damage = 10f;
        [Min(0.1f)] public float AttackSpeed = 1f;
        [Min(0f)] public float MoveSpeed = 2.5f;
        [Min(0f)] public float Defense = 2f;
    }
}
