using TinyHunter.Data.Combat;
using UnityEngine;

namespace TinyHunter.Core.Combat
{
    public interface IDamageable
    {
        void TakeHit(float damage, DamageType damageType, string hitPartId = null, Vector3? hitPoint = null);
    }
}
